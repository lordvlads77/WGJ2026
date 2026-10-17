using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputSystem_Actions _inputActions;

    private InputAction _verticalMoveAction;
    private InputAction _horizontalMoveAction;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    
    private float _verticalMoveDirection;
    private float _horizontalMoveDirection;

    private Vector2 _moveDirection;
    
    private bool _isMovingVertically;
    private bool _isMovingHorizontally;
    private bool _isMoving = false;
    private bool _isJumping = false;
    
    
    void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _moveAction = _inputActions.Player.Movement;
        _jumpAction = _inputActions.Player.Jump;
    }

    void OnEnable()
    {

        _inputActions.Enable();

        _moveAction.performed += OnMovePerformed;
        _moveAction.canceled += OnMoveCancelled;
        _jumpAction.performed += OnJumpPerformed;
    }

    void OnDisable()
    {
        _inputActions.Disable();
        _moveAction.performed -= OnMovePerformed;
        _moveAction.canceled -= OnMoveCancelled;

        _inputActions.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();

        PlayerEvents.RaiseMovePerformed(true, direction);
    }

    private void OnMoveCancelled(InputAction.CallbackContext context)
    {
        PlayerEvents.RaiseMovePerformed(false, Vector2.zero);
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        PlayerEvents.RaiseJumpPerformed(true);
    }
    
}
