using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShotsBar : MonoBehaviour
{
    [SerializeField] ShootState shootState;
    [SerializeField] UIBar bar;
    [SerializeField] float barSizeInPixels;
    [Header("Decrement Preview")]
    [SerializeField] Image decrementPreview;
    [SerializeField] float blinkFrequency;
    [SerializeField] Color shotsAvailable;
    [SerializeField] Color shotsUnavailable;


    Player player;
    bool decrementPreviewActive = false;

    void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    void Start()
    {
        PreviewShotDecrement(false);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barSizeInPixels * shootState.GetMaxNumberOfShots());
    }

    void Update()
    {
        if (shootState != null)
        {
            float percentage = shootState.GetNumberOfShots() / shootState.GetMaxNumberOfShots() * 100f;
            bar.SetPercentage(percentage);
            //if weapon is being aimed
            if (player.GetCurrentState() is ShootState)
            {
                if (!decrementPreviewActive) PreviewShotDecrement(true);
            }
            else
            {
                if (decrementPreviewActive) PreviewShotDecrement(false);
            }
        }
    }

    private void PreviewShotDecrement(bool preview)
    {
        if (preview)
        {
            decrementPreviewActive = true;
            float shotDecrement = shootState.GetWeapon().GetDecrementPerShot();
            decrementPreview.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barSizeInPixels * shotDecrement);
            StartCoroutine(Blink());
        }
        else
        {
            decrementPreviewActive = false;
            decrementPreview.color = new Color(decrementPreview.color.r, decrementPreview.color.b, decrementPreview.color.g, 0);
            StopAllCoroutines();
        }
    }

    private IEnumerator Blink()
    {
        float t = 0;
        while (true)
        {
            decrementPreview.color = shootState.AreShotsAvailable() ? shotsAvailable : shotsUnavailable;
            float alpha = 1 - Mathf.PingPong(t * blinkFrequency, 1);
            decrementPreview.color = new Color(decrementPreview.color.r, decrementPreview.color.b, decrementPreview.color.g, alpha);
            yield return null;
            t += Time.deltaTime;
        }
    }
}
