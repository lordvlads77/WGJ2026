using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ResetPlayer : MonoBehaviour
{
    [SerializeField] private TransitionController _transition;
    [SerializeField] private CameraFollowPlayer _cameraFollow;

    private Vector3 _initialPosition;
    private Rigidbody _playerRigidbody;
    private bool _isResetting;

    private void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _initialPosition = transform.position;
    }

    private void OnEnable()
    {
        GameOverController.OnRestartClicked += RestartLevel;
    }

    private void OnDisable()
    {
        GameOverController.OnRestartClicked -= RestartLevel;
    }

    public void PlayerReset()
    {
        if (_isResetting) return;

        if (_transition == null)
        {
            TeleportToStart();
            return;
        }

        PlayerResetAsync(destroyCancellationToken).Forget();
    }

    private async UniTaskVoid PlayerResetAsync(CancellationToken ct)
    {
        _isResetting = true;
        GameManager.Source.ChangeState(GameState.OnRespawn);

        try
        {
            await _transition.PlayAsync(TeleportToStart, ct);

            if (GameManager.Source.CurrentGameState == GameState.OnRespawn)
            {
                GameManager.Source.ChangeState(GameState.OnPlay);
            }
        }
        finally
        {
            _isResetting = false;
        }
    }

    private void TeleportToStart()
    {
        _playerRigidbody.linearVelocity = Vector3.zero;
        _playerRigidbody.angularVelocity = Vector3.zero;
        _playerRigidbody.position = _initialPosition;
        transform.position = _initialPosition;

        if (_cameraFollow != null)
        {
            _cameraFollow.SnapToTarget();
        }
    }

    private void RestartLevel()
    {
        StartReset();
    }

    private void StartReset()
    {
        if (_isResetting) return;

        if (_transition == null)
        {
            TeleportToStart();
            GameManager.Source.ChangeState(GameState.OnPlay);
            return;
        }

        PlayerResetAsync(destroyCancellationToken).Forget();
    }
}
