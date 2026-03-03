using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleSystemCallback : MonoBehaviour
{
    public UnityEvent callback;

    void OnParticleSystemStopped()
    {
        callback?.Invoke();
    }
}
