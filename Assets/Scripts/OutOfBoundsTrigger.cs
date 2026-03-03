using UnityEngine;

public class OutOfBoundsTrigger : MonoBehaviour
{
    [SerializeField] Transform returnPoint;
    [SerializeField] int ylimit;

    GameObject player;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (player.transform.position.y < ylimit)
        {
            player.transform.position = returnPoint.position;
            Physics.SyncTransforms();
        }
    }
}
