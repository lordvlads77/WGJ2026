using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private PlayerVariables _playerVariables;

    private Rigidbody _playerRigidbody;
    private float _moveSpeed;
    private Vector3 _moveDirection;

    private void Update()
    {
        // Debug.Log("Direction: " + _moveDirection);
    }

    private void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _moveSpeed = _playerVariables.BaseMoveSpeed;
    }

    private void OnEnable()
    {
        PlayerEvents.OnMove += OnPlayerMovement;
    }

    private void OnDisable()
    {
        PlayerEvents.OnMove -= OnPlayerMovement;
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        Vector3 velocity = _playerRigidbody.linearVelocity;

        velocity.x = _moveDirection.x * _moveSpeed;
        velocity.z = _moveDirection.z * _moveSpeed;

        _playerRigidbody.linearVelocity = velocity;
    }

    void OnPlayerMovement(bool isMoving, Vector2 direction)
    {
        _moveDirection = new Vector3(direction.x, 0f, direction.y);
    }
}
