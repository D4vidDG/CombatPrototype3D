
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] TextMeshPro damageText;

    private void Awake()
    {
        damageText = GetComponentInChildren<TextMeshPro>();
    }

    public void Destroy() //Animation Event
    {
        Destroy(this.gameObject);
    }

    public void SetDamageText(float damage)
    {
        damageText.text = damage.ToString();
    }
}