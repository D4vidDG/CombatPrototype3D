using System;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;

public class RollState : MonoBehaviour, State
{
    [SerializeField] float speed;
    [SerializeField] float rollTime;
    [SerializeField] AnimationCurve deccelerationCurve;
    [SerializeField] GroundCheck groundCheck;
    [SerializeField] ParticleSystem rollVFX;
    [Header("Counter Roll")]
    [SerializeField] float worldTimeScale;
    [SerializeField] float slowedRollTime;
    [SerializeField] float maxDistanceToCounter;
    [SerializeField] float nonAttackedWindow;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] CounterVFX counterVFX;

    //rolling
    Vector3 rollingDirection;
    float rollTimer;

    //counter roll
    bool counterRoll = false;
    float slowMotionTimer;
    float timeSinceAttacked;
    PlayerInvincibility playerInvincibility;

    //states
    MovementState movementState;
    AttackState attackState;

    Health health;

    void Awake()
    {
        movementState = GetComponent<MovementState>();
        attackState = GetComponent<AttackState>();
        playerInvincibility = GetComponent<PlayerInvincibility>();
        health = GetComponent<Health>();
    }

    private void Start()
    {
        timeSinceAttacked = Mathf.Infinity;
        rollTimer = Mathf.Infinity;
        slowMotionTimer = -Mathf.Infinity;
        counterRoll = false;
    }

    void OnEnable()
    {
        health.OnAttacked.AddListener(OnAttacked);
    }

    void OnDisable()
    {
        health.OnAttacked.RemoveListener(OnAttacked);
    }


    public void Enter(Player player)
    {
        rollTimer = 0;
        rollingDirection = Vector3.zero;
        player.animator.SetBool("Roll", true);
        AudioManager.instance.PlaySound(SoundName.RollStart);
        rollVFX.Play();

        if (PerfectDodge(out CounterTarget target))
        {
            StartCounter(target);
        }
    }

    public void Exit(Player player)
    {
        player.animator.SetBool("Roll", false);

        if (counterRoll)
        {
            counterRoll = false;
            attackState.EnableCounterAttack();
            player.mover.SetHorizontalVelocity(Vector2.zero);
            playerInvincibility.EnableInvincibility(false, false);
        }
    }

    protected void Update()
    {
        if (slowMotionTimer > 0) slowMotionTimer -= Time.unscaledDeltaTime;
        if (timeSinceAttacked < nonAttackedWindow) timeSinceAttacked += Time.deltaTime;
    }

    public State UpdateState(Player player, PlayerInputs input)
    {
        //return roll to not-slowed speed after slow motion
        if (counterRoll && slowMotionTimer < 0)
        {
            player.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        //get roll direction
        if (rollingDirection == Vector3.zero)
        {
            rollingDirection = GetRollDirection(player, input);
            //rotate towards roll
            transform.rotation = Quaternion.LookRotation(rollingDirection, transform.up);
        }

        //update roll speed based on time
        if (rollTimer < rollTime)
        {
            float currentRollingSpeed = deccelerationCurve.Evaluate(1.0f - rollTimer / rollTime) * speed;

            //if normal roll
            if (!counterRoll)
            {
                //roll normally
                player.mover.SetHorizontalVelocity(currentRollingSpeed * rollingDirection);
                rollTimer += Time.deltaTime;

            }
            else //if counter roll
            {
                if (slowMotionTimer > 0)
                {
                    //roll in slow motion
                    player.mover.SetHorizontalVelocity(currentRollingSpeed * rollingDirection);
                    rollTimer += Time.deltaTime;
                }
                else
                {
                    //roll at normal speed
                    player.mover.SetHorizontalVelocity(currentRollingSpeed * rollingDirection / worldTimeScale);
                    rollTimer += Time.unscaledDeltaTime;
                }
            }

            return null;
        }
        else
        {
            //return to movement when roll is done
            return movementState;
        }
    }

    public void LateUpdateState(Player player)
    {
    }

    private Vector3 GetRollDirection(Player player, PlayerInputs input)
    {
        if (input.move != Vector2.zero)
        {
            return (player.mover.GetForwardDirection() * input.move.y +
                player.mover.GetRightDirection() * input.move.x).normalized;
        }
        else
        {
            //if no input, return current forward direction
            return transform.forward;
        }
    }


    private bool PerfectDodge(out CounterTarget target)
    {
        target = null;
        //if player was recently attacked, not a perfect dodge
        if (timeSinceAttacked < nonAttackedWindow) return false;

        //find nearby targerts that are in a counterable state
        Collider[] candidates = Physics.OverlapSphere(transform.position, maxDistanceToCounter, enemyLayer);
        foreach (Collider candidate in candidates)
        {
            if (candidate.TryGetComponent<CounterTarget>(out var counterTarget) && counterTarget.IsCounterable())
            {
                target = counterTarget;
                return true;
            }
        }

        return false;
    }


    private void StartCounter(CounterTarget target)
    {
        counterRoll = true;
        Time.timeScale = worldTimeScale;
        Time.fixedDeltaTime *= Time.timeScale;
        slowMotionTimer = slowedRollTime;
        counterVFX.Enable();
        playerInvincibility.EnableInvincibility(true, false);
        attackState.SetCounterTarget(target);
    }

    private void StopCounter()
    {
        counterRoll = false;
        Time.fixedDeltaTime /= Time.timeScale;
        Time.timeScale = 1;
        slowMotionTimer = 0;
        counterVFX.Disable();
        playerInvincibility.EnableInvincibility(false, false);
    }

    private void OnAttacked(float damage)
    {
        timeSinceAttacked = 0;
        if (counterRoll)
        {
            StopCounter();
        }
    }

    //animation event
    public void OnRollLanding()
    {
        if (groundCheck.IsGrounded())
        {
            AudioManager.instance.PlaySound(SoundName.RollLand);
            rollVFX.Play();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistanceToCounter);
    }
}