using System.Collections;
using UnityEngine;

public class PlayerInvincibility : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] GameObject[] flashTargets;
    [SerializeField] float flashRateVFX;
    [SerializeField] Collider hurtbox;
    bool invincible = false;

    Coroutine invincibilityCoroutine;

    void OnDisable()
    {
        if (invincibilityCoroutine != null)
        {
            StopCoroutine(invincibilityCoroutine);
            invincibilityCoroutine = null;
        }
        SetFlashTargetVisibility(true);
    }

    public void EnableInvincibility(bool enable, bool flash)
    {
        if (invincibilityCoroutine != null) StopCoroutine(invincibilityCoroutine);

        SetFlashTargetVisibility(true);
        invincible = enable;
        hurtbox.enabled = !enable;
        invincibilityCoroutine = null;

        if (enable && flash)
        {
            invincibilityCoroutine = StartCoroutine(WaitTimer(Mathf.Infinity, flash));
        }
    }

    public void StartTimedInvincibility(bool flash)
    {
        StartTimedInvincibility(duration, true, flash);
    }

    public void StartTimedInvincibility(float seconds, bool flash, bool overrideState = true)
    {
        if (invincible && !overrideState) return;
        else if (invincible && overrideState)
        {
            if (invincibilityCoroutine != null)
            {
                StopCoroutine(invincibilityCoroutine);
                SetFlashTargetVisibility(true);
            }
        }

        invincibilityCoroutine = StartCoroutine(TimedInvincibilityCoroutine(seconds, flash));
    }

    private IEnumerator WaitTimer(float seconds, bool flash)
    {
        float timer = 0;
        yield return new WaitWhile(() =>
        {
            timer += Time.deltaTime;
            if (flash) FlashVFX(timer);
            return timer < seconds;
        });
    }

    private IEnumerator TimedInvincibilityCoroutine(float seconds, bool flash)
    {
        invincible = true;
        hurtbox.enabled = false;

        yield return WaitTimer(seconds, flash);

        SetFlashTargetVisibility(true);

        hurtbox.enabled = true;
        invincible = false;
        invincibilityCoroutine = null;
    }

    private void FlashVFX(float timer)
    {
        float pingPongValue = Mathf.PingPong(timer * flashRateVFX, 1);

        bool visible = pingPongValue > 0.5f;

        SetFlashTargetVisibility(visible);
    }

    private void SetFlashTargetVisibility(bool visible)
    {
        foreach (GameObject flashTarget in flashTargets)
        {
            flashTarget.SetActive(visible);
        }
    }
}
