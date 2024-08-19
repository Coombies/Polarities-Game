using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableStats : MonoBehaviour
{

    // Normal Lateral Movement
    [Tooltip("The standard speed the player travels.")]
    public float normalSpeed = 8f;

    [Tooltip("General ground deceleration")]
    public float normalGroundDeceleration = 120f;

    [Tooltip("The player has a longer deceleration when moving through the air")]
    public float normalAirDeceleration = 100f;

    [Tooltip("General ground acceleration")]
    public float normalGroundAcceleration = 140f;

    [Tooltip("The player has less control when moving through the air")]
    public float normalAirAcceleration = 120f;

    // Modified Lateral Movement
    [Tooltip("The fastest a player can generally move laterally without the influence of other external forces.")]
    public float sprintSpeed = 14f;

    [Tooltip("The player has takes longer to decelerate when sprinting")]
    public float sprintGroundDeceleration = 60f;

    [Tooltip("The player has takes much longer to decelerate when sprinting in the air")]
    public float sprintAirDeceleration = 50f;

    [Tooltip("The player has takes longer to accelerate when sprinting")]
    public float sprintGroundAcceleration = 70f;

    [Tooltip("The player has takes much longer to accelerate when sprinting in the air")]
    public float sprintAirAcceleration = 60f;

    // Jumping
    [Tooltip("The speed at which the player leaves the ground when jumping.")]
    public float jumpForce = 22f;

    [Tooltip("The speed at which the player accelerates towards the ground.")]
    public float gravityAcceleration = 90f;

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
    public float hitboxBase = 0.5f;

    [Tooltip("How much of the maximum height the player jumps when letting go")]
    public float graceGravityModifier = 0.5f;

    [Tooltip("How much of the maximum height the player jumps when letting go")]
    public float verticalSpeedApexThreshold = 1f;

    // Maximum Speeds the Player May Fall
    [Tooltip("The modified terminal velocity of the player")]
    public float fastFallSpeed = 35f;

    [Tooltip("The rate of change between slow and fast fall")]
    public float fastFallAcceleration = 220f;

    [Tooltip("The highest at which the player may move upward before being able to initiate a fast fall")]
    public float fastFallActuationSpeed = 3f;

    [Tooltip("The general terminal velocity of the player")]
    public float slowFallSpeed = 18f;


    // Ceiling Control
    [Tooltip("The speed at which a player is forced off a ceiling")]
    public float ceilingBounce = 5f;

    [Tooltip("The speed at which a player is forced off a ceiling")]
    public float ceilingBoxSize = 0.8f;

    [Tooltip("The speed at which a player is forced off a ceiling")]
    public float clipForce = 0.8f;
}
