using UnityEngine;

public class ProjectileWeapon : RangedWeapon
{
    [SerializeField] float projectileDamage;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] Transform shootingPoint;
    [SerializeField] float shootingCooldown;
    [SerializeField] ParticleSystem shootingVFX;
    [SerializeField] SoundName shootingSound;
    [SerializeField] float decrementPerShoot;

    float shootCooldownTimer;

    void Start()
    {
        shootCooldownTimer = 0;
    }

    void Update()
    {
        if (0 < shootCooldownTimer) shootCooldownTimer -= Time.deltaTime;
    }

    public override void Shoot(Vector3 aimDirection)
    {
        Projectile projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity, null);
        projectile.Launch(aimDirection);
        projectile.OnTargetHit += OnTargetHit;
        shootCooldownTimer = shootingCooldown;
        Instantiate(shootingVFX, shootingPoint.transform.position, shootingPoint.rotation, null);
        AudioManager.instance.PlaySound(shootingSound, true);
    }

    private void OnTargetHit(Health health)
    {
        health.TakeDamage(projectileDamage);
    }

    public override float GetDecrementPerShot()
    {
        return decrementPerShoot;
    }


    public override bool CanShoot()
    {
        return shootCooldownTimer <= 0;
    }

}
