using ExtensionMethods;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    [SerializeField] bool grounded;

    [Tooltip("What layers the character uses as ground")]
    [SerializeField] LayerMask GroundLayers;

    void Start()
    {
        grounded = false;
    }

    public bool IsGrounded()
    {
        return grounded;
    }

    private void OnTriggerStay(Collider other)
    {
        if (LayerMaskExtensions.IsInLayerMask(GroundLayers, other.gameObject))
        {
            grounded = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (LayerMaskExtensions.IsInLayerMask(GroundLayers, other.gameObject))
        {
            grounded = false;
        }
    }
}
