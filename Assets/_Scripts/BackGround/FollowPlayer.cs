using System;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _followThresholdX;
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
        if (_playerTransform.position.x > _transform.position.x + _followThresholdX || _playerTransform.position.x < _transform.position.x - _followThresholdX)
        {
            Vector3 _targetPos = new Vector3(_playerTransform.position.x+_followOffsetX, 5.5f, 5f);
            _transform.position = Vector3.Lerp(_transform.position, _targetPos, 1f * Time.deltaTime);
        }else if (_playerTransform.position.x < _transform.position.x - _followThresholdX)
        {
            Vector3 _targetPos = new Vector3(_playerTransform.position.x-_followOffsetX, 5.5f, 5f);
            _transform.position = Vector3.Lerp(_transform.position, _targetPos, 1f * Time.deltaTime);
        }
       
    }
}
