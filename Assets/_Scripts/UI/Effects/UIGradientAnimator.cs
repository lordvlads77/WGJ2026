using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(UIGradient))]
public sealed class UIGradientAnimator : MonoBehaviour
{
    [System.Serializable]
    private class TweenSettings
    {
        public bool Enabled;
        public float Duration = 2f;
        public Ease Ease = Ease.Linear;
        public int Loops = -1;
        public LoopType LoopType = LoopType.Restart;
        public float Delay;
        public float LoopDelay;
    }

    [System.Serializable]
    private sealed class FloatTween : TweenSettings
    {
        public float Amount = 1f;
    }

    [System.Serializable]
    private sealed class Vector2Tween : TweenSettings
    {
        public Vector2 Amount = new Vector2(0.1f, 0f);
    }

    [SerializeField] private bool _playOnEnable = true;
    [SerializeField] private bool _ignoreTimeScale = true;

    [SerializeField]
    private FloatTween _offset = new FloatTween
    {
        Enabled = true,
        Amount = 1f,
        Duration = 2f
    };

    [SerializeField]
    private FloatTween _angle = new FloatTween
    {
        Amount = 360f,
        Duration = 4f
    };

    [SerializeField]
    private FloatTween _scale = new FloatTween
    {
        Amount = 0.25f,
        Duration = 1f,
        Ease = Ease.InOutSine,
        LoopType = LoopType.Yoyo
    };

    [SerializeField]
    private Vector2Tween _center = new Vector2Tween
    {
        Amount = new Vector2(0.1f, 0f),
        Duration = 1.5f,
        Ease = Ease.InOutSine,
        LoopType = LoopType.Yoyo
    };

    private UIGradient _gradient;
    private bool _isPlaying;
    private float _initialOffset;
    private float _initialAngle;
    private float _initialScale;
    private Vector2 _initialCenter;

    public bool IsPlaying => _isPlaying;

    private void Awake()
    {
        _gradient = GetComponent<UIGradient>();
    }

    private void OnEnable()
    {
        if (_playOnEnable) Play();
    }

    private void OnDisable()
    {
        Stop();
    }

    public void Play()
    {
        Stop();

        _initialOffset = _gradient.Offset;
        _initialAngle = _gradient.Angle;
        _initialScale = _gradient.Scale;
        _initialCenter = _gradient.Center;
        _isPlaying = true;

        if (_offset.Enabled)
        {
            Build(DOTween.To(() => _gradient.Offset, x => _gradient.Offset = x, _offset.Amount, _offset.Duration), _offset);
        }

        if (_angle.Enabled)
        {
            Build(DOTween.To(() => _gradient.Angle, x => _gradient.Angle = x, _angle.Amount, _angle.Duration), _angle);
        }

        if (_scale.Enabled)
        {
            Build(DOTween.To(() => _gradient.Scale, x => _gradient.Scale = x, _scale.Amount, _scale.Duration), _scale);
        }

        if (_center.Enabled)
        {
            Build(DOTween.To(() => _gradient.Center, x => _gradient.Center = x, _center.Amount, _center.Duration), _center);
        }
    }

    public void Stop(bool restoreValues = true)
    {
        DOTween.Kill(this);

        if (_isPlaying && restoreValues && _gradient != null)
        {
            _gradient.Offset = _initialOffset;
            _gradient.Angle = _initialAngle;
            _gradient.Scale = _initialScale;
            _gradient.Center = _initialCenter;
        }

        _isPlaying = false;
    }

    private void Build(Tween tween, TweenSettings settings)
    {
        tween.SetRelative(true).SetEase(settings.Ease);

        DOTween.Sequence()
            .Append(tween)
            .AppendInterval(settings.LoopDelay)
            .SetLoops(settings.Loops, settings.LoopType)
            .SetDelay(settings.Delay)
            .SetUpdate(_ignoreTimeScale)
            .SetTarget(this)
            .SetLink(gameObject);
    }
}