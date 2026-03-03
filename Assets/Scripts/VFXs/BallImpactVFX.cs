using UnityEngine;

public class BallImpactVFX : MonoBehaviour
{
    [SerializeField] ParticleSystem ballImpactFX;
    [SerializeField] AnimationClip clip;
    [SerializeField] SoundName collisionSound;
    [SerializeField] int frame;

    bool played = false;
    public float NormalizedTime
    {
        get
        {
            if (clip == null) return 0;
            return frame / (clip.length * clip.frameRate);
        }
    }

    Animator playerAnimator;

    void Awake()
    {
        playerAnimator = FindObjectOfType<Player>().animator;
    }

    void LateUpdate()
    {
        AnimatorClipInfo[] animatorClipInfos = playerAnimator.GetCurrentAnimatorClipInfo(0);
        if (animatorClipInfos.Length < 1) return;
        AnimationClip currentClip = animatorClipInfos[0].clip;
        if (currentClip == clip)
        {
            if (!played)
            {
                AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.normalizedTime >= NormalizedTime)
                {
                    ballImpactFX.Play(true);
                    AudioManager.instance.PlaySound(SoundName.SpikeBallHit, true);
                    played = true;
                }
            }
        }
        else
        {
            played = false;
        }
    }

}
