using System;
using DG.Tweening;
using UnityEngine;

public class SlimeEnemy : MonoBehaviour
{
    [SerializeField] private MovementDirections moveDirections;
    [SerializeField] private Ease _easeType;
    [SerializeField]private float _travelDistance;
    [SerializeField] private float _duration;
    private Tween _movementTween;
    private Vector3 _initialPosition;

    private Transform _enemyTransform;

    private void Awake()
    {
        _enemyTransform = GetComponent<Transform>();
    }

    void Start()
    {
        DoMovement();
    }


    private void OnDisable()
    {
        _movementTween.Kill();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.collider.TryGetComponent(out ResetPlayer _resetPlayer);
            _resetPlayer.PlayerReset();
        }
    }

    void DoMovement()
    {
        switch (moveDirections)
        {
            case MovementDirections.Horizontal:
                _movementTween = _enemyTransform.DOMoveX(_initialPosition.x + _travelDistance, _duration)
                    .SetEase(_easeType).SetLoops(-1, LoopType.Yoyo);
                break;
            case MovementDirections.Forward:
                _movementTween = _enemyTransform.DOMoveZ(_initialPosition.z + _travelDistance, _duration)
                    .SetEase(_easeType).SetLoops(-1, LoopType.Yoyo);
                break;
        }
    }
}

public enum MovementDirections
{
    Horizontal,
    Forward
}



