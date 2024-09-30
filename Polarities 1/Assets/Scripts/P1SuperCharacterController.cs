using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SuperCharacterController1 : MonoBehaviour
{
    // base variables
    private float xMovement;
    private float yMovement;
    private float previousYVelocity;
    private float previousXVelocity;
    private float coyoteJump;
    private float gravityModifier;
    private float bufferJump;
    private float groundAcceleration;
    private float airAcceleration;
    private float groundDeceleration;
    private float airDeceleration;
    private float moveSpeed;

    private bool isFacingRight;
    private bool isJumping;

    private Vector2 movement;

    private List<CompositeCollider2D> groundColliders = new List<CompositeCollider2D>();

    [Header("Default Object References"), SerializeField] private Transform groundCheck;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private Transform ceilingBoxRight;
    [SerializeField] private Transform ceilingBoxLeft;

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
    private bool isIce;
    private float accelerationModifier = 1;

    // One Way Platform Variables
    private GameObject currentOneWay;
    private bool ignoreGround;

    [SerializeField] private Transform hurtBox;
    [SerializeField] private LayerMask hazardLayer;
    [SerializeField] private LayerMask otherPlayerLayer;

    private void Start()
    {
        // links class Scriptable Stats, filled with all constant variables for convenience
        GameObject[] groundObjects = GameObject.FindGameObjectsWithTag("Ground");
        foreach (GameObject obj in groundObjects)
        {
            CompositeCollider2D objectCol = obj.GetComponent<CompositeCollider2D>();
            if (objectCol != null)
            {
                groundColliders.Add(objectCol);
            }
        }

        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(playerCol, otherPlayerCol);

        GameObject oneWayObject = GameObject.FindGameObjectWithTag("OneWayDown");
        CompositeCollider2D oneWayCol = oneWayObject.GetComponent<CompositeCollider2D>();
        Physics2D.IgnoreCollision(playerCol, oneWayCol);
    }

    private void Update()
    {
        // calls methods that require inputs
        GetInput();
        CheckJumping();
        Flip();

        // calls modded methods that require inputs
        CheckLadder();
        OneWayPlatform();
    }

    private void FixedUpdate()
    {
        // calls methods that apply movement change
        PreviousVariables();
        ApplyMovement();
        ApplyGravity();
        HandleXMovement();
        EdgeHandling();

        // calls modded methods that apply movement change
        ClimbLadder();
        CheckHazards();
    }

    // applies x and y velocity to rigidbody
    private void ApplyMovement() => rb.velocity = movement;

    private void PreviousVariables()
    {
        previousYVelocity = movement.y;
        previousXVelocity = movement.x;
    }

    private void GetInput()
    {
        // Movement returns -1, 0, or 1
        xMovement = Input.GetAxisRaw("Horizontal");
        yMovement = Input.GetAxisRaw("Vertical");

        // There is air acceleration/deceleration, and a different speed for sprint and walk for
        // maximum control over the platformers movement
        if (IsIce())
        {
            accelerationModifier = stats.iceAccelerationModifier;
        }
        else if (IsGrounded())
        {
            accelerationModifier = 1;
        }

        if (Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(movement.x) >= stats.normalSpeed)
        {

            groundAcceleration = stats.sprintGroundAcceleration * accelerationModifier;
            groundDeceleration = stats.sprintGroundDeceleration * accelerationModifier;
            airAcceleration = stats.sprintAirAcceleration * accelerationModifier;
            airDeceleration = stats.sprintAirDeceleration * accelerationModifier;

            moveSpeed = stats.sprintSpeed;
        }
        else
        {
            groundAcceleration = stats.normalGroundAcceleration * accelerationModifier;
            groundDeceleration = stats.normalGroundDeceleration * accelerationModifier;
            airAcceleration = stats.normalAirAcceleration * accelerationModifier;
            airDeceleration = stats.normalAirDeceleration * accelerationModifier;

            moveSpeed = stats.normalSpeed;
        }
    }

    private void HandleXMovement()
    {
        if (xMovement == 0)
        {
            // If grounded, apply ground deceleration, otherwise apply air deceleration
            var deceleration = IsGrounded() ? groundDeceleration : airDeceleration;
            movement.x = Mathf.MoveTowards(movement.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            // If grounded, apply ground acceleration, otherwise apply air acceleration
            var acceleration = IsGrounded() ? groundAcceleration : airAcceleration;
            movement.x = Mathf.MoveTowards(movement.x, xMovement * moveSpeed, acceleration * Time.fixedDeltaTime);

        }
    }

    private void CheckJumping()
    {
        // Allows player to perform a jump for a short time after falling off a platform
        if (IsGrounded() || isClimbing)
        {
            coyoteJump = stats.coyoteTime;
        }
        else
        {
            coyoteJump -= Time.deltaTime;
        }

        // Allows player to input jump a moment before they are grounded
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bufferJump = stats.jumpBufferTime;
            isClimbing = false;
        }
        else
        {
            bufferJump -= Time.deltaTime;
        }

        // Applies jump force if conditions are met
        if (coyoteJump > 0f && bufferJump > 0f)
        {
            movement.y = stats.jumpForce;
            bufferJump = 0;
            isJumping = true;
        }

        if (Input.GetKeyUp(KeyCode.Space) && movement.y > 0)
        {
            movement.y *= stats.jumpHeightModifier;
            coyoteJump = 0f;
        }

        if (movement.y <= 0)
        {
            isJumping = false;
        }

    }

    // returns true if the GroundCheck object is colliding with the Ground layer
    private bool IsGrounded()
    {
        // Perform OverlapBoxAll to get all colliders in the box
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            groundCheck.position,
            new Vector2(stats.hitboxBase, stats.hitboxHeight),
            0f,
            groundLayer
        );

        // Iterate through the colliders and check if any of them are not tagged as "OneWay"
        foreach (var collider in colliders)
        {
            if (!collider.CompareTag("OneWayDown"))
            {
                // If we find a valid collision that is not "OneWay", return true
                return true;
            }
        }

        // If no valid collisions are found, return false
        return false;
    }
    private bool TouchingCeiling()
    {
        // Perform OverlapBoxAll to get all colliders in the box
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            ceilingCheck.position,
            new Vector2(stats.hitboxBase, stats.hitboxHeight),
            0f,
            groundLayer
        );

        // Iterate through the colliders and check if any of them are not tagged as "OneWay"
        foreach (var collider in colliders)
        {
            if (!collider.CompareTag("OneWayUp"))
            {
                // If we find a valid collision that is not "OneWay", return true
                return true;
            }
        }

        // If no valid collisions are found, return false
        return false;
    }

    private void Flip()
    {
        if (isFacingRight && xMovement < 0f || !isFacingRight && xMovement > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void ApplyGravity()
    {

        if (!IsGrounded() && Mathf.Abs(movement.y) < stats.verticalSpeedApexThreshold)
        {
            gravityModifier = stats.graceGravityModifier;
        }
        else
        {
            gravityModifier = 1;
        }

        if (IsGrounded() && !ignoreGround && movement.y <= 0f)
        {
            movement.y = stats.weightForce;
        }
        else if (Input.GetAxisRaw("Vertical") < 0 && movement.y <= stats.fastFallActuationSpeed)
        {
            movement.y = Mathf.MoveTowards(movement.y, -stats.fastFallSpeed, stats.fastFallAcceleration * gravityModifier * Time.deltaTime);
        }
        else
        {
            movement.y = Mathf.MoveTowards(movement.y, -stats.slowFallSpeed, stats.gravityAcceleration * gravityModifier * Time.deltaTime);
        }

    }

    private void EdgeHandling()
    {

        bool leftCheck = false;
        bool rightCheck = false;

        // First, detect all overlapping colliders
        Collider2D[] collidersLeft = Physics2D.OverlapBoxAll(
            ceilingBoxLeft.position + new Vector3(-(1 - stats.ceilingBoxSize + stats.ceilingBoxPosition) / 2f, 0f, 0f),
            new Vector2(stats.ceilingBoxSize - 0.05f, 0.1f),
            0f,
            groundLayer
        );

        Collider2D[] collidersRight = Physics2D.OverlapBoxAll(
            ceilingBoxLeft.position + new Vector3((1 - stats.ceilingBoxSize + stats.ceilingBoxPosition) / 2f, 0f, 0f),
            new Vector2(stats.ceilingBoxSize - 0.05f, 0.1f),
            0f,
            groundLayer
        );

        // Now, filter out the ones tagged as "OneWay"
        foreach (var collider in collidersLeft)
        {
            if (!collider.CompareTag("OneWayUp"))
            {
                // If the collider is not tagged as "OneWay", it's a valid collision
                leftCheck = true;
                break;
            }
        }

        foreach (var collider in collidersRight)
        {
            if (!collider.CompareTag("OneWayUp"))
            {
                // If the collider is not tagged as "OneWay", it's a valid collision
                rightCheck = true;
                break;
            }
        }

        foreach (Collider2D objectCol in groundColliders)
        {

            if (rightCheck && !leftCheck)
            {
                Physics2D.IgnoreCollision(playerCol, objectCol, true);
                rb.AddForce(Vector2.left * (stats.clipForce + Mathf.Abs(xMovement)), ForceMode2D.Impulse);
            }
            else if (leftCheck && !rightCheck)
            {
                Physics2D.IgnoreCollision(playerCol, objectCol, true);
                rb.AddForce(Vector2.right * (stats.clipForce + Mathf.Abs(xMovement)), ForceMode2D.Impulse);
            }
            else if (TouchingCeiling())
            {
                Physics2D.IgnoreCollision(playerCol, objectCol, false);
                movement.y = Mathf.Abs(movement.y) * -stats.ceilingBounce;
            }
            else
            {
                Physics2D.IgnoreCollision(playerCol, objectCol, false);
            }
        }



    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(ceilingBoxRight.position + new Vector3((1 - stats.ceilingBoxSize + stats.ceilingBoxPosition) / 2f, 0f, 0f), new Vector2(stats.ceilingBoxSize - 0.05f, 0.1f));
        Gizmos.DrawWireCube(ceilingBoxLeft.position + new Vector3(-(1 - stats.ceilingBoxSize + stats.ceilingBoxPosition) / 2f, 0f, 0f), new Vector2(stats.ceilingBoxSize - 0.05f, 0.1f));

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, new Vector2(stats.hitboxBase, stats.hitboxHeight));

        // modded gizmos
        Gizmos.color = Color.blue;
        DrawCapsule(new Vector2(hurtBox.position.x + stats.capsuleCenter.x, hurtBox.position.y + stats.capsuleCenter.y), stats.capsuleSize, stats.capsuleDirection);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1f);
    }

    // Below are additional methods that aren't a part of the default script

    private void CheckLadder()
    {
        if (isLadder && Mathf.Abs(yMovement) > 0)
        {
            isClimbing = true;
        }
    }
    private void ClimbLadder()
    {
        if (isClimbing && !isJumping)
        {
            movement.y = stats.ladderClimbSpeed * yMovement;
        }
    }

    private void CheckHazards()
    {
        if (SpikeHit() || PlayerObliteration())
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

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

    private void SnapToPlatform()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);

        if (hit.collider != null && hit.collider.CompareTag("OneWayUp"))
        {
            float platformTop = Mathf.Abs(hit.collider.bounds.max.y);
            float playerBottom = Mathf.Abs(playerCol.bounds.min.y);

            if (platformTop < playerBottom && playerBottom - platformTop < stats.snapThreshold)
            {
                // Snap player to the top of the platform
                transform.position = new Vector2(transform.position.x, transform.position.y + playerBottom - platformTop);
            }
        }
    }

    private IEnumerator DisableCollision()
    {
        CompositeCollider2D platformCollider = currentOneWay.GetComponent<CompositeCollider2D>();

        ignoreGround = true;
        Physics2D.IgnoreCollision(playerCol, platformCollider);
        yield return new WaitForSeconds(0.25f);
        ignoreGround = false;
        Physics2D.IgnoreCollision(playerCol, platformCollider, false);

    }

    private bool SpikeHit() => Physics2D.OverlapCapsule(new Vector2(hurtBox.position.x + stats.capsuleCenter.x, hurtBox.position.y + stats.capsuleCenter.y), stats.capsuleSize, stats.capsuleDirection, 0, hazardLayer);

    private bool PlayerObliteration() => Physics2D.OverlapCapsule(new Vector2(hurtBox.position.x + stats.capsuleCenter.x, hurtBox.position.y + stats.capsuleCenter.y), stats.capsuleSize, stats.capsuleDirection, 0, otherPlayerLayer);

    private bool IsIce()
    {
        // Perform OverlapBoxAll to get all colliders in the box
        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            groundCheck.position,
            new Vector2(stats.hitboxBase, stats.hitboxHeight),
            0f,
            groundLayer
        );

        // Iterate through the colliders and check if any of them are not tagged as "OneWay"
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Ice"))
            {
                // If we find a valid collision that is not "OneWay", return true
                return true;
            }
        }

        // If no valid collisions are found, return false
        return false;
    }

    void DrawCapsule(Vector2 center, Vector2 size, CapsuleDirection2D direction)
    {
        float radius = size.x / 2f; // width determines the radius
        float height = size.y;      // height is used for the capsule height

        if (direction == CapsuleDirection2D.Vertical)
        {
            // Draw two half circles (top and bottom) for vertical capsule
            Vector2 topCircleCenter = new Vector2(center.x, center.y + (height / 2f - radius));
            Vector2 bottomCircleCenter = new Vector2(center.x, center.y - (height / 2f - radius));
            Gizmos.DrawWireSphere(topCircleCenter, radius);
            Gizmos.DrawWireSphere(bottomCircleCenter, radius);

            // Draw the connecting rectangle
            Gizmos.DrawLine(new Vector2(center.x - radius, topCircleCenter.y), new Vector2(center.x - radius, bottomCircleCenter.y));
            Gizmos.DrawLine(new Vector2(center.x + radius, topCircleCenter.y), new Vector2(center.x + radius, bottomCircleCenter.y));
        }
        else if (direction == CapsuleDirection2D.Horizontal)
        {
            // Draw two half circles (left and right) for horizontal capsule
            Vector2 leftCircleCenter = new Vector2(center.x - (height / 2f - radius), center.y);
            Vector2 rightCircleCenter = new Vector2(center.x + (height / 2f - radius), center.y);
            Gizmos.DrawWireSphere(leftCircleCenter, radius);
            Gizmos.DrawWireSphere(rightCircleCenter, radius);

            // Draw the connecting rectangle
            Gizmos.DrawLine(new Vector2(leftCircleCenter.x, center.y - radius), new Vector2(rightCircleCenter.x, center.y - radius));
            Gizmos.DrawLine(new Vector2(leftCircleCenter.x, center.y + radius), new Vector2(rightCircleCenter.x, center.y + radius));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Ladder":
                isLadder = true;
                break;

            case "Ice":
                isIce = true;
                break;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Ladder":
                isLadder = false;
                isClimbing = false;
                break;

            case "Ice":
                isIce = false;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayUp"))
        {
            currentOneWay = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayUp"))
        {
            currentOneWay = null;
        }
    }
}

