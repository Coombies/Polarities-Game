using UnityEngine;
using System.Collections;

public class ObjectMovement : MonoBehaviour
{
    public float maxSpeed = 50f;    // Maximum speed the object will reach
    public float acceleration = 30f; // Acceleration rate
    public float targetDistance = 3.5f; // Distance from the starting point to the target
    public float pauseDuration = 0.2f; // How long to pause at the target point
    public Vector3 direction = Vector3.down;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float currentSpeed = 0f;
    private bool movingToTarget = true;
    private bool isPaused = false;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + direction * targetDistance; // Move along the x-axis
        StartCoroutine(MoveToTargetAndBack());
    }

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

    IEnumerator MoveObject(Vector3 destination)
    {
        movingToTarget = true;

        while (movingToTarget)
        {
            // Calculate the distance to the target
            float distanceRemaining = Vector3.Distance(transform.position, destination);

            // Calculate the acceleration/deceleration based on distance
            float decelerationDistance = (currentSpeed * currentSpeed) / (2 * acceleration);
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
            transform.position = Vector3.MoveTowards(transform.position, destination, currentSpeed * Time.deltaTime);

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