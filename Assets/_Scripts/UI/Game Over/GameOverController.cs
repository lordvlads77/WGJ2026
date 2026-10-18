using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _panel;
    [SerializeField] private Button _retryButton;
    [SerializeField] private float _fadeDuration = 0.35f;

    public static event Action OnRestartClicked;

    private Tween _fadeTween;

    private void Awake()
    {
        HideImmediate();
    }

    private void Start()
    {
        GameManager.Source.OnGameStateChanged += OnGameStateChanged;
        _retryButton.onClick.AddListener(OnRetryClicked);
    }

    private void OnDestroy()
    {
        GameManager.Source.OnGameStateChanged -= OnGameStateChanged;

        _retryButton.onClick.RemoveListener(OnRetryClicked);
        _fadeTween?.Kill();
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.OnGameOver)
        {
            Show();
        }
        else
        {
            HideImmediate();
        }
    }

    private void Show()
    {
        _fadeTween?.Kill();

        _panel.alpha = 0f;
        _panel.blocksRaycasts = true;
        _panel.interactable = false;

        _fadeTween = _panel.DOFade(1f, _fadeDuration)
            .SetUpdate(true)
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                _panel.interactable = true;

                if (EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(_retryButton.gameObject);
                }
            });
    }

    private void HideImmediate()
    {
        _fadeTween?.Kill();

        _panel.alpha = 0f;
        _panel.blocksRaycasts = false;
        _panel.interactable = false;
    }

    private void OnRetryClicked()
    {
        _panel.interactable = false;
        OnRestartClicked?.Invoke();
    }
}
