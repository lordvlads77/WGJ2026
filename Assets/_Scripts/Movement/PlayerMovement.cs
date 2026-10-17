using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private PlayerVariables _playerVariables;
    
    private Rigidbody _playerRigidbody;
    
    private float _moveSpeed;
   
    private Vector3 _moveDirection;

    private bool _isMoving;

    private void Update()
    {
        Debug.Log("Direction: " + _moveDirection);
    }

    private void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _moveSpeed = _playerVariables.BaseMoveSpeed;
    }

    private IEnumerator MovementCoroutine()
    {
        while (_isMoving)
        {
            _playerRigidbody.AddForce(_moveDirection * _moveSpeed, ForceMode.Force);
            yield return null;
        }
    }

    private void OnEnable()
    {
        PlayerEvents.OnMove += OnPlayerMovement;
    }

    private void OnDisable()
    {
        PlayerEvents.OnMove -= OnPlayerMovement;
    }

    void OnPlayerMovement(bool isMoving, Vector2 direction)
    {
        _isMoving = isMoving;
        _moveDirection = new Vector3(direction.x, 0, direction.y);
        StartCoroutine(MovementCoroutine());
    }
}
