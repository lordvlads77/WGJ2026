using System;
using UnityEngine;

#if UNITY_EDITOR
public class DebugInput : Singleton<IDebugInputSource>, IDebugInputSource
{
    public event Action OnPauseButtonPressed;

    private void Update()
    {
        CheckPauseInput();
    }

    private void CheckPauseInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (GameManager.Source.CurrentGameState != GameState.OnPause)
            {
                OnPauseButtonPressed?.Invoke();
                GameManager.Source.ChangeState(GameState.OnPause);
                Debug.Log("Current Game State: " + GameManager.Source.CurrentGameState);
            }
            else
            {
                GameManager.Source.ResumePreviousState();
                Debug.Log("Current Game State: " + GameManager.Source.CurrentGameState);
            }
        }
    }
}

public interface IDebugInputSource
{
    event Action OnPauseButtonPressed;
}
#endif