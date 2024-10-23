using UnityEngine;
using System.Collections;


/// <summary>
/// Controls the movement of moving objects.
/// </summary>
public class ObjectMovement : MonoBehaviour
{
    [Tooltip("Maximum speed of the moving object")]
    [SerializeField] private float maxSpeed = 50f;

    [Tooltip("Acceleration adn Deceleration of the moving object")]
    [SerializeField] private float acceleration = 30f;

    [Tooltip("Distance for the moving object to travel")]
    [SerializeField] private float targetDistance = 3.5f;

    [Tooltip("Time taken at rest before moving again")]
    [SerializeField] private float pauseDuration = 0.2f;

    [Tooltip("Direction the moving object travels")]
    [SerializeField] private Vector3 direction = Vector3.down;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float currentSpeed = 0f;
    private bool movingToTarget = true;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + direction * targetDistance;
        StartCoroutine(MoveToTargetAndBack());
    }


    /// <summary>
    /// Controls the objects cycle.
    /// </summary>
    /// <returns>Waits until each part is completed.</returns>
    IEnumerator MoveToTargetAndBack()
    {
        while (true)
        {
            // Move towards the target position
            yield return StartCoroutine(MoveObject(targetPosition));

            // Pause at the target
            yield return new WaitForSeconds(pauseDuration);

            // Move back to the starting position
            yield return StartCoroutine(MoveObject(startPosition));

            // Pause at the start
            yield return new WaitForSeconds(pauseDuration);
        }
    }


    /// <summary>
    /// Controls the objects movement.
    /// </summary>
    /// <param name="destination">The point the object aims for.</param>
    /// <returns>Null</returns>
    IEnumerator MoveObject(Vector3 destination)
    {
        movingToTarget = true;

        while (movingToTarget)
        {
            // Calculate the distance to the target
            float distanceRemaining = 
                Vector3.Distance(transform.position, destination);

            // Calculate the acceleration/deceleration based on distance
            float decelerationDistance = 
                (currentSpeed * currentSpeed) / (2 * acceleration);
            if (distanceRemaining <= decelerationDistance)
            {
                // Decelerate the object as it approaches the target
                currentSpeed -= acceleration * Time.deltaTime;
            }
            else
            {
                // Accelerate until we need to decelerate
                currentSpeed += acceleration * Time.deltaTime;
            }

            // Clamp speed to ensure it doesn't go over maxSpeed
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

            // Move the object towards the destination
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                currentSpeed * Time.deltaTime
            );

            // Check if we've reached the destination
            if (distanceRemaining <= 0.01f)
            {
                movingToTarget = false;
                currentSpeed = 0f; // Reset speed when reaching the destination
            }

            yield return null; // Wait for the next frame
        }
    }
}