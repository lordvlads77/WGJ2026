using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : Singleton<ILoadSource>, ILoadSource
{
    [Header("Async scenes")]
    [SerializeField] private List<string> _asyncScenes;

    [Header("Loading screen")]
    [SerializeField] private Canvas _loadingScreen;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private RectTransform _loadingIcon;
    [SerializeField] private float _iconRotationDuration = 1f;

    private Tween _loadingIconTween;
    private bool _isLoading;

    public void LoadScene(string sceneName)
    {
        if (_isLoading) return;

        if (sceneName == null) return;

        if (IsAsyncScene(sceneName))
        {
            LoadAsyncScene(sceneName).Forget();
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private bool IsAsyncScene(string sceneName)
    {
        return _asyncScenes.Contains(sceneName);
    }

    private async UniTaskVoid LoadAsyncScene(string sceneName)
    {
        Cursor.visible = false;
        _isLoading = true;
        _loadingScreen.enabled = true;

        _progressBar.value = 0f;
        StartLoadingAnimation();

        await UniTask.NextFrame();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        if (operation == null)
        {
            StopLoadingAnimation();
            _loadingScreen.enabled = false;
            _isLoading = false;
            return;
        }

        operation.allowSceneActivation = false;

        float displayedProgress = 0f;

        while (operation.progress < 0.9f)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.unscaledDeltaTime * 0.5f);
            _progressBar.value = displayedProgress;

            await UniTask.Yield();
        }

        while (displayedProgress < 1f)
        {
            displayedProgress = Mathf.MoveTowards(displayedProgress, 1f, Time.unscaledDeltaTime * 0.5f);
            _progressBar.value = displayedProgress;

            await UniTask.Yield();
        }

        operation.allowSceneActivation = true;
        await operation;

        StopLoadingAnimation();
        _loadingScreen.enabled = false;
        Cursor.visible = true;
        _isLoading = false;
    }

    private void StartLoadingAnimation()
    {
        _loadingIcon.localRotation = Quaternion.identity;
        _loadingIconTween = _loadingIcon.DORotate(new Vector3(0,0,-360f), _iconRotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(true);
    }

    private void StopLoadingAnimation()
    {
        _loadingIconTween?.Kill();
        _loadingIconTween = null;
        _loadingIcon.localRotation = Quaternion.identity;
    }
}

public interface ILoadSource
{
    void LoadScene(string sceneName);
}
