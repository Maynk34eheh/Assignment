using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Handles main menu UI and audio
public class MainMenuManager : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;

    // Name of the game scene to load
    public string gameSceneName = "GameScene";

    private void Awake()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void Start()
    {
        // Play main menu background music
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMainMenuMusic();
    }

    private void OnPlayClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClickSound();

        SceneManager.LoadScene(gameSceneName);
    }

    private void OnQuitClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClickSound();

#if !UNITY_WEBGL
        Application.Quit();
#endif
    }
}
