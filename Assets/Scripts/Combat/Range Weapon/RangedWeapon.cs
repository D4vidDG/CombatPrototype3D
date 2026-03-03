using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedWeapon : Weapon
{
    [SerializeField] bool enableTargetLock;
    public abstract void Shoot(Vector3 aimDirection);
    public abstract float GetDecrementPerShot();
    public abstract bool CanShoot();

    public bool IsTargetLockEnabled()
    {
        return enableTargetLock;
    }
}
