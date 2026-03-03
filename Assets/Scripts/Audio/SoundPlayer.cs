using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] SoundName sound;

    public void Play()
    {
        AudioManager.instance.PlaySound(sound);
    }

    public void PlayAtPoint()
    {
        AudioManager.instance.PlaySoundAtPosition(sound, transform.position);
    }
}
