using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        //         [Header("Player")]
        //         [Tooltip("Move speed of the character in m/s")]
        //         public float MoveSpeed = 2.0f;

        //         [Tooltip("How fast player turns")]
        //         public float RotationSpeed = 1f;

        //         [Tooltip("Acceleration and deceleration")]
        //         public float SpeedChangeRate = 10.0f;

        //         [Space(10)]
        //         [Tooltip("The height the player can jump")]
        //         public float JumpHeight = 1.2f;

        //         [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        //         public float Gravity = -15.0f;

        //         [Space(10)]
        //         [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        //         public float JumpTimeout = 0.50f;

        //         [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        //         public float FallTimeout = 0.15f;


        //         [Header("Cinemachine")]
        //         [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        //         public GameObject CinemachineCameraTarget;

        //         [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        //         public float CameraOverridePitchAngle = 0.0f;

        //         public float TopPitchLimit = 0.0f;

        //         public float BottomPitchLimit = 0.0f;

        //         [Tooltip("For locking the camera position on all axis")]
        //         public bool LockCameraPosition = false;

        //         // player
        //         private float _speed;
        //         private Vector2 _animationBlend;
        //         private float _targetYaw = 0.0f;
        //         private float _targetPitch = 0.0f;
        //         private float _verticalVelocity;
        //         private float _terminalVelocity = 53.0f;

        //         // timeout deltatime
        //         private float _jumpTimeoutDelta;
        //         private float _fallTimeoutDelta;

        //         // animation IDs
        //         private int _animIDSpeedX;
        //         private int _animIDSpeedZ;
        //         private int _animIDSpeed;
        //         private int _animIDGrounded;
        //         private int _animIDJump;
        //         private int _animIDFreeFall;
        //         private int _animIDMotionSpeed;

        // #if ENABLE_INPUT_SYSTEM 
        //         private PlayerInput _playerInput;
        // #endif
        //         private Animator _animator;
        //         private CharacterController _controller;
        //         private PlayerInputs _input;
        //         private GameObject _mainCamera;
        //         private GroundCheck _groundCheck;


        //         private const float _threshold = 0.01f;

        //         private bool _hasAnimator;

        //         private bool IsCurrentDeviceMouse
        //         {
        //             get
        //             {
        // #if ENABLE_INPUT_SYSTEM
        //                 return _playerInput.currentControlScheme == "KeyboardMouse";
        // #else
        //                 return false;
        // #endif
        //             }
        //         }


        //         private void Awake()
        //         {
        //             // get a reference to our main camera
        //             if (_mainCamera == null)
        //             {
        //                 _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        //             }
        //         }

        //         private void Start()
        //         {
        //             _hasAnimator = TryGetComponent(out _animator);
        //             _controller = GetComponent<CharacterController>();
        //             _input = GetComponent<PlayerInputs>();
        //             _groundCheck = GetComponentInChildren<GroundCheck>();
        // #if ENABLE_INPUT_SYSTEM
        //             _playerInput = GetComponent<PlayerInput>();
        // #else
        //             Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
        // #endif

        //             AssignAnimationIDs();

        //             // reset our timeouts on start
        //             _jumpTimeoutDelta = JumpTimeout;
        //             _fallTimeoutDelta = FallTimeout;

        //             //set initial camera pitch
        //             CinemachineCameraTarget.transform.Rotate(CameraOverridePitchAngle, 0, 0);

        //         }

        //         private void Update()
        //         {
        //             _hasAnimator = TryGetComponent(out _animator);

        //             JumpAndGravity();
        //             Move();
        //         }

        //         private void LateUpdate()
        //         {
        //             CameraRotation();
        //         }

        //         private void AssignAnimationIDs()
        //         {
        //             _animIDSpeedX = Animator.StringToHash("SpeedX");
        //             _animIDSpeedZ = Animator.StringToHash("SpeedZ");
        //             _animIDSpeed = Animator.StringToHash("Speed");
        //             _animIDGrounded = Animator.StringToHash("Grounded");
        //             _animIDJump = Animator.StringToHash("Jump");
        //             _animIDFreeFall = Animator.StringToHash("FreeFall");
        //             _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        //         }


        //         private void CameraRotation()
        //         {
        //             // if there is an input and camera position is not fixed
        //             if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        //             {
        //                 //Don't multiply mouse input by Time.deltaTime;
        //                 float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

        //                 float deltaYaw = _input.look.x * deltaTimeMultiplier;
        //                 float deltaPitch = _input.look.y * deltaTimeMultiplier;

        //                 _targetYaw = transform.eulerAngles.y + deltaYaw * RotationSpeed;
        //                 float currentPitch = CinemachineCameraTarget.transform.eulerAngles.x;
        //                 if (currentPitch > 180) currentPitch = currentPitch - 360;
        //                 _targetPitch = currentPitch + deltaPitch * RotationSpeed;
        //                 _targetPitch = Mathf.Max(Mathf.Min(_targetPitch, -BottomPitchLimit), -TopPitchLimit);

        //                 transform.rotation = Quaternion.Euler(0.0f, _targetYaw, 0.0f);
        //                 CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_targetPitch, 0.0f, 0.0f);
        //             }
        //         }

        //         private void Move()
        //         {
        //             float targetSpeed = MoveSpeed;

        //             // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        //             // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        //             // if there is no input, set the target speed to 0
        //             if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        //             // a reference to the players current horizontal velocity
        //             float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        //             float speedOffset = 0.1f;
        //             float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        //             // accelerate or decelerate to target speed
        //             if (currentHorizontalSpeed < targetSpeed - speedOffset ||
        //                 currentHorizontalSpeed > targetSpeed + speedOffset)
        //             {
        //                 // creates curved result rather than a linear one giving a more organic speed change
        //                 // note T in Lerp is clamped, so we don't need to clamp our speed
        //                 _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
        //                     Time.deltaTime * SpeedChangeRate);

        //                 // round speed to 3 decimal places
        //                 _speed = Mathf.Round(_speed * 1000f) / 1000f;
        //             }
        //             else
        //             {
        //                 _speed = targetSpeed;
        //             }

        //             _animationBlend = Vector2.Lerp(_animationBlend, targetSpeed * _input.move.normalized, Time.deltaTime * SpeedChangeRate);
        //             if (_animationBlend.magnitude < 0.01f) _animationBlend = Vector2.zero;

        //             Vector3 targetDirection = Camera.main.transform.forward * _input.move.y + Camera.main.transform.right * _input.move.x;

        //             // move the player
        //             _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
        //                              new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        //             // update animator if using character
        //             if (_hasAnimator)
        //             {
        //                 _animator.SetFloat(_animIDSpeedX, _animationBlend.x);
        //                 _animator.SetFloat(_animIDSpeedZ, _animationBlend.y);
        //                 _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        //                 _animator.SetFloat(_animIDSpeed, currentHorizontalSpeed);
        //             }
        //         }

        //         private void JumpAndGravity()
        //         {
        //             if (_groundCheck.IsGrounded())
        //             {
        //                 // reset the fall timeout timer
        //                 _fallTimeoutDelta = FallTimeout;

        //                 // update animator if using character
        //                 if (_hasAnimator)
        //                 {
        //                     _animator.SetBool(_animIDJump, false);
        //                     _animator.SetBool(_animIDFreeFall, false);
        //                 }

        //                 // stop our velocity dropping infinitely when grounded
        //                 if (_verticalVelocity < 0.0f)
        //                 {
        //                     _verticalVelocity = -2f;
        //                 }

        //                 // Jump
        //                 if (_input.roll && _jumpTimeoutDelta <= 0.0f)
        //                 {
        //                     // the square root of H * -2 * G = how much velocity needed to reach desired height
        //                     _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

        //                     // update animator if using character
        //                     if (_hasAnimator)
        //                     {
        //                         _animator.SetBool(_animIDJump, true);
        //                     }
        //                 }

        //                 // jump timeout
        //                 if (_jumpTimeoutDelta >= 0.0f)
        //                 {
        //                     _jumpTimeoutDelta -= Time.deltaTime;
        //                 }
        //             }
        //             else
        //             {
        //                 // reset the jump timeout timer
        //                 _jumpTimeoutDelta = JumpTimeout;

        //                 // fall timeout
        //                 if (_fallTimeoutDelta >= 0.0f)
        //                 {
        //                     _fallTimeoutDelta -= Time.deltaTime;
        //                 }
        //                 else
        //                 {
        //                     // update animator if using character
        //                     if (_hasAnimator)
        //                     {
        //                         _animator.SetBool(_animIDFreeFall, true);
        //                     }
        //                 }

        //                 // if we are not grounded, do not jump
        //                 _input.roll = false;
        //             }

        //             // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        //             if (_verticalVelocity < _terminalVelocity)
        //             {
        //                 _verticalVelocity += Gravity * Time.deltaTime;
        //             }

        //             if (_hasAnimator)
        //             {
        //                 _animator.SetBool(_animIDGrounded, _groundCheck.IsGrounded());
        //             }
        //         }

        //         private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        //         {
        //             if (lfAngle < -360f) lfAngle += 360f;
        //             if (lfAngle > 360f) lfAngle -= 360f;
        //             return Mathf.Clamp(lfAngle, lfMin, lfMax);
        //         }

        //         private void OnJumpStart()
        //         {
        //             AudioManager.instance.PlaySound(SoundName.PlayerJump);
        //         }

        //         private void OnLand(AnimationEvent animationEvent)
        //         {
        //             if (animationEvent.animatorClipInfo.weight > 0.5f)
        //             {
        //                 AudioManager.instance.PlaySoundAtPosition(SoundName.PlayerLand, transform.TransformPoint(_controller.center));
        //             }
        //         }
    }
}