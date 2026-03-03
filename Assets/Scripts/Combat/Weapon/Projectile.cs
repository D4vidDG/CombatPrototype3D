using System;
using ExtensionMethods;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float maxLifetime;
    [SerializeField] float maxTravelingDistance;
    [SerializeField] float startSpeed;
    [SerializeField] bool destroyOnHit;
    [SerializeField] GameObject model;
    [SerializeField] GameObject[] persistentEffectsAfterDestroy;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask environmentLayer;
    [SerializeField] ParticleSystem impactVFX;


    Rigidbody rigidBody;
    Collider myCollider;

    bool launched;
    Vector3 startPosition;
    float lifetime;

    public Action<Health> OnTargetHit;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        myCollider.enabled = false;
        EnableModel(false);
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        lifetime += Time.deltaTime;
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (maxTravelingDistance < distanceTraveled || maxLifetime < lifetime)
        {
            Destroy();
        }
    }

    public void Launch(Vector3 direction)
    {
        launched = true;
        EnableModel(true);
        myCollider.enabled = true;
        rigidBody.velocity = direction.normalized * startSpeed;
        transform.forward = direction.normalized;
    }

    protected void EnableModel(bool enable)
    {
        model.SetActive(enable);
    }

    protected bool IsShoot()
    {
        return launched;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other);
    }

    private void HandleHit(Collider other)
    {
        if (LayerMaskExtensions.IsInLayerMask(targetLayer, other.gameObject))
        {
            Health target = other.GetComponentInParent<Health>();
            if (target != null)
            {
                OnTargetHit?.Invoke(target);
                if (impactVFX != null) Instantiate(impactVFX, other.bounds.center, Quaternion.identity, null);
                if (destroyOnHit) Destroy();
                return;
            }
        }

        if (LayerMaskExtensions.IsInLayerMask(environmentLayer, other.gameObject))
        {
            if (impactVFX != null) Instantiate(impactVFX, transform.position, Quaternion.identity, null);
            if (destroyOnHit) Destroy();
            return;
        }
    }

    private void Destroy()
    {
        foreach (GameObject effect in persistentEffectsAfterDestroy)
        {
            if (effect != null)
            {
                effect.transform.parent = null;
            }
        }
        OnTargetHit = null;
        Destroy(this.gameObject);
    }
}
