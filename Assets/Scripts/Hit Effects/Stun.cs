using System.Collections;
using UnityEngine;
public class Stun : MonoBehaviour, TargetHitEffect
{
    [SerializeField] public float stunTime;

    public void Trigger(Health target)
    {
        if (target.TryGetComponent<AIMovement>(out AIMovement enemyMovement))
        {
            AudioManager.instance.PlaySoundAtPosition(SoundName.CupcakeHit, transform.position);
            IEnumerator stunCoroutine = StunTarget(enemyMovement, target);
            enemyMovement.StartCoroutine(stunCoroutine);
        }
    }

    IEnumerator StunTarget(AIMovement enemyMovement, Health health)
    {
        enemyMovement.CanMove(false);
        yield return new WaitForSeconds(stunTime);
        if (!health.IsDead())
        {
            enemyMovement.CanMove(true);
        }
    }
}