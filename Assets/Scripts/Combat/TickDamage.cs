using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TickDamage : MonoBehaviour
{
    [SerializeField] float damagePerTick;
    [SerializeField] int ticks;
    [SerializeField] float timePerTick;
    [SerializeField] ParticleSystem effect;

    static ObjectPool<ParticleSystem> effectPool;

    static Dictionary<Health, Coroutine> routineByTarget;
    static Dictionary<Health, ParticleSystem> effectByTarget;

    void Awake()
    {
        if (effectPool == null) effectPool = new ObjectPool<ParticleSystem>(OnEffectCreate, actionOnDestroy: OnEffectDestroy);
        if (routineByTarget == null) routineByTarget = new();
        if (effectByTarget == null) effectByTarget = new();
    }

    public void ApplyTickDamage(Health target)
    {
        if (target.IsDead()) return;
        if (!routineByTarget.ContainsKey(target))
        {
            ParticleSystem effect = effectPool.Get();
            effectByTarget.Add(target, effect);
            Coroutine routine = target.StartCoroutine(ApplyTickDamageRoutine(target, effect));
            routineByTarget.Add(target, routine);
        }
        else
        {
            Debug.Log(routineByTarget.ContainsKey(target));
            if (routineByTarget[target] != null) StopCoroutine(routineByTarget[target]);
            Coroutine routine = target.StartCoroutine(ApplyTickDamageRoutine(target, effectByTarget[target]));
            routineByTarget[target] = routine;
        }
    }

    private IEnumerator ApplyTickDamageRoutine(Health target, ParticleSystem effect)
    {
        int tickCount = 0;
        effect.Play();
        effect.transform.position = target.transform.position;
        effect.transform.parent = target.transform;

        float timer = 0;
        while (!target.IsDead() && tickCount < ticks)
        {
            timer = 0;
            yield return new WaitUntil(() =>
            {
                timer += Time.deltaTime;
                return target.IsDead() || timer > timePerTick;
            });

            target.TakeDamage(damagePerTick);
            tickCount++;
        }

        effect.Stop();
        effectPool.Release(effect);
        effect.transform.parent = null;
        effectByTarget.Remove(target);
        routineByTarget.Remove(target);
    }

    private ParticleSystem OnEffectCreate()
    {
        return Instantiate(effect);
    }

    private void OnEffectDestroy(ParticleSystem system)
    {
        Destroy(system.gameObject);
    }
}
