using System;
using System.Collections;
using StarterAssets;
using UnityEngine;

public class AttackState : MonoBehaviour, State
{
    [Header("Attack")]
    [SerializeField] protected int attackID;
    [SerializeField] int maxAttacksPerCombo = 4;
    [Header("Weapon")]
    [SerializeField] MeleeWeapon defaultWeapon;
    [SerializeField] float weaponEquipTime;
    [SerializeField] bool equipOnStart;
    [SerializeField] Component[] unequipOnStates;
    [Header("Counter Attack")]
    [SerializeField] float counterTime;
    [SerializeField] float speedToPerfectRange;
    [SerializeField] float reachedPerfectRangeTolerance;
    [SerializeField] CounterVFX counterVFX;

    //attack
    protected MeleeWeapon currentWeapon;
    float attackCooldown;
    float comboTimeWindow;
    int attackNumber;
    bool comboFinished;

    //counter
    bool counterActive;
    Coroutine counterMovementRoutine;
    CounterTarget currentCounterTarget;
    Health health;

    //timers
    float attackCooldownTimer;
    float comboTimer;
    float weaponEquipTimer;
    float counterTimer;

    //states
    MovementState movementState;
    RollState rollState;
    AttackState[] attackStates;
    ShootState shootState;
    Player player;

    void OnValidate()
    {
        for (int i = 0; i < unequipOnStates.Length; i++)
        {
            if (unequipOnStates[i] is not State) unequipOnStates[i] = null;
        }
    }

    void Awake()
    {
        movementState = GetComponent<MovementState>();
        rollState = GetComponent<RollState>();
        attackStates = GetComponents<AttackState>();
        shootState = GetComponent<ShootState>();
        player = GetComponent<Player>();
        health = GetComponent<Health>();
    }

    protected void Start()
    {
        attackNumber = 1;
        attackCooldownTimer = 0;
        comboTimer = 0;
        comboFinished = false;
        counterActive = false;
        counterTimer = Mathf.NegativeInfinity;

        SetWeapon(defaultWeapon);
        if (equipOnStart)
        {
            weaponEquipTimer = weaponEquipTime;
            EquipCurrentWeapon(true);
        }
        else
        {
            weaponEquipTimer = 0;
            EquipCurrentWeapon(false);
        }

        // Si el jugador pulsa la tecla y el personaje no está en el carril izquierdo de nuestra pista, entonces
        if (Input.GetKeyDown(KeyCode.A) && transform.position.x > -9)
        {
            // Mover el personaje 9 unidades a la izquierda
            transform.Translate(-9, 0, 0);
        }
        // Si el jugador pulsa la tecla y el personaje no está en el carril izquierdo de nuestra pista, entonces
        if (Input.GetKeyDown(KeyCode.D) && transform.position.x < 9)
        {
            // Mover el personaje 9 unidades a la derecha
            transform.Translate(9, 0, 0);
        }
    }

    void OnEnable()
    {
        health.OnAttacked.AddListener(OnAttacked);
        health.OnAttacked.AddListener(OnDead);
    }

    void OnDisable()
    {
        health.OnAttacked.RemoveListener(OnAttacked);
        health.OnAttacked.RemoveListener(OnDead);
    }

    public virtual void Enter(Player player)
    {
        player.animator.SetInteger("AttackNumber", attackNumber);

        if (!IsCounterAttackActive())
        {
            player.mover.SetHorizontalVelocity(Vector3.zero);
        }

        if (currentWeapon != null) player.animator.runtimeAnimatorController = currentWeapon.GetAnimatorController();
    }

    public virtual void Exit(Player player)
    {
        player.animator.SetInteger("AttackNumber", attackNumber);

        if (!IsCounterAttackActive())
        {
            player.mover.SetHorizontalVelocity(Vector3.zero);
        }

        //reset attack animation
        player.animator.Play(player.animator.GetCurrentAnimatorStateInfo(0).shortNameHash, -1, 0);
    }

