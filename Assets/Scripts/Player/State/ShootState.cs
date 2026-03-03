using StarterAssets;
using UnityEngine;

public class ShootState : MonoBehaviour, State
{
    [SerializeField] float movementSpeed;
    [Header("Shooting")]
    [SerializeField] RangedWeapon defaultWeapon;
    [SerializeField] int maxNumberOfShots;
    [Header("Aiming")]
    [SerializeField] float aimLockRadius;
    [SerializeField] float aimLockHeight;
    [SerializeField] float aimRotationRate;
    [SerializeField] AimingLine aimingLine;
    [SerializeField] LayerMask enemyMask;

    MovementState movementState;
    RollState rollState;

    Vector3 horizontalVelocity;
    Vector3 aimDirection;

    float currentNumberOfShots;

    RangedWeapon currentWeapon;

    void Awake()
    {
        movementState = GetComponent<MovementState>();
        rollState = GetComponent<RollState>();
    }

    void Start()
    {
        SetWeapon(defaultWeapon);
        EquipCurrentWeapon(false);

        aimingLine.gameObject.SetActive(false);
        currentNumberOfShots = maxNumberOfShots;
    }

    public void Enter(Player player)
    {
        aimingLine.gameObject.SetActive(true);
        currentWeapon.gameObject.SetActive(true);
        player.animator.SetBool("Aim", true);
    }

    public void Exit(Player player)
    {
        aimingLine.gameObject.SetActive(false);
        currentWeapon.gameObject.SetActive(false);
        player.animator.SetBool("Aim", false);
    }

    public State UpdateState(Player player, PlayerInputs input)
    {
        if (input.roll)
        {
            return rollState;
        }
        else if (!input.aim)
        {
            return movementState;
        }

        aimDirection = GetAimDirection(input);
        aimingLine.transform.forward = aimDirection.normalized;
        RotateTowardsAim(aimDirection);

        if (input.attack && AreShotsAvailable() && currentWeapon.CanShoot())
        {
            currentWeapon.Shoot(aimDirection);
            currentNumberOfShots -= currentWeapon.GetDecrementPerShot();
            currentNumberOfShots = Mathf.Max(0, currentNumberOfShots);
        }

        Move(player, input);

        return null;
    }

    public RangedWeapon GetWeapon()
    {
        return currentWeapon;
    }

    public void SetWeapon(RangedWeapon weapon)
    {
        if (weapon != null)
        {
            bool isHoldingRangedWeapon = false;
            if (currentWeapon != null)
            {
                isHoldingRangedWeapon = currentWeapon.gameObject.activeInHierarchy;
                EquipCurrentWeapon(false);
            }

            currentWeapon = weapon;

            if (isHoldingRangedWeapon)
            {
                EquipCurrentWeapon(true);
            }
        }
    }

    public void LateUpdateState(Player player)
    {
        float aimForwardVelocity = Vector3.Dot(horizontalVelocity, transform.forward);
        float aimSideVelocity = Vector3.Dot(horizontalVelocity, transform.right);
        player.animator.SetFloat("AimForwardSpeed", aimForwardVelocity);
        player.animator.SetFloat("AimSideSpeed", aimSideVelocity);
        if (aimDirection != Vector3.zero)
        {
            aimingLine.transform.forward = aimDirection.normalized;
        }
    }

    public void ReloadShots(float amount)
    {
        currentNumberOfShots = Mathf.Min(maxNumberOfShots, currentNumberOfShots + amount);
    }

    public float GetNumberOfShots()
    {
        return currentNumberOfShots;
    }

    public int GetMaxNumberOfShots()
    {
        return maxNumberOfShots;
    }


    public bool AreShotsAvailable()
    {
        return currentNumberOfShots >= currentWeapon.GetDecrementPerShot();
    }

    private Vector3 GetAimDirection(PlayerInputs input)
    {
        Vector3 vectorTowardsMouse = input.GetAimingDirectionTowardsMouse().normalized;

        if (currentWeapon.IsTargetLockEnabled())
        {
            bool hit = Physics.BoxCast(
                center: aimingLine.transform.position,
                halfExtents: new Vector3(aimLockRadius / 2, aimLockHeight / 2, 0.1f),
                direction: vectorTowardsMouse,
                out RaycastHit hitInfo,
                orientation: transform.rotation,
                maxDistance: aimingLine.maxLineLength,
                layerMask: enemyMask);

            if (hit)
            {
                return (hitInfo.collider.bounds.center - aimingLine.transform.position).normalized;
            }
            else
            {
                return vectorTowardsMouse;
            }
        }
        else
        {
            return vectorTowardsMouse;
        }
    }

    private void EquipCurrentWeapon(bool equip)
    {
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(equip);
        }
    }


    private void Move(Player player, PlayerInputs input)
    {
        horizontalVelocity = movementState.GetMovementVelocity(player, input, movementSpeed);
        player.mover.SetHorizontalVelocity(horizontalVelocity);
    }

    private void RotateTowardsAim(Vector3 aimDirection)
    {
        //rotate player towards aim
        aimDirection.y = 0;
        if (aimDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.LookRotation(aimDirection, transform.up),
                    Time.deltaTime * aimRotationRate
                    );
        }
    }
}
