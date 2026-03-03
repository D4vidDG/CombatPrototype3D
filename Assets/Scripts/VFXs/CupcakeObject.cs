using UnityEngine;

public class CupcakeObject : MonoBehaviour
{
    MeshRenderer meshRenderer;
    ParticleSystem particleEffect;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        particleEffect = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        meshRenderer.enabled = false;
    }

    public void Enable()
    {
        meshRenderer.enabled = true;
        particleEffect.Play();
    }

    public void Disable()
    {
        meshRenderer.enabled = false;
    }
}