    public virtual State UpdateState(Player player, PlayerInputs input)
    {
        //if other attack state is triggered and cooldown is done
        if (IsAttackCooldownDone() && OtherAttackInput(input, out AttackState otherAttack) && !counterActive)
        {
            //go to other attack state
            ResetCombo();
            attackCooldownTimer = 0;
            comboTimer = 0;
            return otherAttack;
        }

        //if the player wants to attack, there's no cooldown, and the combo is not done
        if (AttackInput(input) && IsAttackCooldownDone() && !comboFinished)
        {
            //face attack direction
            Vector3 attackDirection = input.GetAimingDirectionTowardsMouse().normalized;
            if (attackDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(attackDirection, transform.up);
            }

            //trigger attack animation
            player.animator.SetTrigger("Attack");

            //disable attack cooldown until hitbox is activated in attack animation
            attackCooldownTimer = Mathf.Infinity;

            //finish combo if max attacks reached and there's more than one attack
            if (1 < maxAttacksPerCombo && attackNumber == maxAttacksPerCombo)
            {
                comboFinished = true;
            }

            //displace player
            if (!IsCounterAttackActive()) StartCoroutine(DisplacePlayerCoroutine(player, attackDirection));

            //if counter active, move to perfectRange
            if (IsCounterAttackActive() && !OnPerfectRange(player) && counterMovementRoutine == null)
            {
                counterMovementRoutine = StartCoroutine(MoveToPerfectRange(player));
            }

            //reset equip timer
            weaponEquipTimer = weaponEquipTime;
            return null;
        }

        if (comboFinished)
        {
            comboTimer = 0;
        }

        //roll if cooldown is done
        if (input.roll && IsAttackCooldownDone() && !IsCounterAttackActive())
        {
            ResetCombo();
            attackCooldownTimer = 0;
            comboTimer = 0;
            return rollState;
        }

        //return to movement when attack is finished
        if (IsAttackAnimationDone(player.animator))
        {
            if (comboFinished) ResetCombo();
            return movementState;
        }


        return null;
    }

    protected void Update()
    {
        if (attackCooldownTimer > 0) attackCooldownTimer -= Time.deltaTime;
        if (IsCounterAttackActive())
        {
            counterTimer -= Time.unscaledDeltaTime;
            if (counterTimer < 0 || currentCounterTarget.GetComponent<Health>().IsDead())
            {
                DisableCounterAttack();
            }
        }

        if (0 < comboTimer)
        {
            if (IsAttackCooldownDone())
            {
                comboTimer -= Time.deltaTime;
            }
        }
        else if (!comboFinished)
        {
            ResetCombo();
        }


        UpdateWeaponEquip();
    }


    public void LateUpdateState(Player player)
    {
        player.animator.SetInteger("AttackNumber", attackNumber);
    }


    public virtual bool AttackInput(PlayerInputs input)
    {
        return input.attack;
    }

    //animation event
    public virtual void OnHitboxActive(int attackID)
    {
        if (attackID != this.attackID) return;

        Health[] targetsHit = currentWeapon.ActivateHitbox(attackNumber);
        shootState.ReloadShots(currentWeapon.attackParameters.shotRechargePerAttack * targetsHit.Length);

        if (comboFinished)
        {
            //reset timers
            attackCooldownTimer = attackCooldown;
        }
        else
        {
            //cycle attack from combo
            CycleAttack();
            //reset timers
            comboTimer = comboTimeWindow;
            attackCooldownTimer = attackCooldown;
        }

        if (comboFinished) AudioManager.instance.PlaySound(SoundName.StrongGrunt);
        else AudioManager.instance.PlaySound(SoundName.WeakGrunt);
    }

    public void SetWeapon(MeleeWeapon weapon)
    {
        if (weapon != null)
        {
            if (currentWeapon != null)
            {
                EquipCurrentWeapon(false);
            }

            currentWeapon = weapon;
            attackCooldown = weapon.attackParameters.attackCooldown;
            comboTimeWindow = weapon.attackParameters.comboTimeWindow;

            weaponEquipTimer = weaponEquipTime;
        }
    }

    public MeleeWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    private void OnAttacked(float damage)
    {
        if (IsCounterAttackActive())
        {
            DisableCounterAttack();
        }
    }


    private void OnDead(float arg0)
    {
        attackCooldownTimer = 0;
        comboTimer = 0;
        ResetCombo();
    }


    private void UpdateWeaponEquip()
    {
        if (weaponEquipTimer > 0) weaponEquipTimer -= Time.deltaTime;
        //if time to equip weapon is not done AND the current state is movement OR attack
        if (0 < weaponEquipTimer && CanEquipOnCurrentState())
        {
            EquipCurrentWeapon(true);
        }
        else
        {
            EquipCurrentWeapon(false);
        }
    }

