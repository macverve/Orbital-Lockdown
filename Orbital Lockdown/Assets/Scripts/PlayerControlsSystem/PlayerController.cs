using UnityEngine;
using OrbitalLockdown.Manager;

namespace OrbitalLockdown.PlayerControl
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float AnimblendSpeed = 8.9f;
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;
        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float BottomLimit = 70f;
        [SerializeField] private float MosueSensitivity = 21.9f;

        private Rigidbody _playerRigidbody;
        private InputManager _inputManager;
        private Animator _animator;
        private bool _hasAnimator;

        private int _xVelHash;
        private int _yVelHash;

        private float _xRotation;
        private float _yawInput;

        private const float _walkSpeed = 2f;
        private const float _runSpeed = 3.5f;

        private Vector2 _currentVelocity;

        public void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            _playerRigidbody = GetComponent<Rigidbody>();
            _inputManager = GetComponent<InputManager>();

            _xVelHash = Animator.StringToHash("X_Velocity");
            _yVelHash = Animator.StringToHash("Y_Velocity");
        }

        private void FixedUpdate()
        {
            Move();
            RotatePlayer();
        }

        private void LateUpdate()
        {
            CamMovements();
        }

        private void Move()
        {
            if (!_hasAnimator) return;

            float targetSpeed = _inputManager.Run ? _runSpeed : _walkSpeed;

            if (_inputManager.Move == Vector2.zero)
                targetSpeed = 0.1f;

            _currentVelocity.x = Mathf.Lerp(
                _currentVelocity.x,
                _inputManager.Move.x * targetSpeed,
                AnimblendSpeed * Time.fixedDeltaTime
            );

            _currentVelocity.y = Mathf.Lerp(
                _currentVelocity.y,
                _inputManager.Move.y * targetSpeed,
                AnimblendSpeed * Time.fixedDeltaTime
            );

            float xVelDifference = _currentVelocity.x - _playerRigidbody.linearVelocity.x;
            float zVelDifference = _currentVelocity.y - _playerRigidbody.linearVelocity.z;

            _playerRigidbody.AddForce(
                transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)),
                ForceMode.VelocityChange
            );

            _animator.SetFloat(_xVelHash, _currentVelocity.x);
            _animator.SetFloat(_yVelHash, _currentVelocity.y);
        }

        private void CamMovements()
        {
            if (!_hasAnimator) return;
            if (Camera == null) return;

            float mouseX = _inputManager.Look.x;
            float mouseY = _inputManager.Look.y;

            _yawInput = mouseX;

            if (CameraRoot != null)
                Camera.position = CameraRoot.position;

            _xRotation -= mouseY * MosueSensitivity * Time.deltaTime;
            _xRotation = Mathf.Clamp(_xRotation, UpperLimit, BottomLimit);

            Camera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }

        private void RotatePlayer()
        {
            float yawAmount = _yawInput * MosueSensitivity * Time.fixedDeltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0f, yawAmount, 0f);
            _playerRigidbody.MoveRotation(_playerRigidbody.rotation * deltaRotation);
        }
    }
}