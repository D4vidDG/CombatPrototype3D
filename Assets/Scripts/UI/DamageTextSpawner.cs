using UnityEngine;
public class DamageTextSpawner : MonoBehaviour
{
    [SerializeField] DamageText damageTextPrefab;
    [SerializeField] Transform spawnPosition;
    [SerializeField] Health health;

    private void Awake()
    {
        health = GetComponentInParent<Health>();
    }

    private void OnEnable()
    {
        health.OnAttacked.AddListener(Spawn);
    }

    private void OnDisable()
    {
        health.OnAttacked.RemoveListener(Spawn);
    }


    public void Spawn(float damage)
    {
        if (Mathf.Approximately(damage, 0)) return;
        DamageText instance =
            Instantiate(damageTextPrefab, spawnPosition.position, spawnPosition.rotation, spawnPosition);

        instance.SetDamageText(damage);
    }
}
