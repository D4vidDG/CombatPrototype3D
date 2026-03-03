using System.Collections;
using ExtensionMethods;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Bomber : MonoBehaviour
{
    private const float BOMBING_DISTANCE_TOLERANCE = 0.1f;
    [SerializeField] float facingSpeed;
    [Header("Attack")]
    [SerializeField] GroundCheck groundCheck;
    [SerializeField] GroundCheck animationGroundCheck;
    [SerializeField] Vector2 attackDistance;
    [SerializeField] Vector2 timeToAttack;
    [SerializeField] Vector2 waitTimeAfterAttack;
    [SerializeField] float aggressionDistance;
    [SerializeField] float aggressionTime;
    [SerializeField] float recoveryTime;
    [Header("Vertical Movement")]
    [SerializeField] float flyingHeight;
    [SerializeField] float groundHeight;
    [SerializeField] float ascendSpeed;
    [SerializeField] float fallSpeedAfterHit;
    [Header("Bombing")]
    [SerializeField] float bombingDistance;
    [SerializeField] float bombingStartOffset;
    [SerializeField] float bombSeparation;
    [SerializeField] float airTravelSpeed;
    [SerializeField] Bomb bombPrefab;
    [SerializeField] LayerMask groundLayer;
    [Header("Roaming")]
    [SerializeField] Roamer roamer;


    float MinTimeToAttack => timeToAttack.x;
    float MaxTimeToAttack => timeToAttack.y;

    float MinAttackDistance => attackDistance.x;
    float MaxAttackDistance => attackDistance.y;

    float MinWaitTimeAfterAttack => waitTimeAfterAttack.x;
    float MaxWaitTimeAfterAttack => waitTimeAfterAttack.y;

    float attackTimer;
    float aggressionTimer;
    float currentTimeToAttack;
    float currentAttackDistance;
    bool attacking = false;
    bool falling = false;
    GameObject player;

    Coroutine attackCoroutine;

    Rigidbody rigidBody;
    AIMovement movement;
    Animator animator;
    Health health;
    RuptureTarget ruptureTarget;
    KnockbackTarget knockbackTarget;

    void Awake()
    {
        movement = GetComponent<AIMovement>();
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        knockbackTarget = GetComponent<KnockbackTarget>();
        player = FindObjectOfType<Player>().gameObject;
        ruptureTarget = GetComponent<RuptureTarget>();
    }

    void OnEnable()
    {
        health.OnDead.AddListener(OnDead);
        health.OnAttacked.AddListener(OnAttacked);
        health.OnRevived.AddListener(OnRevived);
        ruptureTarget.OnRuptureEnabled += OnRuptureEnabled;
        ruptureTarget.OnRuptureDisabled += OnRuptureDisabled;
    }

    void OnDisable()
    {
        health.OnDead.RemoveListener(OnDead);
        health.OnAttacked.RemoveListener(OnAttacked);
        health.OnRevived.RemoveListener(OnRevived);
        ruptureTarget.OnRuptureEnabled -= OnRuptureEnabled;
        ruptureTarget.OnRuptureDisabled -= OnRuptureDisabled;

    }

    void Start()
    {
        RandomizeParameters();
        animator.SetInteger("FlyingStage", 0);
        aggressionTimer = Mathf.NegativeInfinity;
    }

    void Update()
    {
        if (health.IsDead()) return;
        if (attacking || falling) return;
        if (ruptureTarget.IsRuptureEnabled()) return;


        if (!IsAggressive())
        {
            if (IsPlayerWithinDistance(aggressionDistance)) aggressionTimer = aggressionTime;
            roamer.Enable(true);
        }
        else
        {
            roamer.Enable(false);
            aggressionTimer -= Time.deltaTime;

            if (!IsPlayerWithinDistance(currentAttackDistance))
            {
                //move towards player
                movement.CanMove(true);
                movement.SetDestination(player.transform.position);
            }
            else
            {

                movement.CanMove(false);
                attackTimer += Time.deltaTime;
                if (attackTimer > currentTimeToAttack)
                {
                    attackTimer = 0;
                    RandomizeParameters();
                    Attack();
                }
                FacePlayer();
            }
        }
    }


    private void FacePlayer()
    {
        Vector3 vectorToPlayer = player.transform.position - transform.position;
        vectorToPlayer.y = 0;
        vectorToPlayer = vectorToPlayer.normalized;
        transform.forward = Vector3.MoveTowards(transform.forward, vectorToPlayer, facingSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        animator.SetFloat("Speed", movement.velocity.magnitude);
        animator.SetBool("Grounded", animationGroundCheck.IsGrounded());
    }


    private void Attack()
    {
        attackCoroutine = StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        knockbackTarget.EnableKnockback(false);
        attacking = true;
        movement.Enable(false);

        animator.SetInteger("FlyingStage", 1);
        yield return Ascend();

        animator.SetInteger("FlyingStage", 2);
        yield return Bombard();

        animator.SetInteger("FlyingStage", 3);
        yield return Descend(ascendSpeed);

        yield return new WaitUntil(() => animationGroundCheck.IsGrounded());
        yield return null;
        yield return AnimatorExtensions.WaitForCurrentAnimatorState(animator, 0);

        knockbackTarget.EnableKnockback(true);
        float waitTime = Random.Range(MinWaitTimeAfterAttack, MaxWaitTimeAfterAttack);

        float timer = 0;
        yield return new WaitWhile(() =>
        {
            FacePlayer();
            timer += Time.deltaTime;
            return timer < waitTime;
        });

        attackCoroutine = null;
        movement.Enable(true);
        animator.SetInteger("FlyingStage", 0);
        attacking = false;
    }

    IEnumerator Bombard()
    {
        //align hitboxs to player direction
        Vector3 attackDirection = player.transform.position - transform.position;
        attackDirection.y = 0;
        attackDirection = attackDirection.normalized;

        //start deploying bombs
        int currentBombNumber = 1;
        float distanceTraveled = 0;
        Vector3 targetPosition = transform.position + attackDirection * bombingDistance;

        while (distanceTraveled < bombingDistance - BOMBING_DISTANCE_TOLERANCE)
        {
            rigidBody.MovePosition(transform.position + airTravelSpeed * Time.fixedDeltaTime * attackDirection);
            distanceTraveled = bombingDistance - Vector3.Distance(transform.position, targetPosition);
            float currentBombDistance = GetBombDistance(currentBombNumber);

            if (distanceTraveled > currentBombDistance)
            {
                Vector3 floorPoint = GetFloorPoint();
                Bomb bomb = Instantiate(bombPrefab, floorPoint + 0.02f * Vector3.up, Quaternion.identity, null);
                bomb.Trigger();
                currentBombNumber++;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private Vector3 GetFloorPoint()
    {
        bool hit = Physics.Raycast(
        transform.position,
        Vector3.down,
        out RaycastHit hitInfo,
        flyingHeight + 10f,
        groundLayer);

        if (hit)
        {
            return hitInfo.point;
        }

        return transform.position;
    }

    private float GetBombDistance(int bombNumber)
    {
        return (bombSeparation + bombPrefab.Radius * 2) * (bombNumber - 1) + bombingStartOffset;
    }

    IEnumerator Ascend()
    {
        return MoveVertically(flyingHeight, ascendSpeed, true);
    }

    IEnumerator Descend(float speed)
    {
        return MoveVertically(groundHeight, speed, false);
    }

    IEnumerator MoveVertically(float targetHeight, float speed, bool up)
    {
        if (up)
        {
            while (transform.position.y < targetHeight)
            {
                rigidBody.MovePosition(transform.position + speed * Time.fixedDeltaTime * Vector3.up);
                yield return new WaitForFixedUpdate();
            }

        }
        else
        {
            while (transform.position.y > targetHeight && !groundCheck.IsGrounded())
            {
                rigidBody.MovePosition(transform.position - speed * Time.fixedDeltaTime * Vector3.up);
                yield return new WaitForFixedUpdate();
            }
        }

        rigidBody.MovePosition(new Vector3(transform.position.x, targetHeight, transform.position.z));
    }

    private void OnAttacked(float damage)
    {
        if (attacking && !falling)
        {
            FallAfterHit();
        }
    }

    private void OnDead()
    {
        movement.Enable(false);
        attacking = false;
        falling = false;
        aggressionTimer = Mathf.NegativeInfinity;
        attackTimer = 0;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        Invoke(nameof(DisableAfterDeath), 6);
    }

    private void OnRevived()
    {
        RandomizeParameters();
        animator.SetInteger("FlyingStage", 0);
        movement.Enable(true);
        animator.Rebind();
        animator.Update(0f);
        knockbackTarget.EnableKnockback(true);
    }

    private void OnRuptureEnabled()
    {
        knockbackTarget.EnableKnockback(false);
        movement.Enable(false);
    }

    private void OnRuptureDisabled()
    {
        if (!health.IsDead())
        {
            movement.Enable(true);
            knockbackTarget.EnableKnockback(true);
        }
        attackTimer = -recoveryTime;
    }

    private void DisableAfterDeath()
    {
        if (!health.IsDead()) return;
        gameObject.SetActive(false);
    }

    private void FallAfterHit()
    {
        StartCoroutine(FallAfterHitCoroutine());
    }

    private IEnumerator FallAfterHitCoroutine()
    {
        falling = true;
        attacking = false;
        StopCoroutine(attackCoroutine);
        attackCoroutine = null;
        animator.SetInteger("FlyingStage", 0);

        if (!groundCheck.IsGrounded())
        {
            animator.SetInteger("FlyingStage", 3);
            yield return StartCoroutine(Descend(fallSpeedAfterHit));
            knockbackTarget.EnableKnockback(true);
        }

        movement.Enable(!health.IsDead());
        attackTimer = -recoveryTime;
        falling = false;
    }

    private void RandomizeParameters()
    {
        currentTimeToAttack = Random.Range(MinTimeToAttack, MaxTimeToAttack);
        currentAttackDistance = Random.Range(MinAttackDistance, MaxAttackDistance);
    }

    private bool IsAggressive()
    {
        return aggressionTimer > 0;
    }

    private bool IsPlayerWithinDistance(float distance)
    {
        return Vector3.Distance(this.transform.position, player.transform.position) < distance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, MinAttackDistance);
        Gizmos.DrawWireSphere(this.transform.position, MaxAttackDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, bombingDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position, aggressionDistance);

        Gizmos.color = Color.blue;
        float spacePerBomb = bombPrefab.Radius * 2 + bombSeparation;
        int numberOfBombs = (int)Mathf.Ceil((bombingDistance - bombingStartOffset) / spacePerBomb);

        for (int bombIndex = 0; bombIndex < numberOfBombs; bombIndex++)
        {
            Vector3 hitboxLocalPosition = GetBombDistance(bombIndex + 1) * transform.forward;
            Gizmos.DrawWireSphere(transform.position + hitboxLocalPosition, bombPrefab.Radius);
        }
    }
}