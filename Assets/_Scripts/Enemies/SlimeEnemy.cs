using DG.Tweening;
using UnityEngine;

public class SlimeEnemy : MonoBehaviour
{
    [SerializeField] private MovementTypes _movementType;
    [SerializeField] private float _moveDistance;
    [SerializeField] private float _duration;
    [SerializeField] private Ease _easeType;
    [SerializeField] private Transform _visual;
    [SerializeField] private float _hopHeight = 0.15f;
    [SerializeField] private int _hopsPerTrip = 4;
    [SerializeField, Range(0f, 0.3f)] private float _squashAmount = 0.08f;

    private Tween _movementTween;
    private Sequence _hopSequence;
    private Transform _enemyTransform;

    private void Awake()
    {
        _enemyTransform = GetComponent<Transform>();
        if (_visual == null) _visual = _enemyTransform;
    }

    void Start()
    {
        MoveEnemy();
        Hop();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.collider.TryGetComponent(out ResetPlayer resetPlayer))
        {
            resetPlayer.PlayerReset();
        }
    }

    private void OnDisable()
    {
        _movementTween?.Kill();
        _hopSequence?.Kill();
    }

    private void MoveEnemy()
    {
        switch (_movementType)
        {
            case MovementTypes.Horizontal:
                _movementTween = _enemyTransform.DOMoveX(_enemyTransform.position.x + _moveDistance, _duration);
                break;
            case MovementTypes.Forward:
                _movementTween = _enemyTransform.DOMoveZ(_enemyTransform.position.z + _moveDistance, _duration);
                break;
        }

        _movementTween?
            .SetEase(_easeType)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }

    private void Hop()
    {
        if (_hopHeight <= 0f) return;

        float cycle = _duration / _hopsPerTrip;
        float squashTime = _squashAmount > 0f ? cycle * 0.2f : 0f;
        float airTime = cycle - squashTime;

        float baseY = _visual.localPosition.y;
        Vector3 baseScale = _visual.localScale;
        Vector3 squashScale = Vector3.Scale(baseScale, new Vector3(1f + _squashAmount, 1f - _squashAmount, 1f + _squashAmount));

        _hopSequence = DOTween.Sequence()
            .Append(_visual.DOLocalMoveY(baseY + _hopHeight, airTime * 0.5f).SetEase(Ease.OutQuad))
            .Append(_visual.DOLocalMoveY(baseY, airTime * 0.5f).SetEase(Ease.InQuad));

        if (squashTime > 0f)
        {
            _hopSequence
                .Append(_visual.DOScale(squashScale, squashTime * 0.5f).SetEase(Ease.OutQuad))
                .Append(_visual.DOScale(baseScale, squashTime * 0.5f).SetEase(Ease.OutBack));
        }

        _hopSequence
            .SetLoops(-1, LoopType.Restart)
            .SetLink(gameObject);
    }
}

public enum MovementTypes
{
    Horizontal,
    Forward
}