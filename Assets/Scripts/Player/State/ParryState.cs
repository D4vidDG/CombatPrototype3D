// using StarterAssets;
// using UnityEngine;

// public class ParryState : MonoBehaviour, State
// {
//     [SerializeField] float slowTimeScale;
//     [SerializeField] float totalTime;
//     [SerializeField] float rollSpeed;
//     [SerializeField] float attackCooldown;

//     RollState rollState;
//     MovementState movementState;
//     AttackState attackState;

//     State currentState;

//     float timer;

//     void Awake()
//     {
//         movementState = GetComponent<MovementState>();
//         rollState = GetComponent<RollState>();
//         attackState = GetComponent<AttackState>();
//     }

//     public void Enter(Player player)
//     {
//         currentState = rollState;
//         Time.timeScale = slowTimeScale;
//         timer = totalTime;
//         attackState.EnableCooldown(false);
//         rollState.Enter(player);
//     }

//     public void Exit(Player player)
//     {
//         Time.timeScale = 1;
//         attackState.EnableCooldown(true);
//     }

//     public State UpdateState(Player player, PlayerInputs input)
//     {
//         timer -= Time.deltaTime;

//         //disable inputs
//         if (currentState is MovementState)
//         {
//             input.aim = false;
//             input.roll = false;
//             input.backpack = false;
//             input.move = Vector2.zero;
//         }
//         else if (currentState is AttackState)
//         {
//             input.roll = false;
//             input.backpack = false;
//         }


//         State nextState = currentState.UpdateState(player, input);
//         if (nextState != null)
//         {
//             currentState.Exit(player);
//             currentState = nextState;
//             currentState.Enter(player);
//         }

//         if (timer < 0)
//         {
//             currentState.Exit(player);
//             return movementState;
//         }

//         return null;
//     }

//     public void LateUpdateState(Player player)
//     {
//     }
// }