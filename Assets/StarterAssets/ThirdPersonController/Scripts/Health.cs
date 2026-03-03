// using System;
// using System.Collections;
// using UnityEngine;

// public class Health : MonoBehaviour
// {
//     [SerializeField] float maxHealth;
//     [SerializeField] UIBar healthBar;
//     [SerializeField] GameObject damageEffect;
//     [SerializeField] GameObject deadEffect;
//     [SerializeField] GameObject[] disableOnDead;

//     Animator animator;

//     Coroutine damageEffectCoroutine;

//     float health;
//     bool isDead;
//     bool isInvincible;

//     public Action OnDead;
//     public Action<float> OnAttacked;

//     private void Awake()
//     {
//         animator = GetComponentInChildren<Animator>();
//     }

//     private void Start()
//     {
//         health = maxHealth;
//     }

//     private void OnEnable()
//     {
//         Reset();
//     }

//     public float GetHealthPercentage()
//     {
//         return (health / maxHealth) * 100;
//     }
//     public float GetHealth()
//     {
//         return health;
//     }
//     public float GetMaxHealth()
//     {
//         return maxHealth;
//     }

//     public void TakeDamage(float damage)
//     {
//         if (isDead) return;
//         if (isInvincible) return;
//         health -= damage;
//         print(this.gameObject.name + " was attacked");
//         if (damageEffectCoroutine == null) damageEffectCoroutine = StartCoroutine(DamageEffect());
//         OnAttacked?.Invoke(damage);
//         if (healthBar != null) healthBar.SetPercentage(health / maxHealth);
//         if (health <= 0)
//         {
//             Die();
//             health = 0;
//             if (healthBar != null) healthBar.SetPercentage(0);
//         }
//     }

//     public bool IsDead()
//     {
//         return isDead;
//     }

//     public void Reset()
//     {
//         health = maxHealth;
//         isDead = false;
//         if (healthBar != null) healthBar.SetPercentage(health / maxHealth);
//     }

//     public void SetInvincible(bool isInvincible)
//     {
//         this.isInvincible = isInvincible;
//     }

//     private void Die()
//     {
//         isDead = true;
//         if (animator != null) animator.SetTrigger("Die");
//         if (damageEffectCoroutine != null)
//         {
//             StopCoroutine(damageEffectCoroutine);
//             damageEffectCoroutine = null;
//         }
//         if (deadEffect != null) Instantiate(deadEffect, transform.position, Quaternion.identity, null);
//         OnDead?.Invoke();
//         foreach (GameObject gameObject in disableOnDead)
//         {
//             gameObject.SetActive(false);
//         }
//     }

//     private IEnumerator DamageEffect()
//     {
//         if (damageEffect != null) Instantiate(damageEffect, transform.position, Quaternion.identity, null);
//         yield break;
//     }

// }
