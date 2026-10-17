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
    
    private Action<bool, float> _onVerticalMoveAction;
    private Action<bool, float> _onHorizontalMoveAction;
    private Action<bool, Vector2> _onMoveAction;
    private Action<bool> _onJumpAction;
    
    void Awake()
    {
        _onJumpAction = PlayerEvents.RaiseJumpPerformed;
        _onMoveAction = PlayerEvents.RaiseMovePerformed;
        _inputActions = new InputSystem_Actions();
        _verticalMoveAction = _inputActions.Player.VerticalMovement;
        _horizontalMoveAction = _inputActions.Player.HorizontalMovement;
        _moveAction = _inputActions.Player.Movement;
        _jumpAction = _inputActions.Player.Jump;
    }

    private void Update()
    {
        //Debug.Log(_verticalMoveDirection);
        //Debug.Log(_horizontalMoveDirection);
    }

    void OnEnable()
    {

        _inputActions.Enable();

        _moveAction.performed += OnMovePerformed;
        _moveAction.canceled += OnMoveCancelled;
        //_jumpAction.performed += OnJumpPerformed(_isJumping);
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

    void OnJumpPerformed(bool isJumping)
    {
        
    }
    
}
