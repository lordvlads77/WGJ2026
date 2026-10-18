using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _closeSettingsButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _closeCreditsButton;
    [SerializeField] private Button _exitButton;

    [SerializeField] private Canvas _settingsCanvas;
    [SerializeField] private Canvas _creditsCanvas;
    [SerializeField] private Canvas _menuCanvas;

    private void Awake()
    {
        _menuCanvas.enabled = true;
        _settingsCanvas.enabled = false;
        _creditsCanvas.enabled = false;
    }

    private void Start()
    {
        _settingsButton.onClick.AddListener(OpenSettingsScreen);
        _closeSettingsButton.onClick.AddListener(CloseSettingsScreen);
        _creditsButton.onClick.AddListener(OpenCreditsScreen);
        _closeCreditsButton.onClick.AddListener(CloseCreditsScreen);
        _exitButton.onClick.AddListener(ExitGame);
    }

    private void OpenSettingsScreen()
    {
        _settingsCanvas.enabled = true;
        _menuCanvas.enabled = false;
    }

    private void CloseSettingsScreen()
    {
        BackToMenu(_settingsCanvas);
    }

    private void OpenCreditsScreen()
    {
        _creditsCanvas.enabled = true;
        _menuCanvas.enabled = false;
    }

    private void CloseCreditsScreen()
    {
        BackToMenu(_creditsCanvas);
    }

    private void BackToMenu(Canvas canvasName)
    {
        _menuCanvas.enabled = true;
        canvasName.enabled = false;
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
