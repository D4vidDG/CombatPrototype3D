using System;
using System.Collections;
using UnityEngine;

public class PlayerHitFX : MonoBehaviour
{
    [SerializeField] CanvasGroup redOverlay;
    [SerializeField] float redOverlayDuration;
    [SerializeField] AnimationCurve redOverlayAlphaOverTime;
    [SerializeField] CameraShakeParams cameraShakeParams;
    [SerializeField] SoundPlayer hitSoundPlayer;

    CameraShaker cameraShaker;
    Coroutine playCoroutine;

    void Awake()
    {
        cameraShaker = FindObjectOfType<CameraShaker>();
    }

    public void Play()
    {
        hitSoundPlayer.Play();
        cameraShaker.Shake(cameraShakeParams);
        if (playCoroutine != null) StopCoroutine(playCoroutine);
        playCoroutine = StartCoroutine(PlayCoroutine());
    }

    private IEnumerator PlayCoroutine()
    {
        float timer = 0;

        yield return new WaitWhile(() =>
        {
            timer += Time.deltaTime;
            redOverlay.alpha = redOverlayAlphaOverTime.Evaluate(timer / redOverlayDuration);
            return timer < redOverlayDuration;
        });

        redOverlay.alpha = 0;

        playCoroutine = null;
    }
}