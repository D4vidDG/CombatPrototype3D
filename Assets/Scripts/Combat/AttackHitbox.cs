using System.Collections.Generic;
using ExtensionMethods;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackHitbox : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask excludeLayers;

    List<Health> targets;

    Health myself;

    Collider hitboxCollider;

    void Awake()
    {
        targets = new List<Health>();
        hitboxCollider = GetComponent<Collider>();
        myself = GetComponentInParent<Health>();
    }

    public void Enable()
    {
        hitboxCollider.enabled = true;
    }

    public void Disable()
    {
        foreach (Health target in targets)
        {
            target.OnDead.RemoveListener(OnTargetDead);
        }
        targets.Clear();
        hitboxCollider.enabled = false;
    }

    public Health[] GetTargetsInside()
    {
        CleanseInvalidTargets();
        return targets.ToArray();
    }

    public void ApplyDamageToTargets(float damage)
    {
        CleanseInvalidTargets();
        foreach (Health target in targets)
        {
            target.TakeDamage(damage);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (LayerMaskExtensions.IsInLayerMask(targetLayer, other.gameObject))
        {
            Health target = other.GetComponentInParent<Health>();
            if (target == null) return;
            if (LayerMaskExtensions.IsInLayerMask(excludeLayers, target.gameObject)) return;
            if (target != myself && !targets.Contains(target))
            {
                targets.Add(target);
                target.OnDead.AddListener(OnTargetDead);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (LayerMaskExtensions.IsInLayerMask(targetLayer, other.gameObject))
        {
            Health target = other.GetComponentInParent<Health>();
            if (target == null) return;
            if (LayerMaskExtensions.IsInLayerMask(excludeLayers, target.gameObject)) return;
            if (target != myself && targets.Contains(target))
            {
                targets.Remove(target);
                target.OnDead.RemoveListener(OnTargetDead);
            }
        }
    }

    void OnTargetDead()
    {
        CleanseInvalidTargets();
    }

    private void CleanseInvalidTargets()
    {
        if (targets.Count < 1) return;
        targets.RemoveAll(target =>
        {
            if (target == null)
            {
                return true;
            }
            else if (target.IsDead())
            {
                target.OnDead.RemoveListener(OnTargetDead);
                return true;
            }

            return false;
        });
    }

}
