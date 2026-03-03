using UnityEngine;

public class RagdollPhysics : MonoBehaviour
{
    [SerializeField] GameObject skeleton;

    void Start()
    {
        SetRigidbodyState(true);
        SetColliderState(false);
        GetComponent<Animator>().enabled = true;
    }

    public void Activate()
    {
        GetComponent<Animator>().enabled = false;
        SetRigidbodyState(false);
        SetColliderState(true);
    }


    public void Deactivate()
    {
        GetComponent<Animator>().enabled = true;
        SetRigidbodyState(true);
        SetColliderState(false);
    }

    void SetRigidbodyState(bool state)
    {

        Rigidbody[] rigidbodies = skeleton.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }


    }


    void SetColliderState(bool state)
    {
        foreach (Collider collider in skeleton.GetComponentsInChildren<Collider>())
        {
            collider.enabled = state;
        }

        GetComponent<Collider>().enabled = !state;
    }
}
