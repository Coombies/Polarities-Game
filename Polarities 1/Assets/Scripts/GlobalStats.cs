using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalStats", menuName = "Stats/GlobalStats")]
public class ScriptableStats : MonoBehaviour
{

    // Normal Lateral Movement
    [Header("Normal Lateral Movement"), Tooltip("The standard speed the player travels.")]
    public float normalSpeed = 5f;

    [Tooltip("General ground deceleration")]
    public float normalGroundDeceleration = 40f;

    [Tooltip("The player has a longer deceleration when moving through the air")]
    public float normalAirDeceleration = 30f;

    [Tooltip("General ground acceleration")]
    public float normalGroundAcceleration = 60f;

    [Tooltip("The player has less control when moving through the air")]
    public float normalAirAcceleration = 50f;

    // Modified Lateral Movement
    [Header("Modified Lateral Movement"), Tooltip("The fastest a player can generally move laterally without the influence of other external forces."),
        ContextMenuItem("Set Default Sprint Settings", "SprintDefaultModifier")]
    public float sprintSpeed = 5f;

    [Tooltip("The player has takes longer to decelerate when sprinting.")]
    public float sprintGroundDeceleration = 30f;

    [Tooltip("The player has takes much longer to decelerate when sprinting in the air")]
    public float sprintAirDeceleration = 20f;

    [Tooltip("The player has takes longer to accelerate when sprinting")]
    public float sprintGroundAcceleration = 40f;

    [Tooltip("The player has takes much longer to accelerate when sprinting in the air")]
    public float sprintAirAcceleration = 30f;

    // Jumping
    [Header("Jumping"), Tooltip("The speed at which the player leaves the ground when jumping.")]
    public float jumpForce = 17f;

    [Tooltip("The speed at which the player accelerates towards the ground.")]
    public float gravityAcceleration = 75f;

    [Tooltip("The weight of the player")]
    public float weightForce = -1.5f;

    [Tooltip("The amount of time a player may jump for after leaving a platform")]
    public float coyoteTime = 0.08f;

    [Tooltip("The amount of time a player may jump for before becoming grounded")]
    public float jumpBufferTime = 0.08f;

    [Tooltip("How much of the maximum height the player jumps when letting go")]
    public float jumpHeightModifier = 0.4f;

    [Tooltip("How much of the maximum height the player jumps when letting go"), Range(0f, 1f)]
    public float hitboxHeight = 0.1f;

    [Tooltip("How much of the maximum height the player jumps when letting go"), Range(0f, 1f)]
    public float hitboxBase = 0.4f;

    [Tooltip("How much of the maximum height the player jumps when letting go")]
    public float graceGravityModifier = 0.4f;

    [Tooltip("How much of the maximum height the player jumps when letting go")]
    public float verticalSpeedApexThreshold = 3f;

    // Maximum Speeds the Player May Fall
    [Header("Fall Speeds"), Tooltip("The modified terminal velocity of the player")]
    public float fastFallSpeed = 17f;

    [Tooltip("The rate of change between slow and fast fall")]
    public float fastFallAcceleration = 75f;

    [Tooltip("The highest at which the player may move upward before being able to initiate a fast fall")]
    public float fastFallActuationSpeed = 3f;

    [Tooltip("The general terminal velocity of the player")]
    public float slowFallSpeed = 17f;


    // Ceiling Control
    [Header("Ceiling Control"), Tooltip("The speed at which a player is forced off a ceiling"), Range(0.75f, 1f)]
    public float ceilingBounce = 0.9f;

    [Range(0.0f, 1f), Tooltip("The x width of the ceiling hitboxes")]
    public float ceilingBoxSize = 0.25f;

    [Range(-1f, 1f), Tooltip("The absolute x position of the ceiling hitboxes")]
    public float ceilingBoxPosition = -0.5f;

    [Tooltip("The speed at which a player forced to the side of a ceiling")]
    public float clipForce = 1.5f;

    [Space(50), Header("                                MODDED STATS")]

    // Ladder Variables
    [Header("Ladders"), Tooltip("The speed at which the player climbs the ladder")]
    public float ladderClimbSpeed = 5f;

    // Player Hurtbox Control
    [Header("Hurtbox Control"), Tooltip("Centre of the capsule")]
    public Vector2 capsuleCenter = new Vector2(0f, 0f);

    [Tooltip("The radius of the capsule circles (x) and distance between them (y)")]
    public Vector2 capsuleSize = new Vector2(0.5f, 1.375f);

    [Tooltip("The direction of the capsule")]
    public CapsuleDirection2D capsuleDirection = CapsuleDirection2D.Vertical;


    // Ice Acceleration
    [Header("Ice"), Tooltip("Acceleration modifier when on ice")]
    public float iceAccelerationModifier = 0.05f;

    [Tooltip("The rate at which the player regains their ordinary acceleration after having been on ice"), Range(0, 0.01f)]
    public float defrostRate = 0.001f;


    // Platforms
    [Header("Platforms"), Tooltip("The point at which the player snaps to a platform"), Range(0, 1)]
    public float snapThreshold = 1f;


    // Inspector Steps
    [Space(30), Header("Inspector Steps"), Tooltip("Rounds slider changes to nearest .01")]
    public float step1 = 0.01f;

    private void OnValidate()
    {
        ceilingBoxSize = Mathf.Round(ceilingBoxSize / step1) * step1;
        ceilingBoxPosition = Mathf.Round(ceilingBoxPosition / step1) * step1;
        hitboxHeight = Mathf.Round(hitboxHeight / step1) * step1;
        hitboxBase = Mathf.Round(hitboxBase / step1) * step1;
        snapThreshold = Mathf.Round(snapThreshold / step1) * step1;

    }

    private void SprintDefaultModifier()
    {
        sprintSpeed = normalSpeed * 1.5f;
        sprintGroundAcceleration = normalGroundAcceleration * 0.5f;
        sprintGroundDeceleration = normalGroundDeceleration * 0.5f;
        sprintAirAcceleration = normalAirAcceleration * 0.5f;
        sprintAirDeceleration = normalAirDeceleration * 0.5f;
    }
}
