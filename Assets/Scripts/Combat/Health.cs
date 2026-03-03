using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth;

    float health;
    bool isDead;
    bool isInvincible;


    public UnityEvent<float> OnHealed;
    public UnityEvent<float> OnAttacked;
    public UnityEvent OnRevived;
    public UnityEvent OnDead;


    private void Start()
    {
        health = maxHealth;
    }

    private void OnEnable()
    {
        Reset();
    }

    public float GetHealthPercentage()
    {
        return (health / maxHealth) * 100;
    }
    public float GetHealth()
    {
        return health;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (isInvincible) return;
        health = Mathf.Max(0, health - damage);
        OnAttacked?.Invoke(damage);

        if (health == 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        if (isInvincible) return;
        health = Mathf.Min(maxHealth, health + amount);
        OnHealed?.Invoke(amount);
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void Revive()
    {
        Reset();
        OnRevived?.Invoke();
    }

    public void SetInvincible(bool isInvincible)
    {
        this.isInvincible = isInvincible;
    }

    private void Reset()
    {
        health = maxHealth;
        isDead = false;
    }

    private void Die()
    {
        isDead = true;
        OnDead?.Invoke();
    }

    private void OnDestroy()
    {
        OnAttacked.RemoveAllListeners();
        OnDead.RemoveAllListeners();
    }
}
