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
        _onHorizontalMoveAction = PlayerEvents.RaiseHorizontalMovePerformed;
        _onVerticalMoveAction = PlayerEvents.RaiseVerticalMovePerformed;
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
        _verticalMoveAction.performed += context => OnVerticalMove(_isMovingVertically, context.ReadValue<float>());
        _horizontalMoveAction.performed += context => OnHorizontalMove(_isMovingHorizontally, context.ReadValue<float>());
        _verticalMoveAction.canceled += context => OnVerticalMoveCancelled();
        _horizontalMoveAction.canceled += context => OnHorizontalMoveCancelled();
        _moveAction.performed += context => OnMovePerformed(_isMoving, context.ReadValue<Vector2>());
        _moveAction.canceled += context => OnMoveCancelled();
        //_jumpAction.performed += OnJumpPerformed(_isJumping);
    }

    void OnDisable()
    {
        _inputActions.Disable();
        _verticalMoveAction.performed -= context => OnVerticalMove(_isMovingVertically, context.ReadValue<float>());
        _horizontalMoveAction.performed -= context => OnHorizontalMove(_isMovingHorizontally, context.ReadValue<float>());
        _verticalMoveAction.canceled -= context => OnVerticalMoveCancelled();
        _horizontalMoveAction.canceled -= context => OnHorizontalMoveCancelled();
        _moveAction.performed -= context => OnMovePerformed(_isMoving, context.ReadValue<Vector2>());
        _moveAction.canceled -= context => OnMoveCancelled();
    }

    private void OnVerticalMove(bool isMovingVertically, float direction)
    {
        _isMovingVertically = true;
        _verticalMoveDirection = direction;
        _onVerticalMoveAction?.Invoke(isMovingVertically, _verticalMoveDirection);
    }

    void OnVerticalMoveCancelled()
    {
        _isMovingVertically = false;
        _verticalMoveDirection = 0;
        _onVerticalMoveAction?.Invoke(false, _verticalMoveDirection);
    }

    void OnHorizontalMove(bool isMoving, float direction) 
    {
        _isMovingHorizontally = true;
        _horizontalMoveDirection = direction;
        _onHorizontalMoveAction?.Invoke(_isMovingHorizontally, _horizontalMoveDirection);
    }

    void OnHorizontalMoveCancelled()
    {
        _isMovingHorizontally = false;
        _horizontalMoveDirection = 0;
        _onHorizontalMoveAction?.Invoke(false, _horizontalMoveDirection);
    }

    void OnMovePerformed(bool isMoving, Vector2 direction)
    {
        _isMoving = true;
        _moveDirection = direction;
        _onMoveAction?.Invoke(_isMoving, _moveDirection);
    }

    void OnMoveCancelled()
    {
        _isMoving = false;
        _moveDirection = Vector2.zero;
        _onMoveAction?.Invoke(_isMoving, _moveDirection);
    }
    
    void OnJumpPerformed(bool isJumping)
    {
        
    }
    
}
