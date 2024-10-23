using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Holds all variables for character-related movement
/// </summary>
[CreateAssetMenu(fileName = "GlobalStats", menuName = "Stats/GlobalStats")]
public class ScriptableStats : MonoBehaviour
{

    // Normal Lateral Movement
    [Header("Normal Lateral Movement"), Tooltip("The standard speed the player travels.")]
    public float NormalSpeed = 5f;

    [Tooltip("General ground deceleration")]
    public float NormalGroundDeceleration = 40f;

    [Tooltip("The player has a longer deceleration when moving through the air")]
    public float NormalAirDeceleration = 30f;

    [Tooltip("General ground acceleration")]
    public float NormalGroundAcceleration = 60f;

    [Tooltip("The player has less control when moving through the air")]
    public float NormalAirAcceleration = 50f;


    // Modified Lateral Movement
    [Header("Modified Lateral Movement"), Tooltip("The fastest a player can generally move laterally without the influence of other external forces."),
        ContextMenuItem("Set Default Sprint Settings", "SprintDefaultModifier")]
    public float SprintSpeed = 5f;

    [Tooltip("The player has takes longer to decelerate when sprinting.")]
    public float SprintGroundDeceleration = 30f;

    [Tooltip("The player has takes much longer to decelerate when sprinting in the air")]
    public float SprintAirDeceleration = 20f;

    [Tooltip("The player has takes longer to accelerate when sprinting")]
    public float SprintGroundAcceleration = 40f;

    [Tooltip("The player has takes much longer to accelerate when sprinting in the air")]
    public float SprintAirAcceleration = 30f;


    // Jumping
    [Header("Jumping"), Tooltip("The speed at which the player leaves the ground when jumping.")]
    public float JumpForce = 17f;

    [Tooltip("The speed at which the player accelerates towards the ground.")]
    public float GravityAcceleration = 75f;

    [Tooltip("The weight of the player")]
    public float WeightForce = -1.5f;

    [Tooltip("The amount of time a player may jump for after leaving a platform")]
    public float CoyoteTime = 0.08f;

    [Tooltip("The amount of time a player may jump for before becoming grounded")]
    public float JumpBufferTime = 0.08f;

    [Tooltip("How much of the maximum height the player jumps when letting go")]
    public float JumpHeightModifier = 0.4f;

    [Tooltip("Height of the groundCheck hitbox"), Range(0f, 1f)]
    public float HitboxHeight = 0.1f;

    [Tooltip("Base length of the groundCheck hitbox"), Range(0f, 1f)]
    public float HitboxBase = 0.4f;

    [Tooltip("What the gravity is multiplied by when past the jump apex threshold")]
    public float GraceGravityModifier = 0.4f;

    [Tooltip("The speed threshold at which the gravity becomes lower")]
    public float VerticalSpeedApexThreshold = 3f;

    [Tooltip("The lowest y speed before the player may perform a smaller jump")]
    public float MinJumpHeightThreshold = 14f;


    // Maximum Speeds the Player May Fall
    [Header("Fall Speeds"), Tooltip("The modified terminal velocity of the player")]
    public float FastFallSpeed = 17f;

    [Tooltip("The rate of change between slow and fast fall")]
    public float FastFallAcceleration = 75f;

    [Tooltip("The highest at which the player may move upward before being able to initiate a fast fall")]
    public float FastFallActuationSpeed = 3f;

    [Tooltip("The general terminal velocity of the player")]
    public float SlowFallSpeed = 17f;


    // Ceiling Control
    [Header("Ceiling Control"), Tooltip("The speed at which a player is forced off a ceiling"), Range(0.75f, 1f)]
    public float CeilingBounce = 0.9f;

    [Range(0.0f, 1f), Tooltip("The x width of the ceiling hitboxes")]
    public float CeilingBoxSize = 0.25f;

    [Range(-1f, 1f), Tooltip("The absolute x position of the ceiling hitboxes")]
    public float CeilingBoxPosition = -0.5f;

    [Tooltip("The speed at which a player forced to the side of a ceiling")]
    public float ClipForce = 1.5f;

    [Space(50), Header("                                MODDED STATS")]

    // Ladder Variables
    [Header("Ladders"), Tooltip("The speed at which the player climbs the ladder")]
    public float LadderClimbSpeed = 5f;


    // Player Hurtbox Control
    [Header("Hurtbox Control"), Tooltip("Centre of the capsule")]
    public Vector2 CapsuleCenter = new Vector2(0f, 0f);

    [Tooltip("The radius of the capsule circles (x) and distance between them (y)")]
    public Vector2 CapsuleSize = new Vector2(0.5f, 1.375f);

    [Tooltip("The direction of the capsule")]
    public CapsuleDirection2D CapsuleDirection = CapsuleDirection2D.Vertical;


    // Ice Acceleration
    [Header("Ice"), Tooltip("Acceleration modifier when on ice")]
    public float IceAccelerationModifier = 0.05f;

    [Tooltip("The rate at which the player regains their ordinary acceleration after having been on ice"), Range(0, 0.01f)]
    public float DefrostRate = 0.001f;


    // Platforms
    [Header("Platforms"), Tooltip("The point at which the player snaps to a platform"), Range(0, 1)]
    public float SnapThreshold = 1f;


    // Inspector Steps
    [Space(30), Header("Inspector Steps"), Tooltip("Rounds slider changes to nearest .01")]
    public float Step1 = 0.01f;


    /// <summary>
    /// Rounds certain variables to step size.
    /// </summary>
    private void OnValidate()
    {
        CeilingBoxSize = Mathf.Round(CeilingBoxSize / Step1) * Step1;
        CeilingBoxPosition = Mathf.Round(CeilingBoxPosition / Step1) * Step1;
        HitboxHeight = Mathf.Round(HitboxHeight / Step1) * Step1;
        HitboxBase = Mathf.Round(HitboxBase / Step1) * Step1;
        SnapThreshold = Mathf.Round(SnapThreshold / Step1) * Step1;

    }


    /// <summary>
    /// Default Sprint Stats.
    /// </summary>
    private void SprintDefaultModifier()
    {
        SprintSpeed = NormalSpeed * 1.5f;
        SprintGroundAcceleration = NormalGroundAcceleration * 0.5f;
        SprintGroundDeceleration = NormalGroundDeceleration * 0.5f;
        SprintAirAcceleration = NormalAirAcceleration * 0.5f;
        SprintAirDeceleration = NormalAirDeceleration * 0.5f;
    }
}
