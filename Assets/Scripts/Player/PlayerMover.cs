using Cinemachine;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    const float GROUNDED_VERTICAL_VELOCITY = -1f;

    [SerializeField] CharacterController controller;
    [SerializeField] CinemachineVirtualCamera followCamera;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [SerializeField] float gravity = -15.0f;
    [SerializeField] bool applyGravity = true;


    [Tooltip("Max vertical velocity when falling")]
    [SerializeField] float terminalFallingVelocity = 53.0f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [SerializeField] float fallTimeout = 0.15f;

    [SerializeField] GroundCheck groundCheck;

    Vector3 horizontalVelocity;
    float gravityVelocity = 0;
    private float fallTimeoutTimer;
    private Vector3 forwardDirection;
    private Vector3 rightDirection;

    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        //get forward and right movement directions relative to camera
        forwardDirection = followCamera.transform.forward;
        forwardDirection.y = 0;
        forwardDirection = forwardDirection.normalized;

        rightDirection = followCamera.transform.right;
        rightDirection.y = 0;
        rightDirection = rightDirection.normalized;

        // reset our timeouts on start
        fallTimeoutTimer = fallTimeout;
    }

    void FixedUpdate()
    {
        Vector3 gravityVelocity = applyGravity ? CalculateGravityVelocity() : Vector3.zero;
        controller.Move((horizontalVelocity + gravityVelocity) * Time.fixedDeltaTime);
    }

    void LateUpdate()
    {
        animator.SetFloat("Speed", GetVelocity().magnitude);
        animator.SetBool("Grounded", groundCheck.IsGrounded());
    }

    public void SetHorizontalVelocity(Vector3 velocity)
    {
        this.horizontalVelocity = velocity;
    }

    public Vector3 GetForwardDirection()
    {
        return forwardDirection;
    }

    public Vector3 GetRightDirection()
    {
        return rightDirection;
    }

    public Vector3 GetVelocity()
    {
        return controller.velocity;
    }

    private Vector3 CalculateGravityVelocity()
    {
        if (groundCheck.IsGrounded())
        {
            // reset the fall timeout timer
            fallTimeoutTimer = fallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (gravityVelocity < 0.0f)
            {
                gravityVelocity = GROUNDED_VERTICAL_VELOCITY;
            }
        }
        else
        {
            // fall timeout
            if (fallTimeoutTimer >= 0.0f)
            {
                fallTimeoutTimer -= Time.deltaTime;
                gravityVelocity = 0;
            }
            else
            {
                // apply gravity over time under terminal velocity (multiply by delta time twice to linearly speed up over time)
                if (Mathf.Abs(gravityVelocity) < terminalFallingVelocity)
                {
                    gravityVelocity += gravity * Time.deltaTime;
                }

            }
        }

        return new Vector3(0, gravityVelocity, 0);
    }
}
