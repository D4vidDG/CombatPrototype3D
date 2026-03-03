using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashingComponent : MonoBehaviour
{
    public float flashDelay;
    public Color initialColor, flashColor;
    public Image image;

    bool isEnabled;

    private void Start()
    {
        isEnabled = false;
    }

    IEnumerator Flash()
    {
        float lerpValue = 0;
        bool flashing = true;
        while (true)
        {
            if (lerpValue < 1)
            {

                if (flashing) image.color = Color.Lerp(initialColor, flashColor, lerpValue);
                else image.color = Color.Lerp(flashColor, initialColor, lerpValue);
            }
            else
            {
                lerpValue = 0;
                flashing = !flashing;
            }

            lerpValue += Time.deltaTime / flashDelay;
            yield return new WaitForEndOfFrame();
        }
    }

    public void ToggleEffect(bool toggle)
    {
        if (toggle && !isEnabled)
        {
            StartCoroutine(Flash());
        }
        else if (!toggle && isEnabled)
        {
            image.color = initialColor;
            StopCoroutine(Flash());
        }

        isEnabled = toggle;
    }
}
