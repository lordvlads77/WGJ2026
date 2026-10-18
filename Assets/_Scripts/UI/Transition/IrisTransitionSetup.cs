using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "Transitions/Iris Transition Setup", fileName = "IrisSetup")]
public sealed class IrisTransitionSetup : ScriptableObject
{
    [SerializeField] private Texture2D _shape;
    [SerializeField, Range(0.05f, 1f)] private float _innerRadius = 0.9f;
    [SerializeField, Range(0f, 0.5f)] private float _softness = 0.03f;

    [SerializeField, Min(0f)] private float _duration = 0.6f;
    [SerializeField] private Ease _ease = Ease.InOutCubic;
    [SerializeField] private bool _useCustomCurve;
    [SerializeField] private AnimationCurve _customCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private const float OpenMargin = 1.05f;

    public Texture2D Shape => _shape;
    public float Softness => _softness;
    public float Duration => _duration;

    public float GetOpenScale(float screenAspect)
    {
        return Mathf.Sqrt(screenAspect * screenAspect + 1f) / _innerRadius * OpenMargin;
    }

    public void ApplyEase(Tween tween)
    {
        if (_useCustomCurve) tween.SetEase(_customCurve);
        else tween.SetEase(_ease);
    }
}
