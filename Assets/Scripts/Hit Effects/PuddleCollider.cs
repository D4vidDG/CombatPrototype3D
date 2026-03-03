using System;
using System.Collections.Generic;
using UnityEngine;

public class PuddleCollider : MonoBehaviour
{
    List<Health> targetsInside;

    private void Awake()
    {
        targetsInside = new List<Health>();
    }

    public List<Health> GetTargetsInside()
    {
        return targetsInside;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Health>(out Health target))
        {
            targetsInside.Add(target);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Health>(out Health target) && targetsInside.Contains(target))
        {
            targetsInside.Remove(target);
        }
    }
}
