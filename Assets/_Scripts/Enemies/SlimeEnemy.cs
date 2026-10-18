using DG.Tweening;
using UnityEngine;

public class SlimeEnemy : MonoBehaviour
{
    [SerializeField] private MovementTypes _movementType;
    [SerializeField] private float _moveDistance;
    [SerializeField] private float _duration;
    [SerializeField] private Ease _easeType;

    private Tween _movementTween;
    private Transform _enemyTransform;

    private void Awake()
    {
        _enemyTransform = GetComponent<Transform>();
    }

    void Start()
    {
        MoveEnemy();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.TryGetComponent(out ResetPlayer _resetPlayer);
            _resetPlayer.PlayerReset();
        }
    }

    private void OnDisable()
    {
        _movementTween.Kill();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MoveEnemy()
    {
        switch (_movementType)
        {
            case MovementTypes.Horizontal:
                _movementTween = _enemyTransform.DOMoveX(transform.position.x + _moveDistance, _duration).SetEase(_easeType)
                    .SetLoops(-1, LoopType.Yoyo);
                break;
            case MovementTypes.Forward:
                _movementTween = _enemyTransform.DOMoveZ(transform.position.x + _moveDistance, _duration).SetEase(_easeType)
                    .SetLoops(-1, LoopType.Yoyo);
                break;
        }
    }
}

public enum MovementTypes
{
    Horizontal,
    Forward
}