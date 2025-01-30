using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInput : MonoBehaviour
{
    private @PlayerInputActions _inputActions;
    private Vector2 _inputValue = new Vector2();
    private bool _isPickUp = false;
    private bool _isThrow = false;
    private bool _isJump = false;
    public Vector2 InputValue { get { return _inputValue; } }
    public bool IsThrow { get { return _isThrow; } }
    public bool IsJump { get { return _isJump; } }
    public bool IsPickUp { get { return _isPickUp; } }

    private void OnMove(InputAction.CallbackContext context)
    {
        _inputValue = context.ReadValue<Vector2>();
    }

    private void OnThrow(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isThrow = true;
        }
        else if (context.canceled)
        {
            _isThrow = false;
        }
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isJump = true;
        }
        else if (context.canceled)
        {
            _isJump = false;
        }
    }
    private void OnPickUp_Catch_WakeUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isPickUp = true;
        }
        else if (context.canceled)
        {
            _isPickUp = true;
        }
    }

    private void Awake()
    {
        _inputActions = new @PlayerInputActions();

        _inputActions.Player.Move.started += OnMove;
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;

        _inputActions.Player.Throw.performed += OnThrow;
        _inputActions.Player.Throw.canceled += OnThrow;

        _inputActions.Player.Jump.started += OnJump;
        _inputActions.Player.Jump.canceled += OnJump;

        _inputActions.Player.PickUp_Catch_WakeUp.performed += OnPickUp_Catch_WakeUp;
        _inputActions.Player.PickUp_Catch_WakeUp.canceled += OnPickUp_Catch_WakeUp;

        _inputActions.Enable();
    }

    private void OnDestroy()
    {
        _inputActions?.Dispose();
    }
}
