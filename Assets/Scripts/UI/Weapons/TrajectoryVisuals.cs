using UnityEngine;
using UnityEngine.UIElements;

public class TrajectoryVisuals : MonoBehaviour
{
    [Header("Trajectory Settings")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int lineSegmentCount = 20;
    [SerializeField] private float timeStep = 0.1f;
    [SerializeField] private LayerMask collisionMask;

    private void Awake()
    {
        if (lineRenderer == null)
        {
            Debug.LogWarning("TrajectoryVisuals is missing a LineRenderer reference!");
        }
    }

    // Show the trajectory visuals
    public void Show()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = true;
    }

    // Hide the trajectory visuals
    public void Hide()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    // Update the trajectory visuals based on starting position and velocity
    public void UpdateTrajectory(Vector3 startPosition, Vector3 startVelocity)
    {
        if (lineRenderer == null || !lineRenderer.enabled) return;

        Vector3[] points = new Vector3[lineSegmentCount];

        Vector3 currentPosition = startPosition;
        Vector3 currentVelocity = startVelocity;

        for (int i = 0; i < lineSegmentCount; i++)
        {
            points[i] = currentPosition;

            currentVelocity += Physics.gravity * timeStep;
            Vector3 nextPosition = currentPosition + currentVelocity * timeStep;
            Vector3 direction = nextPosition - currentPosition;
            float distance = direction.magnitude;

            if (Physics.Raycast(currentPosition, direction.normalized, out RaycastHit hit, distance, collisionMask))
            {
                // Hit a valid obstacle, end trajectory here
                points[i] = hit.point;
                for (int j = i + 1; j < lineSegmentCount; j++)
                {
                    points[j] = hit.point;
                }
                break;
            }

            currentPosition = nextPosition;
        }

        lineRenderer.positionCount = lineSegmentCount;
        lineRenderer.SetPositions(points);
    }
}