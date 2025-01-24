using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInput : MonoBehaviour
{
    private @PlayerInputActions _inputActions;
    private Vector2 _inputValue = new Vector2();
    private bool _isFire = false;
    private bool _isJump = false;
    private bool _isChangeCharacter = false;

    public Vector3 InputValue { get { return _inputValue; } }
    public bool IsFire { get { return _isFire; } }
    public bool IsJump { get { return _isJump; } }
    public bool IsChangeCharacter { get { return _isChangeCharacter; } }

    public void Reset() { _isJump = _isFire = _isChangeCharacter = false; }

    private void OnMove(InputAction.CallbackContext context)
    {
        _inputValue = context.ReadValue<Vector2>();
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        _isFire = true;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        _isJump = true;
    }

    private void OnChangeCharacter(InputAction.CallbackContext context)
    {
        _isChangeCharacter = true;
    }

    private void Awake()
    {
        _inputActions = new @PlayerInputActions();

        _inputActions.Player.Move.started += OnMove;
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;

        _inputActions.Player.Throw.performed += OnFire;

        _inputActions.Player.Jump.performed += OnJump;

        // _inputActions.Player.ChangeCharacter.performed += OnChangeCharacter;

        _inputActions.Enable();
    }

    private void OnDestroy()
    {
        _inputActions?.Dispose();
    }
}
