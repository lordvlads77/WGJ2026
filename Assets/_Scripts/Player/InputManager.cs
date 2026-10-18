using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputSystem_Actions _inputActions;

    private InputAction _moveAction;
    private InputAction _jumpAction;

    void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _moveAction = _inputActions.Player.Movement;
        _jumpAction = _inputActions.Player.Jump;
    }

    void Start()
    {
        GameManager.Source.OnGameStateChanged += OnGameStateChanged;
        OnGameStateChanged(GameManager.Source.CurrentGameState);
    }

    void OnEnable()
    {
        _moveAction.performed += OnMovePerformed;
        _moveAction.canceled += OnMoveCancelled;
        _jumpAction.performed += OnJumpPerformed;

        _inputActions.Enable();

        if (GameManager.Source != null)
        {
            OnGameStateChanged(GameManager.Source.CurrentGameState);
        }
    }

    void OnDisable()
    {
        _moveAction.performed -= OnMovePerformed;
        _moveAction.canceled -= OnMoveCancelled;
        _jumpAction.performed -= OnJumpPerformed;

        _inputActions.Disable();
    }

    void OnDestroy()
    {
        if (GameManager.Source != null)
        {
            GameManager.Source.OnGameStateChanged -= OnGameStateChanged;
        }
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.OnPlay)
        {
            _inputActions.Player.Enable();
        }
        else
        {
            _inputActions.Player.Disable();
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();

        PlayerEvents.RaiseMovePerformed(true, direction);
    }

    private void OnMoveCancelled(InputAction.CallbackContext context)
    {
        PlayerEvents.RaiseMovePerformed(false, Vector2.zero);
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        PlayerEvents.RaiseJumpPerformed(true);
    }
}
