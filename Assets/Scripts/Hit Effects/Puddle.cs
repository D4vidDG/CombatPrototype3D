using UnityEngine;
using System.Collections.Generic;

public class Puddle : MonoBehaviour
{
    [SerializeField] float maxLifetime;
    [SerializeField] float tickRate;
    [SerializeField] float damagePerTick;
    [SerializeField] List<Sprite> sprites;
    [SerializeField] ParticleSystem particles;

    SpriteRenderer spriteRenderer;
    Animator animator;
    PuddleCollider myCollider;

    float remainingTime = 0;
    float tickTimer;
    bool started = false;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        myCollider = GetComponentInChildren<PuddleCollider>();
    }

    private void Start()
    {
        spriteRenderer.sprite = sprites[UnityEngine.Random.Range(0, sprites.Count)];
    }

    private void Update()
    {
        if (!started) return;
        remainingTime = Mathf.Max(0, remainingTime - Time.deltaTime);
        tickTimer += Time.deltaTime;
        if (remainingTime == 0)
        {
            PlayDisappearAnim();
        }
        else if (tickTimer >= (1 / tickRate))
        {
            tickTimer = 0;
            Tick();
        }
    }


    public void StartTicking()
    {
        started = true;
        remainingTime = maxLifetime;
        tickTimer = 0;
        particles.Play(true);
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }


    private void Tick()
    {
        foreach (Health target in myCollider.GetTargetsInside())
        {
            target.TakeDamage(damagePerTick);
        }
    }

    private void PlayDisappearAnim()
    {
        animator.SetTrigger("Disappear");
    }


}
