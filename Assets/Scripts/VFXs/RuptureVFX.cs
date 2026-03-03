using System;
using UnityEngine;

[RequireComponent(typeof(RuptureTarget))]
public class RuptureVFX : MonoBehaviour
{
    [SerializeField] Renderer normalRenderer;
    [SerializeField] Renderer ruptureRenderer;
    [SerializeField] ParticleSystem ruptureParticles;
    [SerializeField] ParticleSystem impactVFX;

    RuptureTarget ruptureTarget;

    void Awake()
    {
        ruptureTarget = GetComponent<RuptureTarget>();
    }

    void Start()
    {
        DisableVFX();
    }

    void OnEnable()
    {
        ruptureTarget.OnRuptureEnabled += EnableVFX;
        ruptureTarget.OnRuptureDisabled += DisableVFX;
        ruptureTarget.OnImpact += PlayImpactVFX;
    }


    void OnDisable()
    {
        ruptureTarget.OnRuptureEnabled -= EnableVFX;
        ruptureTarget.OnRuptureDisabled -= DisableVFX;
        ruptureTarget.OnImpact -= PlayImpactVFX;

    }

    private void EnableVFX()
    {
        normalRenderer.gameObject.SetActive(false);
        ruptureRenderer.gameObject.SetActive(true);
        ruptureParticles.Play(true);
    }

    private void DisableVFX()
    {
        normalRenderer.gameObject.SetActive(true);
        ruptureRenderer.gameObject.SetActive(false);
        ruptureParticles.Stop(true);
        CancelInvoke();
    }

    private void PlayImpactVFX()
    {
        impactVFX.Play();
        AudioManager.instance.PlaySound(SoundName.WeaponHit);
    }
}
