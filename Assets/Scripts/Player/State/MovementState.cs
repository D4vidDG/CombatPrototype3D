using StarterAssets;
using UnityEngine;

public class MovementState : MonoBehaviour, State
{

    //constants
    const float SPEED_TOLERANCE = 0.1f;

    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float walkingSpeed = 2.0f;

    [Tooltip("How fast player turns")]
    public float rotationChangeRate = 1f;

    [Tooltip("Acceleration and deceleration")]
    public float speedChangeRate = 10.0f;

    // player
    private float speed;
    private Vector2 previousNonZeroInput;

    //states
    RollState rollState;
    AttackState attackState;
    BackpackState backpackState;
    ShootState shootingState;

    private void Awake()
    {
        rollState = GetComponent<RollState>();
        attackState = GetComponent<AttackState>();
        backpackState = GetComponent<BackpackState>();
        shootingState = GetComponent<ShootState>();
    }

    public void Enter(Player player)
    {
    }

    public void Exit(Player player)
    {
    }

    public State UpdateState(Player player, PlayerInputs input)
    {
        if (attackState.IsCounterAttackActive())
        {
            if (input.attack)
            {
                return attackState;
            }
            else
            {
                return null;
            }
        }

        if (input.attack)
        {
            return attackState;
        }
        else if (input.roll)
        {
            return rollState;
        }
        else if (input.backpack)
        {
            return backpackState;
        }
        else if (input.aim)
        {
            return shootingState;
        }
        else
        {
            Move(player, input);
            RotateTowardsMovement(player, input);
            return null;
        }
    }

    private void Move(Player player, PlayerInputs input)
    {
        Vector3 horizontalVelocity = GetMovementVelocity(player, input, walkingSpeed);
        player.mover.SetHorizontalVelocity(horizontalVelocity);
    }

    public Vector3 GetMovementVelocity(Player player, PlayerInputs input, float maxSpeed)
    {
        float targetSpeed = input.move != Vector2.zero ? maxSpeed : 0.0f;
        // a reference to the players current horizontal velocity
        Vector3 playerVelocity = player.mover.GetVelocity();
        float currentHorizontalSpeed = new Vector3(playerVelocity.x, 0.0f,
             playerVelocity.z).magnitude;

        //if input is analog, scale input
        if (input.analogMovement)
        {
            targetSpeed *= input.move.magnitude;
        }

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - SPEED_TOLERANCE ||
            currentHorizontalSpeed > targetSpeed + SPEED_TOLERANCE)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                Time.deltaTime * speedChangeRate);

            // round speed to 3 decimal places
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        Vector3 movementDirection = (previousNonZeroInput.y * player.mover.GetForwardDirection() +
             previousNonZeroInput.x * player.mover.GetRightDirection()).normalized;

        if (input.move.magnitude > 0.01f)
        {
            previousNonZeroInput = input.move;
        }

        return movementDirection * speed;
    }

    private void RotateTowardsMovement(Player player, PlayerInputs input)
    {
        //rotate towards movement direction
        Vector3 inputDirection = (player.mover.GetForwardDirection() * input.move.y +
             player.mover.GetRightDirection() * input.move.x).normalized;

        if (inputDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(inputDirection, transform.up),
                Time.deltaTime * rotationChangeRate
                );
        }
    }

    public void LateUpdateState(Player player)
    {
    }
}