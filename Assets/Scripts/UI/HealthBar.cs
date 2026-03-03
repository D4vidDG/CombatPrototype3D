using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] UIBar healthBar;
    [SerializeField] UIBar delayedHealthBar;
    [SerializeField] float delay;
    [SerializeField] float delayedBarReductionSpeed;
    [SerializeField] TextMeshProUGUI healthText;

    float delayTimer;

    void OnEnable()
    {
        health.OnAttacked.AddListener(OnDamageTaken);
    }

    void OnDisable()
    {
        health.OnAttacked.RemoveListener(OnDamageTaken);
    }

    private void OnDamageTaken(float damage)
    {
        delayTimer = delay;
    }

    void Update()
    {
        if (healthBar != null) healthBar.SetPercentage(health.GetHealthPercentage());
        if (delayedHealthBar != null && delayTimer <= 0)
        {
            delayedHealthBar.SetPercentage(Mathf.MoveTowards(delayedHealthBar.GetPercentage(), health.GetHealthPercentage(), delayedBarReductionSpeed));
        }
        if (healthText != null) healthText.text = health.GetHealth().ToString();
        if (delayTimer > 0) delayTimer -= Time.deltaTime;
    }


}
