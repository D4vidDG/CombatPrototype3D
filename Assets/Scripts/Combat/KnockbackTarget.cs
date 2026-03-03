using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class KnockbackTarget : MonoBehaviour
{
    [SerializeField] bool applyKnockback = true;
    [SerializeField] float stunFrames;

    Rigidbody rigidBody;
    AIMovement movement;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        movement = GetComponent<AIMovement>();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void EnableKnockback(bool enable)
    {
        applyKnockback = enable;
    }

    public void ApplyKnockbackToTarget(Vector3 direction, float knockback)
    {
        if (!applyKnockback) return;

        movement.bufferChanges = true;
        movement.Enable(false);

        Vector3 velocity = movement.velocity;
        rigidBody.isKinematic = false;
        rigidBody.velocity = velocity;
        rigidBody.AddForce(direction * knockback, ForceMode.Impulse);

        StartCoroutine(DisablePhysicsAfterKnockback());
    }


    private IEnumerator DisablePhysicsAfterKnockback()
    {
        yield return null;
        int frames = 0;
        while (frames < stunFrames)
        {
            frames++;
            yield return new WaitForFixedUpdate();
        }

        Vector3 velocity = rigidBody.velocity;

        rigidBody.isKinematic = true;

        movement.Enable(true);
        movement.SetVelocity(velocity);
        movement.bufferChanges = false;
        movement.ApplyBufferedChanges();
    }
}