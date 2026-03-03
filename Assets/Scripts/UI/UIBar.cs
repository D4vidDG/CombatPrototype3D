
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UIBar : MonoBehaviour
{
    Slider slider;
    float percentage = 100f;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        slider.value = 1;
    }

    private void Update()
    {
        slider.value = percentage / 100;
    }

    public float GetPercentage()
    {
        return percentage;
    }

    public void SetPercentage(float percentage)
    {
        this.percentage = Mathf.Clamp(percentage, 0, 100);
    }

    public void Add(float percentage)
    {
        this.percentage += percentage;
        this.percentage = Mathf.Clamp(this.percentage, 0, 100);
    }
}