    private bool CanEquipOnCurrentState()
    {
        foreach (Component state in unequipOnStates)
        {
            if (player.GetCurrentState().GetType() == state.GetType())
            {
                return false;
            }
        }

        return true;
    }

    private bool OtherAttackInput(PlayerInputs input, out AttackState otherAttack)
    {
        otherAttack = null;
        foreach (AttackState attackState in attackStates)
        {
            if (attackState == this) continue;
            else if (attackState.AttackInput(input))
            {
                otherAttack = attackState;
                return true;
            }
        }

        return false;
    }

    private bool IsAttackCooldownDone()
    {
        return attackCooldownTimer <= 0;
    }

    private void EquipCurrentWeapon(bool equip)
    {
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(equip);
        }
    }

    private IEnumerator DisplacePlayerCoroutine(Player player, Vector3 direction)
    {
        int frames = 0;
        player.mover.SetHorizontalVelocity(currentWeapon.attackParameters.displacementSpeed * direction.normalized);

        while (frames < currentWeapon.attackParameters.displacementTime)
        {
            frames++;
            yield return null;
        }

        player.mover.SetHorizontalVelocity(Vector3.zero);
    }

    private void CycleAttack()
    {
        attackNumber = Mathf.Min(attackNumber + 1, maxAttacksPerCombo);
    }

    private void ResetCombo()
    {
        comboFinished = false;
        attackNumber = 1;
    }

    private bool IsAttackAnimationDone(Animator animator)
    {
        return !animator.IsInTransition(0) && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }

    public bool IsCounterAttackActive()
    {
        return counterActive;
    }

    public void EnableCounterAttack()
    {
        player.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        counterTimer = counterTime;
        attackCooldown = currentWeapon.attackParameters.attackCooldownOnCounter;
        counterActive = true;
    }

    public void SetCounterTarget(CounterTarget target)
    {
        if (target != null)
        {
            this.currentCounterTarget = target;
        }
    }

    private void OnCounterTargetDead()
    {
        if (IsCounterAttackActive())
        {
            DisableCounterAttack();
        }
    }

    private void DisableCounterAttack()
    {
        player.animator.updateMode = AnimatorUpdateMode.Normal;
        counterTimer = -Mathf.NegativeInfinity;
        attackCooldown = currentWeapon.attackParameters.attackCooldown;
        counterActive = false;
        counterVFX.Disable();

        if (currentCounterTarget != null)
        {
            currentCounterTarget.GetComponent<Health>().OnDead.RemoveListener(OnCounterTargetDead);
            currentCounterTarget = null;
        }

        Time.fixedDeltaTime /= Time.timeScale;
        Time.timeScale = 1;
    }

    private IEnumerator MoveToPerfectRange(Player player)
    {
        while (IsCounterAttackActive() && !OnPerfectRange(player))
        {
            Vector3 positionToHitCounterTarget = GetPositionToHitCounterTarget();
            Vector3 vectorToTargetPosition = positionToHitCounterTarget - player.transform.position;
            vectorToTargetPosition.y = 0;
            player.mover.SetHorizontalVelocity(vectorToTargetPosition.normalized * speedToPerfectRange / Time.timeScale);

            float deltaDistance = speedToPerfectRange / Time.timeScale * Time.fixedDeltaTime;
            if (deltaDistance > Vector3.Distance(player.transform.position, positionToHitCounterTarget))
            {
                player.transform.position = positionToHitCounterTarget;
                break;
            }
            else
            {
                yield return new WaitForFixedUpdate();
            }
        }

        player.mover.SetHorizontalVelocity(Vector3.zero);
        counterMovementRoutine = null;
    }

    private Vector3 GetPositionToHitCounterTarget()
    {
        Vector3 vectorToCounterTarget = currentCounterTarget.transform.position - transform.position;
        vectorToCounterTarget.y = 0;
        Vector3 targetPosition = currentCounterTarget.transform.position - vectorToCounterTarget.normalized * currentWeapon.attackParameters.perfectRangeDistance;
        return targetPosition;
    }

    private bool OnPerfectRange(Player player)
    {
        return currentCounterTarget != null &&
            Vector3.Distance(player.transform.position, GetPositionToHitCounterTarget()) < reachedPerfectRangeTolerance;
    }
}