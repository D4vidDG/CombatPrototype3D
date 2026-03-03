
using Cinemachine;
using StarterAssets;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;


public class CameraLookAhead : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public PlayerInputs _input;
    public float idleValue;
    public float movingValue;

    CinemachineFramingTransposer transposer;

    private void Awake()
    {
        transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    void LateUpdate()
    {
        if (_input.move == Vector2.zero)
        {
            transposer.m_LookaheadSmoothing = idleValue;
        }
        else
        {
            transposer.m_LookaheadSmoothing = movingValue;

        }
    }
}
