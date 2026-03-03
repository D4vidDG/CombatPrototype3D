using UnityEngine;

[ExecuteInEditMode]
public class MirrorChain : MonoBehaviour
{
    [SerializeField] Chain targetChain;

    Chain myChain;

    void Awake()
    {
        myChain = GetComponent<Chain>();
    }

    void Update()
    {
        if (targetChain != null)
        {
            myChain.startPoint.SetLocalPositionAndRotation(
                targetChain.startPoint.transform.localPosition,
                targetChain.startPoint.transform.localRotation
            );

            myChain.endPoint.SetLocalPositionAndRotation(
                targetChain.endPoint.transform.localPosition,
                targetChain.endPoint.transform.localRotation
            );

            myChain.curveSizeMultiplier = targetChain.curveSizeMultiplier;
        }

    }
}
