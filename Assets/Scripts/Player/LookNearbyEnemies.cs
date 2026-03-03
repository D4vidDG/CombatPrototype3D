using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LookNearbyEnemies : MonoBehaviour
{
    [SerializeField] float minRadius;
    [SerializeField] float headAimChangeRate;
    [SerializeField] float headRigWeightChangeRate;
    [SerializeField] Transform head;
    [SerializeField] Rig headRig;
    [SerializeField] Transform headAimTarget;
    [SerializeField] bool debug;

    float headRigTargetWeight = 0;

    private void Start()
    {
        headRig.weight = 0;
    }

    private void Update()
    {
        Vector3 newHeadAimTarget = head.position;
        if (AreEnemiesNeraby(out Transform closestEnemy))
        {
            headRigTargetWeight = 1;
            newHeadAimTarget = new Vector3(closestEnemy.position.x, head.position.y, closestEnemy.position.z);

            if (IsEnemyBehind(closestEnemy))
            {
                newHeadAimTarget = ClampAimTargetToFront(newHeadAimTarget);
            }
        }
        else
        {
            headRigTargetWeight = 0;
        }
        headRig.weight = Mathf.MoveTowards(headRig.weight, headRigTargetWeight, Time.deltaTime * headRigWeightChangeRate);
        headAimTarget.position = Vector3.MoveTowards(headAimTarget.position, newHeadAimTarget, headAimChangeRate * Time.deltaTime);
    }

    private Vector3 ClampAimTargetToFront(Vector3 aimTarget)
    {
        Vector3 aimTargetRelativeToSelf = transform.InverseTransformPoint(aimTarget);
        aimTargetRelativeToSelf = new Vector3(aimTargetRelativeToSelf.x, aimTargetRelativeToSelf.y, -aimTargetRelativeToSelf.z);
        return transform.TransformPoint(aimTargetRelativeToSelf);
    }

    private bool IsEnemyBehind(Transform enemyTransform)
    {
        Vector3 enemyPosRelativeToSelf = transform.InverseTransformPoint(enemyTransform.position);
        return enemyPosRelativeToSelf.z < 0;
    }

    bool AreEnemiesNeraby(out Transform closestEnemy)
    {
        closestEnemy = null;
        Collider[] colliders = Physics.OverlapSphere(head.position, minRadius);
        if (colliders.Length == 0) return false;

        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out AIMovement enemy))
            {
                float distance = Vector3.Distance(head.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestEnemy = enemy.transform;
                    closestDistance = distance;
                }
            }
        }

        if (closestEnemy == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (debug)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(head.position, minRadius);
        }
    }
}



