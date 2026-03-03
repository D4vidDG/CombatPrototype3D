using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraShaker : MonoBehaviour
{
    CinemachineBasicMultiChannelPerlin noise;
    float timer;

    Coroutine currentShake;

    private void Awake()
    {
        noise = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Start()
    {
        noise.m_AmplitudeGain = 0;
    }

    public void Shake(CameraShakeParams shakeParams)
    {
        if (currentShake != null)
        {
            StopCoroutine(currentShake);
        }

        currentShake = StartCoroutine(ShakeRoutine(shakeParams, false)); ;

    }

    public void ShakeWithFadeOut(CameraShakeParams shakeParams)
    {
        if (currentShake != null)
        {
            StopCoroutine(currentShake);
        }

        currentShake = StartCoroutine(ShakeRoutine(shakeParams, true));
    }

    public bool IsShaking()
    {
        return currentShake != null;
    }

    private IEnumerator ShakeRoutine(CameraShakeParams shakeParams, bool fade)
    {
        timer = shakeParams.duration;
        noise.m_AmplitudeGain = shakeParams.magnitude;
        noise.m_FrequencyGain = shakeParams.frequency;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            if (fade) noise.m_AmplitudeGain = shakeParams.magnitude * timer / (shakeParams.duration);

            yield return null;
        }
        noise.m_AmplitudeGain = 0;
        currentShake = null;
    }

}

[Serializable]
public struct CameraShakeParams
{
    public float magnitude;
    public float duration;
    public float frequency;
}