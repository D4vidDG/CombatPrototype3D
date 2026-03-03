using System;
using StarterAssets;
using UnityEngine;

public class WeaponPicker : MonoBehaviour
{
    [SerializeField] float pickUpRadius;

    PlayerInputs input;
    private AttackState attackState;
    public Action<Weapon> OnPickupWeapon;
    private ShootState shootState;

    WeaponPickUp highlightedWeapon;

    private void Awake()
    {
        input = GetComponent<PlayerInputs>();
        attackState = GetComponent<AttackState>();
        shootState = GetComponent<ShootState>();
    }

    void Update()
    {
        WeaponPickUp closestDroppedWeapon = FindClosestWeapon();
        if (closestDroppedWeapon == null)
        {
            if (highlightedWeapon != null) highlightedWeapon.ShowPickUpUI(false);
        }
        else
        {
            if (highlightedWeapon != null) highlightedWeapon.ShowPickUpUI(false);
            closestDroppedWeapon.ShowPickUpUI(true);
            highlightedWeapon = closestDroppedWeapon;

            if (input.interact)
            {
                PickWeapon(closestDroppedWeapon);
            }
        }
    }

    private void PickWeapon(WeaponPickUp weaponPickUp)
    {
        Weapon weapon = weaponPickUp.GetWeapon();

        if (weapon is MeleeWeapon meleeWeapon)
        {
            attackState.SetWeapon(meleeWeapon);
        }
        else if (weapon is RangedWeapon rangedWeapon)
        {
            shootState.SetWeapon(rangedWeapon);
        }
    }

    private WeaponPickUp FindClosestWeapon()
    {
        return ExtensionMethods.Utils.FindClosest<WeaponPickUp>(transform.position, pickUpRadius);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pickUpRadius);
    }
}
