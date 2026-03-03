using System.Collections;
using UnityEngine;

public class EnemyHitVFX : MonoBehaviour
{
    [SerializeField] Renderer targetRenderer;
    [SerializeField] Material hitMaterial;
    [SerializeField] Color hitColor;
    [SerializeField] float hitColorDuration;
    [SerializeField] Color deadColor;
    [SerializeField] float hitColorWhenDeadDuration;
    [SerializeField] float hitToDeadTransitionDuration;

    Material[] originalMaterials;
    Color[] originalColors;
    Material[] materials;
    Material hitMaterialCopy;

    Coroutine currentRoutine;

    void Awake()
    {
        originalMaterials = targetRenderer.materials;
        originalColors = new Color[originalMaterials.Length];
        materials = targetRenderer.materials;
        hitMaterialCopy = new Material(hitMaterial);
    }

    public void ResetVFX()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = originalMaterials[i];
            materials[i].color = originalColors[i];
            targetRenderer.materials = materials;
        }

    }

    public void PlayHitVFX()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            originalColors[i] = originalMaterials[i].color;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = hitMaterialCopy;
            materials[i].color = hitColor;
            targetRenderer.materials = materials;
        }

        yield return new WaitForSeconds(hitColorDuration);

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = originalMaterials[i];
            materials[i].color = originalColors[i];
            targetRenderer.materials = materials;
        }

        currentRoutine = null;
    }

    public void PlayDeadVFX()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(DeadRoutine());
    }

    private IEnumerator DeadRoutine()
    {

        for (int i = 0; i < originalMaterials.Length; i++)
        {
            originalColors[i] = originalMaterials[i].color;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = hitMaterialCopy;
            materials[i].color = hitColor;
            targetRenderer.materials = materials;
        }

        yield return new WaitForSeconds(hitColorWhenDeadDuration);

        float timer = 0;
        while (timer < hitToDeadTransitionDuration)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = Color.Lerp(hitColor, deadColor, timer / hitToDeadTransitionDuration);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = deadColor;
        }

        currentRoutine = null;

    }

    void OnDestroy()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            Destroy(materials[i]);
        }
    }
}
