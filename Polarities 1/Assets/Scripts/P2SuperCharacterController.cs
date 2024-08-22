using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SuperCharacterController2 : MonoBehaviour
{
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

    private Vector2 movement;
    private Collider2D objectCol;

    [Header("Default Object References"), SerializeField] private Transform groundCheck;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private Transform ceilingBoxRight;
    [SerializeField] private Transform ceilingBoxLeft;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D playerCol;
    [SerializeField] private BoxCollider2D otherPlayerCol;
    [SerializeField] private GameObject objectWithCompositeCollider;
    [SerializeField] private ScriptableStats stats;

    // modded variables
    private bool isLadder;
    private bool isClimbing;

    private void Start()
    {
        // links class Scriptable Stats, filled with all constant variables for convenience
        objectCol = objectWithCompositeCollider.GetComponent<CompositeCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(playerCol, otherPlayerCol, true);
    }

    private void Update()
    {
        // calls methods that require inputs
        GetInput();
        CheckJumping();
        Flip();

        // calls modded methods that require inputs
        CheckLadder();
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
    }

    // applies x and y velocity to rigidbody
    private void ApplyMovement() => rb.velocity = -movement;

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
        if (Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(movement.x) >= stats.normalSpeed)
        {

            groundAcceleration = stats.sprintGroundAcceleration;
            groundDeceleration = stats.sprintGroundDeceleration;
            airAcceleration = stats.sprintAirAcceleration;
            airDeceleration = stats.sprintAirDeceleration;

            moveSpeed = stats.sprintSpeed;
        }
        else
        {
            groundAcceleration = stats.normalGroundAcceleration;
            groundDeceleration = stats.normalGroundDeceleration;
            airAcceleration = stats.normalAirAcceleration;
            airDeceleration = stats.normalAirDeceleration;

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
        if (IsGrounded())
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
        }

        if (Input.GetKeyUp(KeyCode.Space) && movement.y > 0)
        {
            movement.y *= stats.jumpHeightModifier;
            coyoteJump = 0f;
        }

    }

    // returns true if the GroundCheck object is colliding with the Ground layer
    private bool IsGrounded() => Physics2D.OverlapBox(groundCheck.position, new Vector2(stats.hitboxBase, stats.hitboxHeight), 0f, groundLayer);
    private bool TouchingCeiling() => Physics2D.OverlapBox(ceilingCheck.position, new Vector2(stats.hitboxBase, stats.hitboxHeight), 0f, groundLayer);

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

        if (IsGrounded() && movement.y <= 0f)
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
        bool leftCheck = Physics2D.OverlapBox(ceilingBoxLeft.position + new Vector3(-(1 - stats.ceilingBoxSize + stats.ceilingBoxPosition) / 2f, 0f, 0f), new Vector2(stats.ceilingBoxSize - 0.05f, 0.1f), 0f, groundLayer);
        bool rightCheck = Physics2D.OverlapBox(ceilingBoxRight.position + new Vector3((1 - stats.ceilingBoxSize + stats.ceilingBoxPosition) / 2f, 0f, 0f), new Vector2(stats.ceilingBoxSize - 0.05f, 0.1f), 0f, groundLayer);


        if (rightCheck && !leftCheck)
        {
            Physics2D.IgnoreCollision(playerCol, objectCol, true);
            rb.AddForce(Vector2.right * -(stats.clipForce + movement.x), ForceMode2D.Impulse);
        }
        else if (leftCheck && !rightCheck)
        {
            Physics2D.IgnoreCollision(playerCol, objectCol, true);
            rb.AddForce(Vector2.right * (stats.clipForce + movement.x), ForceMode2D.Impulse);
        }
        else if (TouchingCeiling())
        {
            Physics2D.IgnoreCollision(playerCol, objectCol, false);
            movement.y = -stats.ceilingBounce;
        }
        else
        {
            Physics2D.IgnoreCollision(playerCol, objectCol, false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(ceilingBoxRight.position + new Vector3((1 - stats.ceilingBoxSize + stats.ceilingBoxPosition) / 2f, 0f, 0f), new Vector2(stats.ceilingBoxSize - 0.05f, 0.1f));
        Gizmos.DrawWireCube(ceilingBoxLeft.position + new Vector3(-(1 - stats.ceilingBoxSize + stats.ceilingBoxPosition) / 2f, 0f, 0f), new Vector2(stats.ceilingBoxSize - 0.05f, 0.1f));

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, new Vector2(stats.hitboxBase, stats.hitboxHeight));
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
        if (isClimbing)
        {
            movement.y = stats.ladderClimbSpeed * yMovement;
        }
    }

    private void OnTriggerEnter2D(Collider2D ladderCollision)
    {
        if (ladderCollision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D ladderCollision)
    {
        if (ladderCollision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
        }
    }
}

