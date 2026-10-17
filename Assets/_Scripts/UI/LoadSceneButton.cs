using UnityEngine;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _button.onClick.AddListener(LoadScene);
        _button.interactable = true;
    }

    private void LoadScene()
    {
        LoadSceneManager.Source.LoadScene(_sceneName);
    }
}
