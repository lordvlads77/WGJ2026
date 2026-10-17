using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputSystem_Actions _inputActions;

    private InputAction _verticalMoveAction;
    private InputAction _horizontalMoveAction;
    
    private float _verticalMoveDirection;
    private float _horizontalMoveDirection;
    
    private bool _isMovingVertically;
    private bool _isMovingHorizontally;

    private Action<bool, float> _onVerticalMoveAction;
    private Action<bool, float> _onHorizontalMoveAction;
    
    void Awake()
    {
        _onHorizontalMoveAction = PlayerEvents.RaiseHorizontalMovePerformed;
        _onVerticalMoveAction = PlayerEvents.RaiseVerticalMovePerformed;
        _inputActions = new InputSystem_Actions();
        _verticalMoveAction = _inputActions.Player.VerticalMovement;
        _horizontalMoveAction = _inputActions.Player.HorizontalMovement;
    }

    private void Update()
    {
        Debug.Log(_verticalMoveDirection);
        Debug.Log(_horizontalMoveDirection);
    }

    void OnEnable()
    {
        
        _inputActions.Enable();
        _verticalMoveAction.performed += context => OnVerticalMove(_isMovingVertically, context.ReadValue<float>());
        _horizontalMoveAction.performed += context => OnHorizontalMove(_isMovingHorizontally, context.ReadValue<float>());
        _verticalMoveAction.canceled += context => OnVerticalMoveCancelled();
        _horizontalMoveAction.canceled += context => OnHorizontalMoveCancelled();
    }

    void OnDisable()
    {
        _inputActions.Disable();
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
    
}
