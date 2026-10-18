using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComicManager : MonoBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform[] _panelPositions;
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private float _waitTime = 3f;
    [SerializeField] private Ease _ease = Ease.InOutSine;

    private Tween _cameraTween;

    private void Start()
    {
        float delay = 0f;

        foreach (Transform panel in _panelPositions)
        {
            _cameraTween = _camera.DOMove(panel.position, _moveDuration)
                                  .SetEase(_ease)
                                  .SetDelay(delay);

            delay += _moveDuration + _waitTime;
        }

        _cameraTween.OnComplete(() =>
            DOVirtual.DelayedCall(_waitTime, () => SceneManager.LoadScene("Test_MainMenu")));
    }
}
