using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private PlayerVariables _playerVariables;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private bool _canSlowBackwards;
    [SerializeField] private bool _canMoveRight;
    
    private Rigidbody _playerRigidbody;
    private float _moveSpeed;
    private Vector3 _moveDirection;
    private bool _isGrounded = true;
    private RaycastHit _hit;
    private float _playerHeight;
    private CapsuleCollider _capsuleCollider;
    private float _rayCastDistance;
    private float _groundCheckTimer = 0f;
    private float _groundCheckDelay = 0.3f;
    

    private void Update()
    {
        // Debug.Log("Direction: " + _moveDirection);
    }

    private void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _playerRigidbody = GetComponent<Rigidbody>();
        _moveSpeed = _playerVariables.BaseMoveSpeed;
    }

    private void Start()
    {
        _playerHeight = _capsuleCollider.height * transform.localScale.y;
        _rayCastDistance = (_playerHeight / 2) + .2f;
    }

    private void OnEnable()
    {
        PlayerEvents.OnMove += OnPlayerMovement;
        PlayerEvents.OnJump += OnPlayerJump;
    }

    private void OnDisable()
    {
        PlayerEvents.OnMove -= OnPlayerMovement;
        PlayerEvents.OnJump -= OnPlayerJump;
    }

    private void FixedUpdate()
    {
        MoveCharacter();
        GroundCheck();
    }

    private void MoveCharacter()
    {
        Vector3 velocity = _playerRigidbody.linearVelocity;

        if (_moveDirection.x > 0f && _canMoveRight)
        {
            velocity.x = 0f;
        }
        else if (_moveDirection.x < 0f)
        {
            float speed = _canSlowBackwards ? _moveSpeed * _playerVariables.SlowdownMultiplier : _moveSpeed;
            velocity.x = _moveDirection.x * speed;
        }
        else
        {
            velocity.x = _moveDirection.x * _moveSpeed;
        }

        velocity.z = _moveDirection.z * _moveSpeed;
        _playerRigidbody.linearVelocity = velocity;
    }

    void OnPlayerMovement(bool isMoving, Vector2 direction)
    {
        _moveDirection = new Vector3(direction.x, 0f, direction.y);
    }

    private void OnPlayerJump(bool isJumping)
    {
        if (!_isGrounded) return;
        _isGrounded = false;
        _groundCheckTimer = _groundCheckDelay;
        _playerRigidbody.AddForce(Vector3.up*_playerVariables.JumpForce, ForceMode.Impulse);
    }
    
    private void GroundCheck()
    {
        if (!_isGrounded && _groundCheckTimer <= 0f)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            _isGrounded = Physics.Raycast(rayOrigin, Vector3.down, _rayCastDistance, _groundLayer);
            Debug.DrawRay(rayOrigin, Vector3.down * _rayCastDistance, Color.red);
        }
        else
        {
            _groundCheckTimer -= Time.fixedDeltaTime;
        }
    }

   
}
