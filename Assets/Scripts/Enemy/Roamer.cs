using UnityEngine;
using UnityEngine.AI;

public class Roamer : MonoBehaviour
{
    [SerializeField] Transform roamingCenterPoint;
    [SerializeField] float minWalkingDistance;
    [SerializeField] float roamingRadius;
    [SerializeField] float dwellingTime;
    [SerializeField] float reachedDestinationDistance;

    AIMovement movement;

    Vector3 roamingCenterPosition;

    const int MAX_ITERATIONS = 50;
    int iterations;

    bool enableRoaming = false;
    float dwellingTimer;


    private void OnValidate()
    {
        if (minWalkingDistance > roamingRadius / 2)
        {
            minWalkingDistance = roamingRadius / 2;
        }
    }

    private void Awake()
    {
        movement = GetComponent<AIMovement>();
    }

    void OnEnable()
    {
        if (movement != null) movement.SetDestination(transform.position);
    }

    private void Start()
    {
        if (roamingCenterPoint == null)
        {
            roamingCenterPosition = transform.position;
        }
        else
        {
            roamingCenterPosition = roamingCenterPoint.position;
        }
    }

    private void Update()
    {
        if (!enableRoaming) return;
        RoamInsideCircle();
    }


    public void Enable(bool enable)
    {
        enableRoaming = enable;
        if (!enable) movement.SetDestination(transform.position);
    }

    private void RoamInsideCircle()
    {
        if (movement.AtDestination())
        {
            dwellingTimer += Time.deltaTime;
            if (dwellingTimer > dwellingTime)
            {
                movement.SetDestination(CalculatePositionInCircle());
            }
        }
        else
        {
            dwellingTimer = 0;
        }
    }

    private Vector3 CalculatePositionInCircle()
    {
        if (iterations >= MAX_ITERATIONS)
        {
            Debug.LogWarning("Roamer " + gameObject.name + ": Max iterations reached");
            return transform.position;
        }

        Vector2 randomVector = Random.insideUnitCircle * Random.Range(0, roamingRadius);
        Vector3 newPosition = roamingCenterPosition + new Vector3(randomVector.x, 0, randomVector.y);
        NavMesh.SamplePosition(newPosition, out NavMeshHit result, Mathf.Infinity, NavMesh.AllAreas);
        float distanceToNewPosition = Vector3.Distance(result.position, transform.position);

        NavMeshPath path = new NavMeshPath();
        if (distanceToNewPosition < minWalkingDistance || !NavMesh.CalculatePath(transform.position, newPosition, NavMesh.AllAreas, path))
        {
            iterations++;
            return CalculatePositionInCircle();
        }

        return result.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (roamingCenterPoint != null)
        {
            Gizmos.DrawWireSphere(roamingCenterPoint.position, roamingRadius);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, roamingRadius);
        }
    }

}

