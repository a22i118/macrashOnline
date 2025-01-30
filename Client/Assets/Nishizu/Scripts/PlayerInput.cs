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
        _isThrow = true;
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
        _isPickUp = true;
    }

    private void Awake()
    {
        _inputActions = new @PlayerInputActions();

        _inputActions.Player.Move.started += OnMove;
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;

        _inputActions.Player.Throw.performed += OnThrow;

        _inputActions.Player.Jump.performed += OnJump;
        _inputActions.Player.PickUp_Catch_WakeUp.performed += OnPickUp_Catch_WakeUp;

        _inputActions.Enable();
    }

    private void OnDestroy()
    {
        _inputActions?.Dispose();
    }
}
