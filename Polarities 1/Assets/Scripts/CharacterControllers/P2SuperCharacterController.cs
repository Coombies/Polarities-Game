using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Handles all movement for the red player.
/// </summary>
public class SuperCharacterController2 : MonoBehaviour
{
    // Base variables
    private float xMovement = 0;
    private float yMovement = 0;
    private float previousYVelocity = 0;
    private float previousXVelocity = 0;
    private float coyoteJump = 0;
    private float gravityModifier = 1;
    private float bufferJump = 0;
    private float groundAcceleration = 0;
    private float airAcceleration = 0;
    private float groundDeceleration = 0;
    private float airDeceleration = 0;
    private float moveSpeed = 0;

    private bool isFacingRight = false;
    private bool isJumping = false;

    private Vector2 movement = new(0, 0);

    private readonly List<CompositeCollider2D> groundColliders =
        new();

    // Variables to be referenced in the inspector
    [Header("Default Object References")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private Transform ceilingBoxRight;
    [SerializeField] private Transform ceilingBoxLeft;

    [Header("Collisions and Rigidbody")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D playerCol;
    [SerializeField] private BoxCollider2D playerHitbox;
    [SerializeField] private BoxCollider2D otherPlayerCol;
    [SerializeField] private ScriptableStats stats;

    // Modded Variables

    // Ladder Variables
    private bool isLadder;
    private bool isClimbing;

    // Ice Variables
    private float accelerationModifier = 1;

    // One Way Platform Variables
    private GameObject currentOneWay;
    private bool ignoreGround;

    [Header("Modded Collisions")]
    [SerializeField] private Transform hurtBox;
    [SerializeField] private LayerMask hazardLayer;
    [SerializeField] private LayerMask otherPlayerLayer;


    /// <summary>
    /// Links to Scriptable Stats class.
    /// Disables collisions for upside down platforms and other player.
    /// </summary>
    private void Start()
    {
        // Links class Scriptable Stats, filled with all constant variables
        // for convenience
        GameObject[] groundObjects =
            GameObject.FindGameObjectsWithTag("Ground");
        foreach (GameObject obj in groundObjects)
        {
            CompositeCollider2D objectCol =
                obj.GetComponent<CompositeCollider2D>();
            if (objectCol != null)
            {
                groundColliders.Add(objectCol);
            }
        }

        // Ensures that the players can't collide
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(playerCol, otherPlayerCol);

        // Disables collisions with upside down passable platforms
        GameObject oneWayObject =
            GameObject.FindGameObjectWithTag("OneWayUp");
        CompositeCollider2D oneWayCol =
            oneWayObject.GetComponent<CompositeCollider2D>();
        Physics2D.IgnoreCollision(playerCol, oneWayCol);
    }


    /// <summary>
    /// Calls all methods which include player input.
    /// </summary>
    private void Update()
    {
        GetInput();
        CheckJumping();
        Flip();

        // Modded methods
        CheckLadder();
        OneWayPlatform();
    }


    /// <summary>
    /// Calls all methods which either apply movement
    /// or calculate movement-related logic.
    /// </summary>
    private void FixedUpdate()
    {
        PreviousVariables();
        ApplyMovement();
        ApplyGravity();
        HandleXMovement();
        EdgeHandling();

        // Modded Methods
        ClimbLadder();
        CheckHazards();
    }


    /// <summary>
    /// Applies the Vector2 movement to the characters rigid body
    /// </summary>
    private void ApplyMovement() => rb.velocity = -movement;


    /// <summary>
    /// Saves the characters movement from the previous frame.
    /// Despite not being used in the program, I decided is necessary
    /// for future proofing my program
    /// </summary>
    private void PreviousVariables()
    {
        previousYVelocity = movement.y;
        previousXVelocity = movement.x;
    }


    /// <summary>
    /// Gets the players WASD input and calculates the acceleration and speed
    /// accordingly
    /// </summary>
    private void GetInput()
    {
        // Movement returns -1, 0, or 1
        xMovement = Input.GetAxisRaw("Horizontal");
        yMovement = Input.GetAxisRaw("Vertical");

        // Applies acceleration modifier if player is on ice
        if (IsIce())
        {
            accelerationModifier = stats.IceAccelerationModifier;
        }
        else if (IsGrounded())
        {
            accelerationModifier = 1;
        }

        // There is air acceleration/deceleration, and a different speed for
        // sprint and walk for maximum control over the platformers movement
        if (Input.GetKey(KeyCode.LeftShift) &&
            Mathf.Abs(movement.x) >= stats.NormalSpeed)
        {
            groundAcceleration = stats.SprintGroundAcceleration
                * accelerationModifier;
            groundDeceleration = stats.SprintGroundDeceleration
                * accelerationModifier;
            airAcceleration = stats.SprintAirAcceleration
                * accelerationModifier;
            airDeceleration = stats.SprintAirDeceleration
                * accelerationModifier;

            moveSpeed = stats.SprintSpeed;
        }
        else
        {
            groundAcceleration = stats.NormalGroundAcceleration
                * accelerationModifier;
            groundDeceleration = stats.NormalGroundDeceleration
                * accelerationModifier;
            airAcceleration = stats.NormalAirAcceleration
                * accelerationModifier;
            airDeceleration = stats.NormalAirDeceleration
                * accelerationModifier;

            moveSpeed = stats.NormalSpeed;
        }
    }

    
    /// <summary>
    /// Handles the players lateral acceleration.
    /// </summary>
    private void HandleXMovement()
    {
        if (xMovement == 0)
        {
            // If grounded, apply ground deceleration,
            // otherwise apply air deceleration
            var deceleration = IsGrounded() ?
                groundDeceleration : airDeceleration;

            movement.x = Mathf.MoveTowards(movement.x, 0, deceleration
                * Time.fixedDeltaTime);
        }
        else
        {
            // If grounded, apply ground acceleration,
            // otherwise apply air acceleration
            var acceleration = IsGrounded() ?
                groundAcceleration : airAcceleration;

            movement.x = Mathf.MoveTowards(movement.x,
                xMovement * moveSpeed,
                acceleration * Time.fixedDeltaTime);

        }
    }

    
    /// <summary>
    /// Handles jumping.
    /// </summary>
    private void CheckJumping()
    {
        // Allows player to perform a jump for a short time after
        // falling off a platform
        if ((IsGrounded() || isClimbing) && !isJumping)
        {
            coyoteJump = stats.CoyoteTime; // Reset coyote time
        }
        else
        {
            coyoteJump = Mathf.Max(0f, coyoteJump - Time.deltaTime);
        }

        // Allows player to input jump a moment before they are grounded
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            bufferJump = stats.JumpBufferTime; // Reset jump buffer
            isClimbing = false; // Stops player climbing for better ladders
        }
        else
        {
            bufferJump = Mathf.Max(0f, bufferJump - Time.deltaTime);
        }

        // Applies jump force if conditions are met
        if (coyoteJump > 0f && bufferJump > 0f)
        {
            movement.y = stats.JumpForce; // Apply jump force
            bufferJump = 0; // Reset buffer jump
            isJumping = true;
        }

        if (isJumping)
        {
            // Clamps the minimum jump height of the player
            if (!Input.GetKey(KeyCode.Space) &&
                movement.y > 0 &&
                movement.y < stats.MinJumpHeightThreshold)
            {
                movement.y *= stats.JumpHeightModifier;
                coyoteJump = 0f;
                isJumping = false;
            }

            if (movement.y <= 0) // Player is falling
            {
                isJumping = false; // Reset jumping state when falling
            }
        }
    }


    /// <summary>
    /// Checks to see if the player ground check is colliding with the 
    /// ground layer.
    /// Disables collisions with upside down passable platforms.
    /// </summary>
    /// <returns>
    /// Returns true if the is a valid collision, otherwise
    /// reutrns false.
    /// </returns>
    private bool IsGrounded()
    {
        // Perform OverlapBoxAll to get all colliders in the box
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            groundCheck.position,
            new Vector2(stats.HitboxBase, stats.HitboxHeight),
            0f,
            groundLayer
        );

        // Iterate through the colliders and check if
        // any of them are not tagged as "OneWay"
        foreach (var collider in colliders)
        {
            if (!collider.CompareTag("OneWayUp"))
            {
                // If we find a valid collision that is not "OneWay",
                // return true
                return true;
            }
        }

        // If no valid collisions are found, return false
        return false;
    }


