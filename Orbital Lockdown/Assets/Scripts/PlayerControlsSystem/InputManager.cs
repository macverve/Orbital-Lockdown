using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace OrbitalLockdown.Manager
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput PlayerInput;

        public Vector2 Move { get; private set; }

        public Vector2 Look { get; private set; }

        public bool Run { get; private set; }

        private InputActionMap _currentMap;

        private InputAction _moveAction;

        private InputAction _lookAction;

        private InputAction _runAction;

        private void Awake()
        {
            _currentMap = PlayerInput.currentActionMap;
            _moveAction = _currentMap.FindAction("Move");
            _lookAction = _currentMap.FindAction("Look");
            _runAction = _currentMap.FindAction("Run");


            _moveAction.performed += OnMove;
            _lookAction.performed += OnLook;
            _runAction.performed += OnRun;

            _moveAction.canceled += OnMove;
            _lookAction.canceled += OnLook;
            _runAction.canceled += OnRun;
        }

        private void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }


        private void OnMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }
        private void OnLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }   
        private void OnRun(InputAction.CallbackContext context)
        {
            Run = context.ReadValueAsButton();
        }

        private void OnEnable()
        {
            _currentMap.Enable();
        }

        private void OnDisable()
        {
            _currentMap.Disable();
        }

    }
}