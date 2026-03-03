using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricSpread : MonoBehaviour, TargetHitEffect
{
    [SerializeField] float damage;
    [SerializeField] float spreadDistance;
    [SerializeField] int maxTargetCount;
    [SerializeField] ParticleSystem electricHit;
    [SerializeField] ElectricSpreadEffect electricSpreadEffect;

    List<Health> targetsHit;


    public void Trigger(Health target)
    {
        targetsHit = new List<Health>
        {
            target
        };

        InstantiateElectricHitEffect(target);

        target.StartCoroutine(Spread(target.GetComponent<Collider>().bounds.center));
        AudioManager.instance.PlaySoundAtPosition(SoundName.ElectricHit, target.transform.position);
    }

    private IEnumerator Spread(Vector3 origin)
    {
        if (maxTargetCount < targetsHit.Count) yield break;

        List<Health> newCloseTargets = GetNewCloseTargets(origin);

        if (0 < newCloseTargets.Count)
        {
            foreach (Health target in newCloseTargets)
            {
                target.TakeDamage(damage);
                targetsHit.Add(target);
                InstantiateElectricHitEffect(target);

                AudioManager.instance.PlaySoundAtPosition(SoundName.ElectricHit, target.transform.position);

                Vector3 targetCenter = target.GetComponent<Collider>().bounds.center;

                if (electricSpreadEffect != null)
                {
                    ElectricSpreadEffect effectInstance = Instantiate<ElectricSpreadEffect>(electricSpreadEffect);
                    effectInstance.Activate(origin, targetCenter);
                }

            }

            yield return new WaitForSeconds(electricSpreadEffect.lifetime);

            foreach (Health target in newCloseTargets)
            {
                Vector3 targetCenter = target.GetComponent<Collider>().bounds.center;
                target.StartCoroutine(Spread(targetCenter));
            }

        }
        else
        {
            yield break;
        }

    }

    private void InstantiateElectricHitEffect(Health target)
    {
        Instantiate(electricHit, target.transform.position, Quaternion.identity, target.transform);
    }

    private List<Health> GetNewCloseTargets(Vector3 origin)
    {
        Collider[] colliders = Physics.OverlapSphere(origin, spreadDistance);
        List<Health> targets = new List<Health>();

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<Health>(out Health target) && !targetsHit.Contains(target))
            {
                targets.Add(target);
            }
        }

        return targets;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spreadDistance);
    }
}
