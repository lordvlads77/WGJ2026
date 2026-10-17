using System;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _followThresholdY;
    [SerializeField] private float _followOffsetX;

    private Transform _transform;
    

    private void Awake()
    { 
        _transform = gameObject.GetComponent<Transform>();
    }

    private void Update()
    {
        PlayerFollower();
    }

    private void PlayerFollower()
    {
        if (_playerTransform.position.y < _followThresholdY) return;
        _transform.position = new Vector3(_playerTransform.position.x,0,0) + new Vector3(_followOffsetX, 5, 5);
    }
}
