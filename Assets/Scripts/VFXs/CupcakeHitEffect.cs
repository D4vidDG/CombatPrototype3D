using System.Collections;
using UnityEngine;

public class CupcakeHitEffect : MonoBehaviour, TargetHitEffect
{
    public void OnEnvironmentHit(Vector3 hitPoint)
    {
    }

    public void Trigger(Health target)
    {
        CupcakeObject cupcake = target.GetComponentInChildren<CupcakeObject>();
        if (cupcake != null)
        {
            cupcake.Enable();
            cupcake.StartCoroutine(DisableCupcake(cupcake));
        }
    }

    private IEnumerator DisableCupcake(CupcakeObject cupcake)
    {
        yield return new WaitForSeconds(GetComponent<Stun>().stunTime);
        cupcake.Disable();
    }
}
