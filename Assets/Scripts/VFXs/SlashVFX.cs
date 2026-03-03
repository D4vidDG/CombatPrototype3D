using System;
using System.Collections.Generic;
using UnityEngine;

public class SlashVFX : MonoBehaviour
{
    [SerializeField] GameObject slashVFX;
    [SerializeField] Transform effectPose;
    [SerializeField] AnimationTimeData[] attackClipsTimeData;

    Dictionary<AnimationClip, AnimationTimeData> timeDataByAnimationClip;
    Animator playerAnimator;


    Vector3 localScale;


    void Awake()
    {
        playerAnimator = FindObjectOfType<Player>().animator;
        timeDataByAnimationClip = new Dictionary<AnimationClip, AnimationTimeData>();
        foreach (AnimationTimeData animationTimeData in attackClipsTimeData)
        {
            timeDataByAnimationClip.Add(animationTimeData.clip, animationTimeData);
        }

        localScale = slashVFX.transform.localScale;
    }

    void Start()
    {
        slashVFX.transform.SetParent(null, true);
        slashVFX.transform.localScale = localScale;
    }

    void OnDisable()
    {
        if (slashVFX != null)
        {
            slashVFX.transform.SetParent(null, true);
            slashVFX.transform.localScale = localScale;
        }
    }

    void LateUpdate()
    {
        AnimatorClipInfo[] animatorClipInfos = playerAnimator.GetCurrentAnimatorClipInfo(0);
        if (animatorClipInfos.Length < 1) return;
        AnimationClip currentClip = animatorClipInfos[0].clip;
        AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        if (timeDataByAnimationClip.ContainsKey(currentClip))
        {
            AnimationTimeData animationTimeData = timeDataByAnimationClip[currentClip];
            if (stateInfo.normalizedTime >= animationTimeData.EndNormalizedTime)
            {
                slashVFX.transform.SetParent(null, true);
                slashVFX.transform.localScale = localScale;

            }
            else if (stateInfo.normalizedTime >= animationTimeData.StartNormalizedTime)
            {
                slashVFX.transform.SetParent(effectPose);
                slashVFX.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                slashVFX.transform.localScale = localScale;
            }
        }
        else
        {
            slashVFX.transform.SetParent(null, true);
            slashVFX.transform.localScale = localScale;
        }
    }

    [Serializable]
    private class AnimationTimeData
    {
        public AnimationClip clip;
        public float startFrame;
        public float endFrame;

        public float StartNormalizedTime
        {
            get
            {
                if (clip == null) return 0;
                return startFrame / (clip.length * clip.frameRate);
            }
        }
        public float EndNormalizedTime
        {
            get
            {
                if (clip == null) return 0;
                return endFrame / (clip.length * clip.frameRate);
            }
        }

    }
}
