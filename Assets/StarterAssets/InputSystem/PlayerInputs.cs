using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class PlayerInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public bool roll;
		public bool interact;
		public bool attack;
		public bool backpack;
		public bool aim;
		public Vector2 mousePosition;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;

		[Header("Aiming")]
		[SerializeField] Collider aimingPlane;

		private bool inputEnabled = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputAction.CallbackContext context)
		{
			MoveInput(context.ReadValue<Vector2>());
		}

		public void OnRoll(InputAction.CallbackContext context)
		{
			RollInput(context.performed);
		}

		public void OnInteract(InputAction.CallbackContext context)
		{
			InteractInput(context.performed);
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			AttackInput(context.performed);
		}

		public void OnBackpack(InputAction.CallbackContext context)
		{
			BackpackInput(context.performed);
		}

		public void OnAim(InputAction.CallbackContext context)
		{
			AimInput(context.performed);
		}

		public void OnMouseMove(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				mousePosition = context.ReadValue<Vector2>();
			}
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			if (!inputEnabled) move = Vector2.zero;
			else move = newMoveDirection;
		}


		public void RollInput(bool newState)
		{
			if (!inputEnabled) roll = false;
			roll = newState;
		}

		public void InteractInput(bool newState)
		{
			if (!inputEnabled) interact = false;
			interact = newState;
		}

		public void AttackInput(bool newState)
		{
			if (!inputEnabled) attack = false;
			attack = newState;
		}

		public void BackpackInput(bool newState)
		{
			if (!inputEnabled) backpack = false;
			backpack = newState;
		}

		public void AimInput(bool newState)
		{
			if (!inputEnabled) aim = false;
			aim = newState;
		}

		public void EnableInput(bool enable)
		{
			inputEnabled = enable;
			if (!enable)
			{
				move = Vector2.zero;
				roll = false;
				interact = false;
				attack = false;
			}
		}



		public Vector3 GetAimingDirectionTowardsMouse()
		{
			Ray ray = Camera.main.ScreenPointToRay(mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray);
			foreach (RaycastHit hit in hits)
			{
				if (hit.collider == aimingPlane)
				{
					Vector3 direction = hit.point - transform.position;
					direction.y = 0;
					return direction;
				}
			}

			return Vector3.zero;
		}
	}

}