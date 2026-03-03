using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] float damage;
    public WeaponAttackParameters attackParameters;
    [SerializeField] AttackHitboxData[] attackHitboxData;
    [SerializeField] RuntimeAnimatorController animatorController;
    [SerializeField] SoundName weaponAttackSound;
    [SerializeField] SoundName weaponHitSound;
    [SerializeField] ParticleSystem impactVFX;

    bool equipped;
    Player player;

    void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    public RuntimeAnimatorController GetAnimatorController()
    {
        return animatorController;
    }

    public Health[] ActivateHitbox(int attackNumber)
    {
        Dictionary<Health, AttackHitboxData> chosenHitboxForHitTargets = new();
        int highestPriority = int.MinValue;

        foreach (AttackHitboxData hitboxData in attackHitboxData)
        {
            if (attackNumber != hitboxData.targetAttackNumber) continue;
            if (hitboxData.priority < highestPriority) continue;

            AttackHitbox hitbox = hitboxData.hitbox;

            Health[] targetsInside = hitbox.GetTargetsInside();

            foreach (Health target in targetsInside)
            {
                if (!chosenHitboxForHitTargets.ContainsKey(target))
                {
                    chosenHitboxForHitTargets.Add(target, hitboxData);
                }
                else
                {
                    chosenHitboxForHitTargets[target] = hitboxData;
                }
            }
        }

        foreach (Health target in chosenHitboxForHitTargets.Keys)
        {
            target.TakeDamage(damage * chosenHitboxForHitTargets[target].damageMultiplier);

            Vector3 playerToTarget = target.transform.position - player.transform.position;
            if (impactVFX != null)
            {
                Instantiate(
                    impactVFX,
                    target.GetComponent<Collider>().bounds.center,
                    Quaternion.LookRotation(playerToTarget.normalized, Vector3.up),
                    null
                    );
            }
            AudioManager.instance.PlaySound(weaponHitSound, true);
        }

        AudioManager.instance.PlaySound(weaponAttackSound, true);

        return chosenHitboxForHitTargets.Keys.ToArray();
    }

    public bool IsEquipped()
    {
        return equipped;
    }
}

[Serializable]
public struct WeaponAttackParameters
{
    public float attackCooldown;
    public float attackCooldownOnCounter;
    public float comboTimeWindow;
    public float perfectRangeDistance;
    public float displacementSpeed;
    public float displacementTime;
    public float shotRechargePerAttack;

}

[Serializable]
public class AttackHitboxData
{
    public AttackHitbox hitbox;
    public float damageMultiplier = 1;
    public int priority = 0;
    public int targetAttackNumber;
}
