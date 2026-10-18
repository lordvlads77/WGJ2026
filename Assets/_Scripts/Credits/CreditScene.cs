using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditScene : MonoBehaviour
{
    [SerializeField] private GameObject[] _titleTexts;
    [SerializeField] private float _scrollDistance;
    [SerializeField] private float _duration;
    [SerializeField] private float _timerUntilNext = 3f;
    [SerializeField] private Ease _ease;

    [SerializeField] private Button _button;

    private float _maxTimerTime;
    private Tween _titleTween;

    private void Start()
    {
        ScrollCredits();
        _button.onClick.AddListener(ReturnToMainMenu);
    }

    private void ScrollCredits()
    {
        float delay = 0f;
        foreach (GameObject titleText in _titleTexts)
        {
           
            Transform _textTransform = titleText.transform;
            _titleTween = _textTransform.DOMoveY(_textTransform.position.y + _scrollDistance, _duration).SetEase(_ease).SetDelay(delay);

            delay += _timerUntilNext;            
            
        }
        _titleTween.OnComplete(() => LoadSceneManager.Source.LoadScene("Menu"));
    }

    private void ReturnToMainMenu()
    {
        LoadSceneManager.Source.LoadScene("Menu");
    }
}

