using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private PlayerVariables _playerVariables;
    
    private Rigidbody _playerRigidbody;
    
    private Action<bool, float> _onVerticalMoveAction;
    private Action<bool, float> _onHorizontalMoveAction;

    private float _moveSpeed;
    private float _horizontalMoveDirection;
    private float _verticalMoveDirection;
    
    private void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _moveSpeed = _playerVariables.BaseMoveSpeed;
        _onHorizontalMoveAction = PlayerEvents.RaiseHorizontalMovePerformed;
        _onVerticalMoveAction = PlayerEvents.RaiseVerticalMovePerformed;
        
    }

    private void Start()
    {
       
    }

    private void OnEnable()
    {
        _onHorizontalMoveAction += OnHorizontalMovement;
        _onHorizontalMoveAction += OnVerticalMovement;
    }

    private void OnDisable()
    {
        _onHorizontalMoveAction -= OnHorizontalMovement;
        _onHorizontalMoveAction -= OnVerticalMovement;
    }

    private void OnHorizontalMovement(bool isMoving, float direction)
    {
        Debug.Log("horizontal direction: "+direction);
        _horizontalMoveDirection = direction;
        if (isMoving)
        {
            
            _playerRigidbody.AddForce(new Vector3(direction, 0, 0),ForceMode.Force);
                        
        }
    }
    
    private void OnVerticalMovement(bool isMoving, float direction)
    {
        Debug.Log("vertical direction: "+direction);
        _verticalMoveDirection = direction;
        if (isMoving)
        {
            _playerRigidbody.AddForce(new Vector3(0, 0, direction), ForceMode.Force);
        }
    }
}
