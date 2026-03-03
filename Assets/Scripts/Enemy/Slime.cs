using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Slime : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] Vector2 attackDistance;
    [SerializeField] Vector2 timeToAttack;
    [SerializeField] Vector2 waitTimeAfterAttack;
    [SerializeField] float aggressionDistance;
    [SerializeField] float aggressionTime;
    [SerializeField] float attackPreparationTime;
    [SerializeField] float attackPreparationOffset;
    [SerializeField] AttackHitbox attackHitbox;
    [SerializeField] AttackAlertUI attackAlertUI;
    [SerializeField] float facingSpeed;
    [SerializeField] ParticleSystem deathVFX;
    [SerializeField] LayerMask playerMask;
    [SerializeField] Roamer roamer;

    AIMovement movement;
    private GameObject player;
    private Health health;
    Animator animator;
    Collider myCollider;
    KnockbackTarget knockbackTarget;

    float MinTimeToAttack => timeToAttack.x;
    float MaxTimeToAttack => timeToAttack.y;
    float MinAttackDistance => attackDistance.x;
    float MaxAttackDistance => attackDistance.y;

    float aggressionTimer;
    float currentAttackDistance;
    float currentTimeToAttack;
    bool attacking = false;

    void OnValidate()
    {
        if (MinAttackDistance < 0) attackDistance.x = 0;
        if (MinTimeToAttack < 0) timeToAttack.x = 0;

        if (MaxAttackDistance < 0) attackDistance.y = 0;
        if (MaxTimeToAttack < 0) timeToAttack.y = 0;

        if (MaxAttackDistance < MinAttackDistance) attackDistance.x = attackDistance.y;
        if (MaxTimeToAttack < MinTimeToAttack) timeToAttack.x = timeToAttack.y;

        if (attackPreparationOffset > attackPreparationTime) attackPreparationOffset = attackPreparationTime;
    }

    void Awake()
    {
        movement = GetComponent<AIMovement>();
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();
        knockbackTarget = GetComponent<KnockbackTarget>();
        player = FindObjectOfType<Player>().gameObject;
    }

    void Start()
    {
        aggressionTimer = Mathf.NegativeInfinity;
        attackAlertUI.Disable();
        attackHitbox.Disable();
        RandomizeAttackParameters();
    }

    void OnEnable()
    {
        health.OnDead.AddListener(OnDead);
        health.OnAttacked.AddListener(OnAttacked);
        health.OnRevived.AddListener(OnRevived);
    }

    void OnDisable()
    {
        health.OnDead.RemoveListener(OnDead);
        health.OnAttacked.RemoveListener(OnAttacked);
        health.OnRevived.RemoveListener(OnRevived);

    }

    private void RandomizeAttackParameters()
    {
        currentAttackDistance = Random.Range(MinAttackDistance, MaxAttackDistance);
        currentTimeToAttack = Random.Range(MinTimeToAttack, MaxTimeToAttack);
    }

    void Update()
    {
        if (health.IsDead()) return;
        if (attacking) return;

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
                //attack
                Attack();
            }
        }
    }

    private void Attack()
    {
        StartCoroutine(AttackCoroutine());
    }

    void LateUpdate()
    {
        animator.SetBool("Moving", movement.velocity.magnitude > 0.5f);
        animator.SetBool("Die", health.IsDead());
    }

    private IEnumerator AttackCoroutine()
    {
        attacking = true;
        movement.Enable(false);
        //wait to attack
        float attackTimer = 0;
        while (attackTimer < currentTimeToAttack)
        {
            //if player moves out of reach
            if (!IsPlayerWithinDistance(currentAttackDistance))
            {
                //cancel attack
                RandomizeAttackParameters();
                attacking = false;
                movement.Enable(true);
                yield break;
            }
            else
            {
                attackTimer += Time.deltaTime;
                yield return null;
            }

            FacePlayer();
        }

        //enable UI that alerts attack
        float attackPreparationTimer = 0;
        attackAlertUI.Enable();

        //update alert UI before attack animation
        while (attackPreparationTimer < attackPreparationTime - attackPreparationOffset)
        {
            attackPreparationTimer += Time.deltaTime;
            attackAlertUI.SetProgress(attackPreparationTimer / attackPreparationTime);
            FacePlayer();
            yield return null;
        }
        //disable alert if there's not offset into the animation
        if (attackPreparationOffset == 0) attackAlertUI.Disable();

        //enable attack animation
        animator.SetTrigger("Attack");
        animator.applyRootMotion = true;
        knockbackTarget.EnableKnockback(false);
        attackHitbox.Enable();
        myCollider.excludeLayers = playerMask;

        yield return null;

        // while attack animation plays
        yield return new WaitWhile(() => animator.IsInTransition(0));
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 && !animator.IsInTransition(0))
        {
            //if player is hit
            if (attackHitbox.GetTargetsInside().Length > 0)
            {
                attackHitbox.ApplyDamageToTargets(damage);
                attackHitbox.Disable();
            }

            // update UI that alerts attack
            if (attackPreparationTimer < attackPreparationTime)
            {
                attackPreparationTimer += Time.deltaTime;
                attackAlertUI.SetProgress(attackPreparationTimer / attackPreparationTime);
            }
            else
            {
                attackAlertUI.Disable();
            }

            yield return null;
        }

        myCollider.excludeLayers = 0;
        attackAlertUI.Disable();
        attackHitbox.Disable();
        animator.applyRootMotion = false;
        knockbackTarget.EnableKnockback(true);
        yield return new WaitForSeconds(Random.Range(waitTimeAfterAttack.x, waitTimeAfterAttack.y));

        //randomize time to attack
        currentTimeToAttack = Random.Range(MinTimeToAttack, MaxTimeToAttack);
        movement.Enable(true);
        attacking = false;
    }

    private bool IsPlayerWithinDistance(float distance)
    {
        return Vector3.Distance(this.transform.position, player.transform.position) < distance;
    }

    private bool IsAggressive()
    {
        return aggressionTimer > 0;
    }


    private void FacePlayer()
    {
        Vector3 vectorToPlayer = player.transform.position - transform.position;
        vectorToPlayer.y = 0;
        vectorToPlayer = vectorToPlayer.normalized;
        transform.forward = Vector3.MoveTowards(transform.forward, vectorToPlayer, facingSpeed * Time.deltaTime);
    }


    private void OnAttacked(float damage)
    {
        if (!health.IsDead() && !attacking)
        {
            animator.SetTrigger("Hit");
        }
    }


    private void OnDead()
    {
        StopAllCoroutines();
        attackAlertUI.Disable();
        attackHitbox.Disable();
        movement.Enable(false);
        attacking = false;
        aggressionTimer = Mathf.NegativeInfinity;
    }

    private void OnRevived()
    {
        RandomizeAttackParameters();
        movement.Enable(true);
        knockbackTarget.EnableKnockback(true);
        deathVFX.transform.SetParent(this.transform);
    }

    //animation event
    private void PlayDeathEffect()
    {
        ParticleSystem instance = Instantiate(deathVFX, deathVFX.transform.position, deathVFX.transform.rotation, null);
        instance.transform.localScale = new(transform.localScale.x * instance.transform.localScale.x, transform.localScale.y * instance.transform.localScale.y, transform.localScale.z * instance.transform.localScale.z);
        instance.Play(true);
        AudioManager.instance.PlaySoundAtPosition(SoundName.SlimeDeath, transform.position, true);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (0 < attackDistance.x) Gizmos.DrawWireSphere(this.transform.position, attackDistance.x);
        if (0 < attackDistance.y) Gizmos.DrawWireSphere(this.transform.position, attackDistance.y);
        Gizmos.color = Color.green;
        if (0 < currentAttackDistance) Gizmos.DrawWireSphere(this.transform.position, currentAttackDistance);
        Gizmos.color = Color.cyan;
        if (0 < aggressionDistance) Gizmos.DrawWireSphere(this.transform.position, aggressionDistance);
    }
}