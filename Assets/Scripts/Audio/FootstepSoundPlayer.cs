using UnityEngine;
using ExtensionMethods;

public class FootstepSoundPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] footstepClips;
    [SerializeField] AudioSource footstepAudioSource;
    [SerializeField] GroundCheck groundCheck;

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (!groundCheck.IsGrounded()) return;
        AnimatorClipInfo animatorClipInfo = animationEvent.animatorClipInfo;
        AudioClip randomClip = ArrayExtensions.GetRandom<AudioClip>(footstepClips);

        if (animatorClipInfo.weight > 0.5f)
        {
            footstepAudioSource.PlayOneShot(randomClip);
        }
        else if (animatorClipInfo.weight == 0.5f)
        {
            if (animatorClipInfo.clip.name == "Run_N" || animatorClipInfo.clip.name == "Run_S")
            {
                footstepAudioSource.PlayOneShot(randomClip);
            }
        }
    }
}
