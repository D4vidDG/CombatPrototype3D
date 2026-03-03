using System;
using System.Collections;
using UnityEngine;

public class AttackAlertUI : MonoBehaviour
{
    [SerializeField] GraphicToColor[] graphicsToColor;
    [SerializeField] float fadeInTime;
    [SerializeField] GameObject fill;
    [SerializeField] float fillMinScale;
    [SerializeField] float fillMaxScale;
    public bool inverted;

    private IEnumerator FadeIn()
    {
        float startScale = transform.localScale.x;
        transform.localScale = Vector3.zero;
        float timer = 0;
        yield return new WaitWhile(() =>
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * startScale, timer / fadeInTime);
            timer += Time.deltaTime;
            return timer < fadeInTime;
        });

        transform.localScale = Vector3.one * startScale;
    }

    public void Enable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        StartCoroutine(FadeIn());

    }

    public void SetProgress(float percentage)
    {
        if (inverted)
        {
            fill.transform.localScale = Mathf.Lerp(fillMaxScale, fillMinScale, percentage) * Vector3.one;

        }
        else
        {
            fill.transform.localScale = Mathf.Lerp(fillMinScale, fillMaxScale, percentage) * Vector3.one;
        }

        foreach (GraphicToColor graphic in graphicsToColor)
        {
            graphic.renderer.color = graphic.colorOverLifetime.Evaluate(percentage);

        }
    }

    public void Disable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    [Serializable]
    private class GraphicToColor
    {
        public Gradient colorOverLifetime;
        public SpriteRenderer renderer;
    }

}
