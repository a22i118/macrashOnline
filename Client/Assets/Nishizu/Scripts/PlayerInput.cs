using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInput : MonoBehaviour
{
    private @PlayerInputActions _inputActions;
    private Vector2 _inputMovement = new Vector2();
    private bool _isPickUp = false;
    private bool _isThrow = false;
    private bool _isJump = false;
    private bool _isTriggerPressed = false;
    public Vector2 InputMovement { get { return _inputMovement; } }
    public bool IsThrow { get { return _isThrow; } }
    public bool IsJump { get { return _isJump; } }
    public bool IsPickUp { get { return _isPickUp; } }

    public void Reset() { _isJump = _isThrow = false; }

    private void OnMove(InputAction.CallbackContext context)
    {
        _inputMovement = context.ReadValue<Vector2>();
    }

    private void OnThrow(InputAction.CallbackContext context)
    {
        float triggerValue = context.action.ReadValue<float>();

        if (triggerValue > 0.0f)
        {
            _isThrow = true;
        }
        else
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
        float triggerValue = context.action.ReadValue<float>();
        if (triggerValue > 0.0f)
        {
            _isPickUp = true;
        }
        else
        {
            _isPickUp = false;
        }
    }

    private void Awake()
    {
        _inputActions = new @PlayerInputActions();

        _inputActions.Player.Move.started += OnMove;
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;

        _inputActions.Player.Throw.started += OnThrow;
        _inputActions.Player.Throw.canceled += OnThrow;

        _inputActions.Player.Jump.started += OnJump;
        _inputActions.Player.Jump.canceled += OnJump;

        _inputActions.Player.PickUp_Catch_WakeUp.started += OnPickUp_Catch_WakeUp;
        _inputActions.Player.PickUp_Catch_WakeUp.canceled += OnPickUp_Catch_WakeUp;

        _inputActions.Enable();
    }

    private void OnDestroy()
    {
        _inputActions?.Dispose();
    }
}
