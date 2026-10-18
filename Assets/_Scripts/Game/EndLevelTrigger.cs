using UnityEngine;

public class EndLevelTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Source.CurrentGameState != GameState.OnPlay) return;

        GameManager.Source.ChangeState(GameState.OnGameOver);
    }
}
