using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [SerializeField] Path path;
    [SerializeField] bool loopPath = true;
    [SerializeField] float dwellingTime = 1f;

    AIMovement movement;
    int currentWaypointNumber = 1;

    private void Awake()
    {
        movement = GetComponent<AIMovement>();
    }

    private void Start()
    {
        movement.SetDestination(GetCurrentWaypoint());
    }

    void Update()
    {
        if (path != null)
        {
            if (ReachedEndOfPath() && !loopPath) return;
            FollowPath();
        }
    }

    public void SetPath(Path path)
    {
        this.path = path;
        currentWaypointNumber = 1;
    }

    private void FollowPath()
    {
        if (movement.AtDestination())
        {
            if (movement.GetTimeSinceReachedDestination() > dwellingTime)
            {
                CycleWaypoint();
                movement.SetDestination(GetCurrentWaypoint());
            }
        }

    }

    private void CycleWaypoint()
    {
        currentWaypointNumber = (currentWaypointNumber % path.GetNumberOfWaypoints()) + 1;
    }

    private bool ReachedEndOfPath()
    {
        return currentWaypointNumber == path.GetNumberOfWaypoints();
    }

    private Vector3 GetCurrentWaypoint()
    {
        return path.GetWaypoint(currentWaypointNumber);
    }
}
