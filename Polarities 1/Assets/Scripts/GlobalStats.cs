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
    public float normalGroundDeceleration = 80f;

    [Tooltip("The player has a longer deceleration when moving through the air")]
    public float normalAirDeceleration = 50f;

    [Tooltip("General ground acceleration")]
    public float normalGroundAcceleration = 100f;

    [Tooltip("The player has less control when moving through the air")]
    public float normalAirAcceleration = 70f;

    // Modified Lateral Movement
    [Header("Modified Lateral Movement"), Tooltip("The fastest a player can generally move laterally without the influence of other external forces."),
        ContextMenuItem("Set Default Sprint Settings", "SprintDefaultModifier")]
    public float sprintSpeed = 7f;

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
    public float jumpForce = 15.5f;

    [Tooltip("The speed at which the player accelerates towards the ground.")]
    public float gravityAcceleration = 65f;

    [Tooltip("The weight of the player")]
    public float weightForce = -1.5f;

    [Tooltip("The amount of time a player may jump for after leaving a platform")]
    public float coyoteTime = 0.08f;

    [Tooltip("The amount of time a player may jump for before becoming grounded")]
    public float jumpBufferTime = 0.08f;

    [Tooltip("How much of the maximum height the player jumps when letting go")]
    public float jumpHeightModifier = 0.4f;

    [Tooltip("How much of the maximum height the player jumps when letting go")]
    public float hitboxHeight = 0.1f;

    [Tooltip("How much of the maximum height the player jumps when letting go")]
    public float hitboxBase = 0.4f;

    [Tooltip("How much of the maximum height the player jumps when letting go")]
    public float graceGravityModifier = 0.4f;

    [Tooltip("How much of the maximum height the player jumps when letting go")]
    public float verticalSpeedApexThreshold = 2.5f;

    // Maximum Speeds the Player May Fall
    [Header("Fall Speeds"), Tooltip("The modified terminal velocity of the player")]
    public float fastFallSpeed = 24f;

    [Tooltip("The rate of change between slow and fast fall")]
    public float fastFallAcceleration = 150f;

    [Tooltip("The highest at which the player may move upward before being able to initiate a fast fall")]
    public float fastFallActuationSpeed = 3f;

    [Tooltip("The general terminal velocity of the player")]
    public float slowFallSpeed = 17f;


    // Ceiling Control
    [Header("Ceiling Control"), Tooltip("The speed at which a player is forced off a ceiling")]
    public float ceilingBounce = 5f;

    [Range(0.0f, 1f), Tooltip("The x width of the ceiling hitboxes")]
    public float ceilingBoxSize = 0.35f;

    [Range(-1f, 1f), Tooltip("The absolute x position of the ceiling hitboxes")]
    public float ceilingBoxPosition = -0.5f;

    [Tooltip("The speed at which a player forced to the side of a ceiling")]
    public float clipForce = 1f;

    [Space(50), Header("                                MODDED STATS")]


    [Header("Ladders")]
    public float ladderClimbSpeed = 5f;


    // Inspector Steps
    [Space(30), Header("Inspector Steps"), Tooltip("Rounds slider changes to nearest .01")]
    public float step1 = 0.01f;

    private void OnValidate()
    {
        ceilingBoxSize = Mathf.Round(ceilingBoxSize / step1) * step1;
        ceilingBoxPosition = Mathf.Round(ceilingBoxPosition / step1) * step1;
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
