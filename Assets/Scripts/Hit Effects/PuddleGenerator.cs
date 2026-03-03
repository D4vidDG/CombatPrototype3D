using ExtensionMethods;
using UnityEngine;

public class PuddleGenerator : MonoBehaviour, TargetHitEffect, EnvironmentHitEffect
{
    [SerializeField] Puddle puddlePrefab;
    [SerializeField] LayerMask environmentLayer;
    [SerializeField] float maxStepAngle;

    const float Y_INSTANCE_OFFSET = 0.1f;
    const float Y_RAY_OFFSET = 0.1f;

    public void Trigger(Health target)
    {
        Ray ray = new Ray(target.transform.position + Vector3.up * Y_RAY_OFFSET, Vector3.down);
        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, environmentLayer);

        if (hit)
        {
            float surfaceNormalAngle = Mathf.Acos(Vector3.Dot(Vector3.up, hitInfo.normal.normalized)) * Mathf.Rad2Deg;

            if (surfaceNormalAngle < maxStepAngle)
            {
                Puddle puddleInstance = Instantiate(puddlePrefab,
                    hitInfo.point + Vector3.up * Y_INSTANCE_OFFSET,
                    puddlePrefab.transform.rotation,
                    null);
                puddleInstance.gameObject.transform.forward = hitInfo.normal.normalized;
            }
        }
    }

    public void Trigger(Collision collision)
    {
        ContactPoint contactPoint = collision.GetContact(0);
        Vector3 surfaceNormal = contactPoint.normal.normalized;
        float surfaceNormalAngle = Mathf.Acos(Vector3.Dot(Vector3.up, surfaceNormal)) * Mathf.Rad2Deg;

        if (surfaceNormalAngle < maxStepAngle)
        {
            Puddle puddleInstance = Instantiate(puddlePrefab,
                contactPoint.point + Vector3.up * Y_INSTANCE_OFFSET,
                puddlePrefab.transform.rotation,
                null);
            puddleInstance.gameObject.transform.forward = surfaceNormal;
        }
    }

}



