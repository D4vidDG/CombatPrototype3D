
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] GameObject targetToDestroy;

    public void Destroy()
    {
        Destroy(targetToDestroy);
    }
}
