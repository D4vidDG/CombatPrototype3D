
using System;
using System.Collections.Generic;
using ExtensionMethods;
using StarterAssets;
using UnityEngine;

public class BackpackState : AttackState
{
    [Header("Knockback")]
    [SerializeField] float knockback;
    [SerializeField] int stunFrames;
    [Header("Rupture")]
    [SerializeField] float ruptureAttackRange;
    [SerializeField] RuptureLaunchParams ruptureLaunchParams;
    [SerializeField] LayerMask ruptureTargetsLayer;
    [SerializeField] RuptureStateUI ruptureStateUI;
    [Header("Backpack")]
    [SerializeField] GameObject backpack;
    [SerializeField] Transform backpackEquipPose;
    [SerializeField] Transform backpackSwingPose;

    RuptureTarget currentRuptureTarget;


    public override void Enter(Player player)
    {
        player.animator.SetBool("Backpack", true);
        backpack.transform.SetParent(backpackSwingPose, false);
        base.Enter(player);
    }

    public override void Exit(Player player)
    {
        player.animator.SetBool("Backpack", false);
        backpack.transform.SetParent(backpackEquipPose, false);
        base.Exit(player);
    }

    new void Update()
    {
        base.Update();
        currentRuptureTarget = GetRuptureTarget();
        ruptureStateUI.SetTarget(currentRuptureTarget);
        ruptureStateUI.SetLaunchParams(ruptureLaunchParams);
    }

    public override bool AttackInput(PlayerInputs input)
    {
        return input.backpack;
    }

    //animation event
    public override void OnHitboxActive(int attackID)
    {
        if (attackID != this.attackID) return;
        base.OnHitboxActive(attackID);
        Health[] targetsHit = currentWeapon.ActivateHitbox(attackNumber: 1);

        foreach (Health target in targetsHit)
        {
            //apply rupture launch to current target
            if (target.TryGetComponent(out RuptureTarget ruptureTarget) &&
                ruptureTarget == this.currentRuptureTarget)
            {
                currentRuptureTarget.Launch(transform.position, ruptureLaunchParams);
            }
            //apply knockback
            else if (target.TryGetComponent(out KnockbackTarget knockbackTarget))
            {
                Vector3 direction = (knockbackTarget.transform.position - transform.position).normalized;
                direction.y = 0;
                knockbackTarget.ApplyKnockbackToTarget(direction, knockback);
            }
        }
    }

    private RuptureTarget GetRuptureTarget()
    {
        RuptureTarget[] nearbyTargets = GetNearbyTargets();

        //return the only target
        if (nearbyTargets.Length == 1)
        {
            return nearbyTargets[0];
        }
        else if (nearbyTargets.Length > 1)
        {
            //if there's targets in front, return the closest one
            RuptureTarget[] targetsInFront = GetTargetsInFront(nearbyTargets);
            if (targetsInFront.Length > 0)
            {
                return (RuptureTarget)Utils.FindClosest(transform.position, targetsInFront);
            }
            //else, return the closest one from nearby targets
            else
            {
                return (RuptureTarget)Utils.FindClosest(transform.position, nearbyTargets);
            }
        }
        else
        {
            return null;
        }
    }

    private RuptureTarget[] GetNearbyTargets()
    {
        List<RuptureTarget> targets = new List<RuptureTarget>();
        Collider[] nearbyEnemies = Physics.OverlapSphere(
            transform.position,
            ruptureAttackRange,
            ruptureTargetsLayer
            );

        foreach (Collider enemy in nearbyEnemies)
        {
            if (enemy.TryGetComponent(out RuptureTarget target) && target.IsRuptureEnabled())
            {
                targets.Add(target);
            }
        }

        return targets.ToArray();
    }

    private RuptureTarget[] GetTargetsInFront(RuptureTarget[] targets)
    {
        List<RuptureTarget> targetsInFront = new List<RuptureTarget>();

        foreach (RuptureTarget target in targets)
        {
            //add targets within 90 degrees of front direction
            Vector3 vectorToTarget = (target.transform.position - this.transform.position).normalized;
            vectorToTarget.y = 0;
            float dotProduct = Vector3.Dot(transform.forward, vectorToTarget);
            if (dotProduct > 0)
            {
                targetsInFront.Add(target);
            }
        }

        return targetsInFront.ToArray();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, ruptureAttackRange);
    }
}

[Serializable]
public class RuptureLaunchParams
{
    public float maxLaunchDistance;
    public float launchSpeed;
    public float collisionPointSearchRadius;
    public float collisionDamage;
    public LayerMask collisionLayers;
}