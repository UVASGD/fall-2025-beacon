using UnityEngine;

public class CarrierMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;               // Current movement speed (0 = stationary)
    public float changeTargetInterval = 5f;    // How often to pick a new target
    public float stoppingDistance = 1f;        // Distance from target position to stop

    [Header("Orbit Settings")]
    private Transform centerPoint;              // Point to orbit around (e.g., Earth)
    public float minDistance = 20f;            // Don't move closer than this
    public float maxDistance = 40f;            // Don't move farther than this

    private Vector3 targetPosition;
    private float timer;

    void Start()
    {
        centerPoint = EarthController.Singleton.transform;
        PickNewTargetPosition();
    }

    void Update()
    {
        if (moveSpeed <= 0f || centerPoint == null) return;

        timer += Time.deltaTime;

        if (timer >= changeTargetInterval || Vector3.Distance(transform.position, targetPosition) <= stoppingDistance)
        {
            PickNewTargetPosition();
            timer = 0f;
        }

        MoveTowardTarget();
    }

    void PickNewTargetPosition()
    {
        const int maxAttempts = 10;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minDistance, maxDistance);
            Vector3 candidate = centerPoint.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            // Check if path to target gets too close to Earth
            float minDist = DistanceFromPointToSegment(centerPoint.position, transform.position, candidate);
            if (minDist >= minDistance)
            {
                targetPosition = candidate;
                return;
            }

            attempts++;
        }

        // If no safe path found, fallback to current position
        targetPosition = transform.position;
    }

    float DistanceFromPointToSegment(Vector3 point, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        Vector3 ap = point - a;

        float t = Mathf.Clamp01(Vector3.Dot(ap, ab) / ab.sqrMagnitude);
        Vector3 closestPoint = a + t * ab;

        return Vector3.Distance(point, closestPoint);
    }

    void MoveTowardTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 move = direction * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
        transform.position += move;
    }

    // Call this to disable movement temporarily
    public void StopMovement()
    {
        moveSpeed = 0f;
    }

    // Call this to resume movement with a specific speed
    public void ResumeMovement(float speed)
    {
        moveSpeed = speed;
    }
}
