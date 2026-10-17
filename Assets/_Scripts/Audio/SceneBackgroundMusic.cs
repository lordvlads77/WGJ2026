using UnityEngine;

public class SceneBackgroundMusic : MonoBehaviour
{
    [SerializeField] private string _musicName;

    private void Start()
    {
        AudioManager.Source.PlayLevelMusic(_musicName);
    }
}
