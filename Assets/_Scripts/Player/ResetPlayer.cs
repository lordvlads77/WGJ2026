using System;
using UnityEngine;

public class ResetPlayer : MonoBehaviour
{
    private Vector3 _initialPosition;
    private Transform _playerTransform;
    
    private void Awake()
    {
        _playerTransform = GetComponent<Transform>();
        _initialPosition = transform.position;
    }

    private void Start()
    {
        _playerTransform.position = _initialPosition;
    }

    public void PlayerReset()
    {
        transform.position = _initialPosition;
    }
}