    /// <summary>
    /// Checks to see if the player ceiling check is colliding with the 
    /// ground layer.
    /// Disables collisions with upside down passable platforms.
    /// </summary>
    /// <returns>
    /// Returns true if the is a valid collision, otherwise
    /// reutrns false.
    /// </returns>
    private bool TouchingCeiling()
    {
        // Perform OverlapBoxAll to get all colliders in the box
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            ceilingCheck.position,
            new Vector2(stats.HitboxBase, stats.HitboxHeight),
            0f,
            groundLayer
        );

        // Iterate through the colliders and check if any of them are not
        // tagged as "OneWay"
        foreach (var collider in colliders)
        {
            if (!collider.CompareTag("OneWayDown"))
            {
                // If we find a valid collision that is not "OneWay",
                // return true
                return true;
            }
        }

        // If no valid collisions are found, return false
        return false;
    }


    /// <summary>
    /// Flips the player depending on which direction they are moving.
    /// Useful for animations and sprites.
    /// </summary>
    private void Flip()
    {
        if (isFacingRight && xMovement < 0f ||
            !isFacingRight && xMovement > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }


    /// <summary>
    /// Handles the downward forces on the player.
    /// </summary>
    private void ApplyGravity()
    {

        if (!IsGrounded() &&
            Mathf.Abs(movement.y) < stats.VerticalSpeedApexThreshold)
        {
            gravityModifier = stats.GraceGravityModifier;
        }
        else
        {
            gravityModifier = 1;
        }

        if (IsGrounded() && !ignoreGround && movement.y <= 0f)
        {
            movement.y = stats.WeightForce;
        }
        else if (Input.GetAxisRaw("Vertical") < 0 &&
            movement.y <= stats.FastFallActuationSpeed)
        {
            movement.y = Mathf.MoveTowards(movement.y,
                -stats.FastFallSpeed,
                stats.FastFallAcceleration * gravityModifier * Time.deltaTime);
        }
        else
        {
            movement.y = Mathf.MoveTowards(movement.y,
                -stats.SlowFallSpeed,
                stats.GravityAcceleration * gravityModifier * Time.deltaTime);
        }

    }


    /// <summary>
    /// Nudges the player to the side of a platform if only
    /// the left or right check detects a collision.
    /// Otherwise, act as a standard ceiling.
    /// </summary>
    private void EdgeHandling()
    {

        bool leftCheck = false;
        bool rightCheck = false;

        // First, detect all overlapping colliders
        Collider2D[] collidersLeft = Physics2D.OverlapBoxAll(
            ceilingBoxLeft.position +
            new Vector3(
                -(1 - stats.CeilingBoxSize + stats.CeilingBoxPosition) / 2f,
                    0f,
                    0f
            ),
            new Vector2(stats.CeilingBoxSize - 0.05f, 0.1f),
            0f,
            groundLayer
        );

        Collider2D[] collidersRight = Physics2D.OverlapBoxAll(
            ceilingBoxLeft.position +
            new Vector3(
                (1 - stats.CeilingBoxSize + stats.CeilingBoxPosition) / 2f,
                    0f,
                    0f
            ),
            new Vector2(stats.CeilingBoxSize - 0.05f, 0.1f),
            0f,
            groundLayer
        );

        // Now, filter out the ones tagged as "OneWay"
        foreach (var collider in collidersLeft)
        {
            if (!collider.CompareTag("OneWayDown"))
            {
                // If the collider is not tagged as "OneWay", it's a
                // valid collision
                leftCheck = true;
                break;
            }
        }

        foreach (var collider in collidersRight)
        {
            if (!collider.CompareTag("OneWayDown"))
            {
                // If the collider is not tagged as "OneWay", it's a
                // valid collision
                rightCheck = true;
                break;
            }
        }

        foreach (Collider2D objectCol in groundColliders)
        {

            if (rightCheck && !leftCheck)
            {
                Physics2D.IgnoreCollision(playerCol, objectCol, true);
                rb.AddForce(
                    Vector2.left * (stats.ClipForce + Mathf.Abs(xMovement)),
                    ForceMode2D.Impulse
                );
            }
            else if (leftCheck && !rightCheck)
            {
                Physics2D.IgnoreCollision(playerCol, objectCol, true);
                rb.AddForce(
                    Vector2.right * (stats.ClipForce + Mathf.Abs(xMovement)),
                    ForceMode2D.Impulse
                );
            }
            else if (TouchingCeiling())
            {
                Physics2D.IgnoreCollision(playerCol, objectCol, false);
                movement.y = Mathf.Abs(movement.y) * -stats.CeilingBounce;
            }
            else
            {
                Physics2D.IgnoreCollision(playerCol, objectCol, false);
            }
        }

    }


    /// <summary>
    /// Draws all collision detections in the Unity Editor.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Draws the right ceiling box
        Gizmos.DrawWireCube(
            ceilingBoxRight.position + new Vector3(
                (1 - stats.CeilingBoxSize + stats.CeilingBoxPosition) / 2f,
                0f,
                0f),
            new Vector2(stats.CeilingBoxSize - 0.05f, 0.1f)
        );

        // Draws the left ceiling box
        Gizmos.DrawWireCube(
            ceilingBoxLeft.position + new Vector3(
                -(1 - stats.CeilingBoxSize + stats.CeilingBoxPosition) / 2f,
                0f,
                0f),
            new Vector2(stats.CeilingBoxSize - 0.05f, 0.1f)
        );

        Gizmos.color = Color.red;

        // Draws the ground check box
        Gizmos.DrawWireCube(
            groundCheck.position,
            new Vector2(stats.HitboxBase, stats.HitboxHeight)
        );

        // Modded gizmos
        Gizmos.color = Color.blue;

        // Because capsules don't have a gizmo, this calls upon the
        // DrawCapsule() method to manually draw one.
        DrawCapsule(
            new Vector2(
                hurtBox.position.x + stats.CapsuleCenter.x,
                hurtBox.position.y + stats.CapsuleCenter.y
            ),
            stats.CapsuleSize,
            stats.CapsuleDirection
        );

        Gizmos.color = Color.cyan;

        // Draws the ray which detects if the player can snap to
        // a platform vertically (experimental)
        Gizmos.DrawLine(
            transform.position,
            transform.position + Vector3.up * 1f
        );
    }

    // Below are additional methods that aren't a part of the default script


    /// <summary>
    /// Sets isClimbing to true if the players is on a ladder
    /// and has a vertical input.
    /// </summary>
    private void CheckLadder()
    {
        if (isLadder && Mathf.Abs(yMovement) > 0)
        {
            isClimbing = true;
        }
    }


    /// <summary>
    /// Applys y movement to the player if isClimbing is true
    /// </summary>
    private void ClimbLadder()
    {
        if (isClimbing && !isJumping)
        {
            movement.y = stats.LadderClimbSpeed * yMovement;
        }
    }


    /// <summary>
    /// Reloads the scene if a player collides with a hazard or themself.
    /// Will create a better looking reload in the future.
    /// </summary>
    private void CheckHazards()
    {
        if (SpikeHit() || PlayerObliteration())
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    /// <summary>
    /// Handles if the player wants to fall through a one way platform.
    /// </summary>
    private void OneWayPlatform()
    {

        if (Input.GetAxisRaw("Vertical") < 0)
        {
            if (currentOneWay)
            {
                StartCoroutine(DisableCollision());
            }
        }

        if (xMovement != 0 && yMovement == 0 && !Input.GetKey(KeyCode.Space))
        {
            SnapToPlatform();
        }
    }


    /// <summary>
    /// Experimental.
    /// Finds the difference between the bottom of the player
    /// and the top of the platform, and teleports the player exactly
    /// to the top of the platform.
    /// </summary>
    private void SnapToPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.up,
            1f,
            groundLayer
        );

        // Runs if the raycast detects a collision with the platform
        if (hit.collider != null && hit.collider.CompareTag("OneWayDown"))
        {
            float platformTop = Mathf.Abs(hit.collider.bounds.min.y);
            float playerBottom = Mathf.Abs(playerCol.bounds.max.y);

            // Runs if the difference between the bottom of the player and the
            // top of the platform is within the threshold.
            if (platformTop < playerBottom &&
                 playerBottom - platformTop < stats.SnapThreshold)
            {
                // Snap player to the top of the platform
                transform.position = new Vector2(
                    transform.position.x,
                    transform.position.y - (playerBottom - platformTop)
                );
            }
        }
    }


    /// <summary>
    /// Handles the collision disabling for one way platforms.
    /// </summary>
    /// <returns>
    /// Waits until the player is through the platform before reenabling
    /// collisions. Moves the player back to the top of the platform if
    /// the raycast still detects a collision with the platform after the time
    /// has ended.
    /// Currently too lenient, also make it a Scriptable Stat.
    /// </returns>
    private IEnumerator DisableCollision()
    {
        var platformCol = currentOneWay.GetComponent<CompositeCollider2D>();

        ignoreGround = true;
        Physics2D.IgnoreCollision(playerCol, platformCol);
        yield return new WaitForSeconds(0.25f);
        ignoreGround = false;
        Physics2D.IgnoreCollision(playerCol, platformCol, false);
    }


    /// <summary>
    /// Checks to see if the players hurtbox is touching a hitbox spike or not
    /// </summary>
    /// <returns>True If the players hurtbox hits a spikes hitbox</returns>
    private bool SpikeHit() => Physics2D.OverlapCapsule(
        new Vector2(
            hurtBox.position.x + stats.CapsuleCenter.x,
            hurtBox.position.y + stats.CapsuleCenter.y
        ),
        stats.CapsuleSize,
        stats.CapsuleDirection,
        0,
        hazardLayer
    );


    /// <summary>
    /// Checks to see if players hurtboxes are colliding.
    /// </summary>
    /// <returns>True if players hurtboxes are colliding.</returns>
    private bool PlayerObliteration() => Physics2D.OverlapCapsule(
        new Vector2(
            hurtBox.position.x + stats.CapsuleCenter.x,
            hurtBox.position.y + stats.CapsuleCenter.y
        ),
        stats.CapsuleSize,
        stats.CapsuleDirection,
        0,
        otherPlayerLayer
    );


    /// <summary>
    /// Handles whether a players acceleration should be increasing
    /// after having hit ice or not.
    /// </summary>
    /// <returns>
    /// True if the player hits Ice or has not yet hit ordinary ground
    /// after having hit ice.
    /// </returns>
    private bool IsIce()
    {
        // Perform OverlapBoxAll to get all colliders in the box
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            groundCheck.position,
            new Vector2(stats.HitboxBase, stats.HitboxHeight),
            0f,
            groundLayer
        );

        // Iterate through the colliders and check if any of them are not
        // tagged as "OneWay"
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Ice"))
            {
                // If we find a valid collision that is not "OneWay",
                // return true
                return true;
            }
        }

        // If no valid collisions are found, return false
        return false;
    }


    /// <summary>
    /// Because there is no drawable gizmo for capsules, I have to manually
    /// create a capsule.
    /// </summary>
    /// <param name="center">
    /// Vector2 position of the capsule.
    /// </param>
    /// <param name="size">
    /// Size of the capsule
    /// </param>
    /// <param name="direction">
    /// Whether the capsule is Vertical or Horizontal
    /// </param>
    void DrawCapsule(
        Vector2 center,
        Vector2 size,
        CapsuleDirection2D direction
        )
    {
        float radius = size.x / 2f; // width determines the radius
        float height = size.y;      // height is used for the capsule height

        if (direction == CapsuleDirection2D.Vertical)
        {
            // Draw two half circles (top and bottom) for vertical capsule
            Vector2 topCircleCenter = new(
                center.x,
                center.y + (height / 2f - radius)
            );

            Vector2 bottomCircleCenter = new(
                center.x,
                center.y - (height / 2f - radius)
            );

            Gizmos.DrawWireSphere(topCircleCenter, radius);
            Gizmos.DrawWireSphere(bottomCircleCenter, radius);

            // Draw the connecting rectangle
            Gizmos.DrawLine(
                new(center.x - radius, topCircleCenter.y),
                new(center.x - radius, bottomCircleCenter.y)
            );

            Gizmos.DrawLine(
                new(center.x + radius, topCircleCenter.y),
                new(center.x + radius, bottomCircleCenter.y)
            );

        }
        else if (direction == CapsuleDirection2D.Horizontal)
        {
            // Draw two half circles (left and right) for horizontal capsule
            Vector2 leftCircleCenter = new(
                center.x - (height / 2f - radius),
                center.y
            );

            Vector2 rightCircleCenter = new(
                center.x + (height / 2f - radius),
                center.y
            );

            Gizmos.DrawWireSphere(leftCircleCenter, radius);
            Gizmos.DrawWireSphere(rightCircleCenter, radius);

            // Draw the connecting rectangle
            Gizmos.DrawLine(
                new(leftCircleCenter.x, center.y - radius),
                new(rightCircleCenter.x, center.y - radius)
            );

            Gizmos.DrawLine(
                new Vector2(leftCircleCenter.x, center.y + radius),
                new Vector2(rightCircleCenter.x, center.y + radius)
            );

        }
    }


    /// <summary>
    /// Handles player entering all trigger colliders with tags.
    /// </summary>
    /// <param name="collision">The Object's Collider</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Ladder":
                isLadder = true;
                break;
        }

    }


    /// <summary>
    /// Handles player exiting all trigger colliers with tags.
    /// </summary>
    /// <param name="collision">The Object's Collider</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Ladder":
                isLadder = false;
                isClimbing = false;
                break;
        }
    }

    /// <summary>
    /// Handles player entering all colliers with tags.
    /// </summary>
    /// <param name="collision">The Object's Collider</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayDown"))
        {
            currentOneWay = collision.gameObject;
        }
    }

    /// <summary>
    /// Handles player exiting all colliers with tags.
    /// </summary>
    /// <param name="collision">The Object's Collider</param>
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayDown"))
        {
            currentOneWay = null;
        }
    }
}

