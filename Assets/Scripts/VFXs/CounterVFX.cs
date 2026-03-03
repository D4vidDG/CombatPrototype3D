using UnityEngine;
using UnityEngine.Rendering;

public class CounterVFX : MonoBehaviour
{
    [SerializeField] Volume postProcessingVolume;
    [SerializeField] float blendSpeed;

    float targetWeight;

    void Start()
    {
        targetWeight = 0;
        postProcessingVolume.weight = 0;
    }

    void Update()
    {
        postProcessingVolume.weight = Mathf.MoveTowards(postProcessingVolume.weight, targetWeight, blendSpeed * Time.unscaledDeltaTime);
    }

    public void Enable()
    {
        AudioManager.instance.PlaySound(SoundName.TimeSlowDown);
        targetWeight = 1;
    }

    public void Disable()
    {
        AudioManager.instance.PlaySound(SoundName.TimeSpeedUp);
        targetWeight = 0;
    }
}