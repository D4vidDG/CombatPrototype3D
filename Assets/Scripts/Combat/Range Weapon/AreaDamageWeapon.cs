using UnityEngine;

public class AreaDamageWeapon : RangedWeapon
{
    [SerializeField] float damagePerSecond;
    [SerializeField] float shotsDecrementPerSecond;
    [SerializeField] AttackHitbox hitbox;

    public override void Shoot(Vector3 aimDirection)
    {
        hitbox.ApplyDamageToTargets(damagePerSecond * Time.deltaTime);
    }

    public override float GetDecrementPerShot()
    {
        return shotsDecrementPerSecond * Time.deltaTime;
    }

    public override bool CanShoot()
    {
        return true;
    }

}
