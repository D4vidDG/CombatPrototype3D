
// using UnityEngine;

// namespace RPG.Combat
// {
//     [CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/New Weapon Config", order = 0)]
//     public class WeaponConfig : ScriptableObject
//     {
//         [SerializeField] Weapon equippedPrefab = null;
//         [SerializeField] AnimatorOverrideController animatorOverride = null;
//         [SerializeField] float weaponRange;
//         [SerializeField] float weaponDamage;
//         [SerializeField] float percentageBonus;
//         [SerializeField] Projectile projectile;
//         [SerializeField] bool isRightHand = true;

//         const string weaponName = "Weapon"; //Created to use in Find() method when trying to destroy the weapon



//         public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
//         {
//             Weapon weapon = null;
//             Transform weaponHand = isRightHand ? rightHand : leftHand;
//             if (equippedPrefab != null)
//             {
//                 weapon = Instantiate(equippedPrefab, weaponHand.transform);
//                 weapon.gameObject.name = weaponName;
//             }

//             if (animatorOverride != null)
//             {
//                 animator.runtimeAnimatorController = animatorOverride;
//             }
//             else
//             {
//                 var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
//                 if (overrideController != null) //Is the controller overriden?
//                 {
//                     //If it is, and this weapon has no animator override controller, set to parent ("default") controller
//                     animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
//                 }
//             }

//             return weapon;

//         }

//         public bool HasProjectile()
//         {
//             return projectile != null;
//         }

//         public void LaunchProjectile(Health target, GameObject sender, float calculatedDamage, Transform rightHand, Transform leftHand)
//         {
//             Transform weaponHand = isRightHand ? rightHand : leftHand;
//             Projectile projectileInstance = Instantiate(projectile, weaponHand.position, Quaternion.identity, null);
//             projectileInstance.SetTarget(target, calculatedDamage);
//             projectileInstance.SetSender(sender);
//         }


//         public float GetRange()
//         {
//             return weaponRange;
//         }

//         public float GetDamage()
//         {
//             return weaponDamage;
//         }

//         public float GetPercentageBonus()
//         {
//             if (percentageBonus < 0) return 0;
//             return percentageBonus;
//         }
//     }
// }
