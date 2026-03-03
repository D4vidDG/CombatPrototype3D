using System;
using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float timeToExplode;
    [SerializeField] float damage;
    [SerializeField] float timeToAlert;
    [SerializeField] AttackAlertUI attackAlertUI;
    [SerializeField] SphereCollider sphereCollider;
    [SerializeField] ParticleSystem explosionVFX;
    AttackHitbox hitbox;

    public float Radius => sphereCollider.radius;

    void OnValidate()
    {
        if (timeToAlert > timeToExplode) timeToAlert = timeToExplode;
    }

    void Awake()
    {
        hitbox = GetComponent<AttackHitbox>();
    }

    void Start()
    {
        attackAlertUI.Disable();
    }

    public void Trigger()
    {
        StartCoroutine(ExplodingRoutine());
    }

    private IEnumerator ExplodingRoutine()
    {
        yield return null;
        hitbox.Enable();
        yield return new WaitForSeconds(timeToAlert);
        attackAlertUI.Enable();

        float timer = 0;
        float remainingTime = timeToExplode - timeToAlert;

        yield return new WaitUntil(() =>
        {
            attackAlertUI.SetProgress(timer / remainingTime);
            timer += Time.deltaTime;
            return timer > remainingTime;
        });

        hitbox.ApplyDamageToTargets(damage);
        explosionVFX.gameObject.SetActive(true);
        explosionVFX.Play(true);
        AudioManager.instance.PlaySoundAtPosition(SoundName.BombExplosion, transform.position, true);
        hitbox.Disable();
        yield return null;
        attackAlertUI.Disable();
        yield return new WaitWhile(() => explosionVFX.isPlaying);
        Destroy(this.gameObject);
    }
}
