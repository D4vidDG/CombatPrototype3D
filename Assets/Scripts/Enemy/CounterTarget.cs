using UnityEngine;

public class CounterTarget : MonoBehaviour
{
    bool counterable;

    void Start()
    {
        counterable = false;
    }

    public bool IsCounterable()
    {
        return counterable;
    }

    // animation event
    void EnableCounterable()
    {
        counterable = true;
    }

    void DisableCounterable()
    {
        counterable = false;
    }
}
