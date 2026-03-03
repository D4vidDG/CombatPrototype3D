using System.Collections;
using UnityEngine;

public class DualChain : MonoBehaviour
{
    [SerializeField] Chain rightChain;
    [SerializeField] Chain leftChain;
    [SerializeField] bool enableLeftChain;

    // Update is called once per frame
    void Update()
    {
        if (enableLeftChain)
        {
            leftChain.gameObject.SetActive(true);
            MirrorRightChain();
        }
        else
        {
            leftChain.gameObject.SetActive(false);
        }
    }

    private void MirrorRightChain()
    {
        leftChain.startPoint.SetLocalPositionAndRotation(
                    rightChain.startPoint.transform.localPosition,
                    rightChain.startPoint.transform.localRotation
                );

        leftChain.endPoint.SetLocalPositionAndRotation(
            rightChain.endPoint.transform.localPosition,
            rightChain.endPoint.transform.localRotation
        );

        leftChain.curveSizeMultiplier = rightChain.curveSizeMultiplier;
    }
}
