using System;
using System.Collections;
using ExtensionMethods;
using UnityEngine;
using UnityEngine.AI;

public class RuptureTarget : MonoBehaviour
{
    const float MIN_COLLISION_NORMAL_ANGLE = 45;

    [SerializeField] float healthPercentageThreshold;
    [SerializeField] float timeLimit;

    bool ruptureEnabled = false;
    bool launched = false;
    float timer;

    AIMovement movement;
    Rigidbody rigidBody;
    Health health;
    Collider myCollider;
    Animator animator;
    Action<Collision> onCollision;

    public Action OnRuptureEnabled;
    public Action OnRuptureDisabled;
    public Action OnImpact;

    float cosineMinAngle;

    void Start()
    {
        cosineMinAngle = Mathf.Cos(MIN_COLLISION_NORMAL_ANGLE * Mathf.Deg2Rad);
    }

    void Awake()
    {
        health = GetComponent<Health>();
        movement = GetComponent<AIMovement>();
        rigidBody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        health.OnDead.AddListener(OnDead);
        health.OnRevived.AddListener(OnRevived);
    }


    void OnDisable()
    {
        health.OnDead.RemoveListener(OnDead);
        health.OnRevived.RemoveListener(OnRevived);
    }

    void Update()
    {
        if (health.IsDead()) return;
        if (launched) return;
        if (timer >= timeLimit) return;


        if (!ruptureEnabled && health.GetHealthPercentage() <= healthPercentageThreshold)
        {
            EnableRupture(true);
            return;
        }

        if (ruptureEnabled)
        {
            timer += Time.deltaTime;
            if (timer >= timeLimit) EnableRupture(false);
        }
    }

    void LateUpdate()
    {
        animator.SetBool("Rupture", ruptureEnabled);
    }

    public void EnableRupture(bool enable)
    {
        if (enable)
        {
            ruptureEnabled = true;
            OnRuptureEnabled?.Invoke();
            animator.SetTrigger("RuptureTrigger");
        }
        else
        {
            ruptureEnabled = false;
            OnRuptureDisabled?.Invoke();
            animator.ResetTrigger("RuptureTrigger");
        }
    }

    public bool IsRuptureEnabled()
    {
        return ruptureEnabled;
    }

    public Vector3 GetLaunchDirection(Vector3 playerPosition, RuptureLaunchParams launchParams, out bool collision)
    {
        collision = false;

        Vector3 vectorFromPlayerToTarget = transform.position - playerPosition;
        vectorFromPlayerToTarget.y = 0;
        vectorFromPlayerToTarget = vectorFromPlayerToTarget.normalized;

        bool hit = Physics.BoxCast(
            GetCenter(),
            new Vector3(launchParams.collisionPointSearchRadius, 0.1f, 0.1f),
            vectorFromPlayerToTarget,
            out RaycastHit hitInfo,
            Quaternion.LookRotation(vectorFromPlayerToTarget),
            launchParams.maxLaunchDistance,
            launchParams.collisionLayers
            );

        if (hit)
        {
            collision = true;
            Vector3 vectorToHit = hitInfo.point - transform.position;
            vectorToHit.y = 0;
            return vectorToHit.normalized;
        }
        else
        {
            return vectorFromPlayerToTarget;
        }
    }

    public Vector3 GetLaunchDirection(Vector3 playerPosition, RuptureLaunchParams launchParams)
    {
        return GetLaunchDirection(playerPosition, launchParams, out bool collision);
    }

    public void Launch(Vector3 playerPosition, RuptureLaunchParams launchParams)
    {
        movement.Enable(false);
        rigidBody.isKinematic = false;

        rigidBody.drag = 0;
        rigidBody.velocity = GetLaunchDirection(playerPosition, launchParams, out bool collision) * launchParams.launchSpeed;
        rigidBody.includeLayers = launchParams.collisionLayers;

        if (!collision)
        {
            StartCoroutine(StopLaunchAfterDistance(launchParams.maxLaunchDistance));
        }

        onCollision = (Collision collision) =>
        {
            OnCollision(collision, launchParams);
        };

        launched = true;
    }

    private void OnDead()
    {
        EnableRupture(false);
    }


    private void OnRevived()
    {
        launched = false;
        timer = 0;
    }

    private IEnumerator StopLaunchAfterDistance(float maxLaunchDistance)
    {
        Vector3 startPosition = transform.position;
        float traveledDistance = 0;

        while (traveledDistance < maxLaunchDistance)
        {
            traveledDistance = Vector3.Distance(transform.position, startPosition);
            yield return new WaitForFixedUpdate();
        }

        rigidBody.velocity = Vector3.zero;
        rigidBody.isKinematic = true;
        movement.Enable(true);
        onCollision = null;
        EnableRupture(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (onCollision != null)
        {
            this.onCollision(collision);
        }
    }

    void OnCollision(Collision collision, RuptureLaunchParams ruptureLaunchParams)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            //discard collisions with horizontal surfaces like the floor
            if (Mathf.Abs(Vector3.Dot(Vector3.up, contact.normal)) > cosineMinAngle) continue;

            if (LayerMaskExtensions.IsInLayerMask(ruptureLaunchParams.collisionLayers, collision.collider.gameObject))
            {
                health.TakeDamage(ruptureLaunchParams.collisionDamage);
                OnImpact?.Invoke();
                animator.SetTrigger("Impact");

                if (collision.collider.TryGetComponent<Health>(out var collidedTarget))
                {
                    collidedTarget.TakeDamage(ruptureLaunchParams.collisionDamage / 2);
                }
            }

            rigidBody.velocity = Vector3.zero;
            rigidBody.isKinematic = true;
            movement.Enable(true);
            StopAllCoroutines();
            onCollision = null;
            EnableRupture(false);
        }
    }

    public Vector3 GetCenter()
    {
        return myCollider.bounds.center;
    }
}
