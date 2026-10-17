using UnityEngine;

public class DefaultState : MonoBehaviour
{
    private void Start()
    {
        GameManager.Source.ChangeState(GameState.OnPlay);
    }
}
