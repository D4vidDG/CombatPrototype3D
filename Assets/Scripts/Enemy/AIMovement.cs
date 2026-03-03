using System;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    [SerializeField] float reachedDestinationTolerance;
    NavMeshAgent navMeshAgent;

    public Vector3 velocity => navMeshAgent.velocity;

    float timeSinceReachedDestination;
    [HideInInspector] public bool bufferChanges;

    Buffer buffer;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        timeSinceReachedDestination = Mathf.Infinity;
        buffer = new Buffer();
    }

    void Start()
    {
        bufferChanges = false;
    }

    private void Update()
    {
        if (AtDestination())
        {
            timeSinceReachedDestination += Time.deltaTime;
        }
    }

    public void Enable(bool enable)
    {
        if (bufferChanges) buffer.enabled = enable;
        navMeshAgent.enabled = enable;
    }

    public void SetDestination(Vector3 destination)
    {
        if (bufferChanges) buffer.destination = destination;
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(destination);
            timeSinceReachedDestination = 0;
        }
    }

    public void SetVelocity(Vector3 velocity)
    {
        navMeshAgent.velocity = velocity;
    }

    public bool AtDestination()
    {
        if (navMeshAgent.enabled)
        {
            return navMeshAgent.remainingDistance <= reachedDestinationTolerance;
        }
        else
        {
            return Vector3.Distance(navMeshAgent.transform.position, navMeshAgent.destination) <= reachedDestinationTolerance;
        }
    }

    public float GetDistanceToDestination()
    {
        return navMeshAgent.remainingDistance;
    }

    public float GetTimeSinceReachedDestination()
    {
        return timeSinceReachedDestination;
    }

    public void CanMove(bool canMove)
    {
        if (bufferChanges) buffer.canMove = canMove;
        if (navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = !canMove;
        }
    }

    public void ApplyBufferedChanges()
    {
        Enable(buffer.enabled);
        SetDestination(buffer.destination);
        CanMove(buffer.canMove);
    }

    struct Buffer
    {
        public bool canMove;
        public Vector3 destination;
        public bool enabled;
    }
}