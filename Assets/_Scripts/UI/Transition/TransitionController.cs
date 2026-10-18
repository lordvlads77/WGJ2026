using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public sealed class TransitionController : MonoBehaviour
{
    private static readonly int MaskTexId = Shader.PropertyToID("_MaskTex");
    private static readonly int ScaleId = Shader.PropertyToID("_Scale");
    private static readonly int AspectId = Shader.PropertyToID("_Aspect");
    private static readonly int MaskAspectId = Shader.PropertyToID("_MaskAspect");
    private static readonly int SoftnessId = Shader.PropertyToID("_Softness");

    [SerializeField] private IrisTransitionSetup _coverProfile;
    [SerializeField] private IrisTransitionSetup _revealProfile;
    [SerializeField] private float _holdDuration = 0.15f;

    public event Action Covered;
    public event Action Revealed;

    public bool IsCovered { get; private set; }
    public bool IsBusy => _activeTween != null && _activeTween.IsActive();

    private Image _image;
    private RectTransform _rect;
    private Material _material;
    private Tween _activeTween;
    private float _scale;
    private float _aspect = 16f / 9f;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rect = (RectTransform)transform;
        _material = new Material(_image.material);
        _image.material = _material;
        _image.enabled = false;

        UpdateAspect();
    }

    private void OnDestroy()
    {
        _activeTween?.Kill();
        if (_material != null)
        {
            Destroy(_material);
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        if (_material == null) return;
        UpdateAspect();
    }

    public async UniTask PlayAsync(Func<CancellationToken, UniTask> onCovered, CancellationToken ct = default)
    {
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);
        var token = linked.Token;

        await CoverAsync(_coverProfile, token);
        if (onCovered != null) await onCovered(token);
        if (_holdDuration > 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_holdDuration), cancellationToken: token);
        }
        await RevealAsync(_revealProfile, token);
    }

    public UniTask PlayAsync(Action onCovered, CancellationToken ct = default)
    {
        return PlayAsync(_ =>
        {
            onCovered?.Invoke();
            return UniTask.CompletedTask;
        }, ct);
    }

    public UniTask CoverAsync(CancellationToken ct = default) => CoverAsync(_coverProfile, ct);
    public UniTask RevealAsync(CancellationToken ct = default) => RevealAsync(_revealProfile, ct);

    public async UniTask CoverAsync(IrisTransitionSetup profile, CancellationToken ct = default)
    {
        if (profile == null) throw new ArgumentNullException(nameof(profile));

        bool wasHidden = !_image.enabled;
        UpdateAspect();
        if (wasHidden || _scale <= 0f) ApplyShape(profile);
        if (wasHidden) SetScale(profile.GetOpenScale(_aspect));
        _image.enabled = true;

        await TweenScale(0f, profile, ct);

        IsCovered = true;
        Covered?.Invoke();
    }

    public async UniTask RevealAsync(IrisTransitionSetup profile, CancellationToken ct = default)
    {
        if (profile == null) throw new ArgumentNullException(nameof(profile));

        UpdateAspect();

        if (_scale <= 0f) ApplyShape(profile);
        _image.enabled = true;
        IsCovered = false;

        await TweenScale(profile.GetOpenScale(_aspect), profile, ct);

        _image.enabled = false;
        Revealed?.Invoke();
    }

    private async UniTask TweenScale(float target, IrisTransitionSetup profile, CancellationToken ct)
    {
        _activeTween?.Kill();

        var tween = DOTween.To(() => _scale, SetScale, target, profile.Duration)
            .SetTarget(this)
            .SetLink(gameObject);
        profile.ApplyEase(tween);
        _activeTween = tween;

        await AwaitTween(tween, ct);
    }

    private static async UniTask AwaitTween(Tween tween, CancellationToken ct)
    {
        var tcs = new UniTaskCompletionSource<bool>();
        tween.OnComplete(() => tcs.TrySetResult(true));
        tween.OnKill(() => tcs.TrySetResult(false));

        bool completed;
        using (ct.Register(() => { if (tween.IsActive()) tween.Kill(); }))
        {
            completed = await tcs.Task;
        }

        if (!completed) throw new OperationCanceledException(ct);
    }

    private void ApplyShape(IrisTransitionSetup profile)
    {
        var tex = profile.Shape != null ? profile.Shape : Texture2D.whiteTexture;
        _material.SetTexture(MaskTexId, tex);
        _material.SetFloat(MaskAspectId, (float)tex.width / tex.height);
        _material.SetFloat(SoftnessId, profile.Softness);
    }

    private void SetScale(float value)
    {
        _scale = value;
        _material.SetFloat(ScaleId, value);
    }

    private void UpdateAspect()
    {
        var r = _rect.rect;
        if (r.height <= 0f) return;
        _aspect = r.width / r.height;
        _material.SetFloat(AspectId, _aspect);
    }
}
