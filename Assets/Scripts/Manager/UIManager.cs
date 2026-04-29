using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Controls everything the player sees on screen
public class UIManager : MonoBehaviour
{
    // --- HUD ---
    public TMP_Text coinsText;
    public TMP_Text betText;
    public Button spinButton;

    // --- Win Popup ---
    public GameObject winPopupPanel;
    public TMP_Text winAmountText;

    [Range(1f, 5f)]
    public float popupDisplayTime = 2.5f;

    // --- Win Animations ---
    public RectTransform winPopupRect;

    [Range(0.05f, 0.5f)]
    public float punchScale = 0.2f;

    [Range(0.1f, 0.5f)]
    public float punchDuration = 0.25f;

    public ParticleSystem confettiParticles;

    // --- Coin Counter ---
    public TMP_Text coinCounterText;

    [Range(0.3f, 2f)]
    public float countDuration = 0.8f;

    // --- Coin Rain ---
    [Header("Coin Rain")]
    public GameObject coinRainPrefab;
    public RectTransform coinRainContainer;

    [Range(10, 100)]
    public int coinRainCount = 40;

    [Range(1f, 5f)]
    public float coinRainDuration = 3f;

    [Range(200f, 1500f)]
    public float coinRainMinSpeed = 400f;
    [Range(200f, 1500f)]
    public float coinRainMaxSpeed = 900f;

    public float coinSize = 80f;

    // --- Bet Menu ---
    public GameObject betMenuPanel;
    public Button bet10GButton;
    public Button bet50GButton;
    public Button bet100GButton;
    // Single quit button in bet menu — returns player to Main Menu
    public Button betMenuQuitButton;
    public SlotMachineController slotController;

    // Main menu scene name — must match exactly in Build Settings
    public string mainMenuSceneName = "MainMenuScene";

    // --- Other Panels ---
    public GameObject gameOverPanel;
    public Button playAgainButton;
    public Button quitButton;
    public TMP_Text messageText;

    [Range(0.5f, 3f)]
    public float messageDuration = 1.5f;

    private Color messageGoldenColor = new Color(1f, 0.84f, 0f, 1f);

    // --- Private coroutine handles ---
    private Coroutine messageCoroutine;
    private Coroutine popupCoroutine;
    private Coroutine coinRainCoroutine;


    private void Awake()
    {
        if (winPopupPanel != null)
            winPopupPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (messageText != null)
            messageText.gameObject.SetActive(false);

        if (betMenuPanel != null)
            betMenuPanel.SetActive(false);

        WireAllButtons();
    }

    private void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SwitchToGameMusic();
    }

    private void WireAllButtons()
    {
        if (bet10GButton != null)
            bet10GButton.onClick.AddListener(() => OnBet10GClicked());

        if (bet50GButton != null)
            bet50GButton.onClick.AddListener(() => OnBet50GClicked());

        if (bet100GButton != null)
            bet100GButton.onClick.AddListener(() => OnBet100GClicked());

        // Quit in bet menu → go to main menu
        if (betMenuQuitButton != null)
            betMenuQuitButton.onClick.AddListener(() => OnBetMenuQuitClicked());

        if (spinButton != null)
            spinButton.onClick.AddListener(() => OnSpinButtonClicked());

        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(() => OnPlayAgainPressed());

        if (quitButton != null)
            quitButton.onClick.AddListener(() => OnQuitPressed());
    }

    private void OnBet10GClicked()
    {
        PlayButtonClickSound();
        if (slotController != null)
            slotController.OnBetSelected(0);
    }

    private void OnBet50GClicked()
    {
        PlayButtonClickSound();
        if (slotController != null)
            slotController.OnBetSelected(1);
    }

    private void OnBet100GClicked()
    {
        PlayButtonClickSound();
        if (slotController != null)
            slotController.OnBetSelected(2);
    }

    // Quit in bet menu → click sound + load main menu
    private void OnBetMenuQuitClicked()
    {
        PlayButtonClickSound();
        HideBetMenu();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClickSound();
    }

    private void OnSpinButtonClicked()
    {
        PlayButtonClickSound();
        if (slotController != null)
            slotController.OnSpinPressed();
    }

    public void ShowBetMenu()
    {
        if (betMenuPanel != null)
            betMenuPanel.SetActive(true);
    }

    public void HideBetMenu()
    {
        if (betMenuPanel != null)
            betMenuPanel.SetActive(false);
    }

    public void UpdateCoinsDisplay(int coins)
    {
        if (coinsText != null)
            coinsText.text = "COINS: " + coins;
    }

    public void UpdateBetDisplay(int bet)
    {
        if (betText != null)
            betText.text = "BET: " + bet;
    }

    public void SetSpinButtonInteractable(bool canClick)
    {
        if (spinButton != null)
            spinButton.interactable = canClick;
    }

    public void ShowWinPopup(int payoutAmount)
    {
        if (winPopupPanel == null) return;

        winPopupPanel.SetActive(true);

        if (popupCoroutine != null)
            StopCoroutine(popupCoroutine);

        popupCoroutine = StartCoroutine(WinPopupSequence(payoutAmount));
    }

    private IEnumerator WinPopupSequence(int payoutAmount)
    {
        if (winPopupRect != null)
            StartCoroutine(PunchScale(winPopupRect));

        if (confettiParticles != null)
        {
            confettiParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            confettiParticles.Play();
        }

        if (coinRainPrefab != null && coinRainContainer != null)
        {
            if (coinRainCoroutine != null)
                StopCoroutine(coinRainCoroutine);

            coinRainCoroutine = StartCoroutine(PlayCoinRain());
        }

        if (coinCounterText != null)
        {
            StartCoroutine(AnimateCoinCount(payoutAmount));
        }
        else if (winAmountText != null)
        {
            winAmountText.text = "+" + payoutAmount;
        }

        yield return new WaitForSeconds(popupDisplayTime);

        winPopupPanel.SetActive(false);

        ShowBetMenu();
    }

    private IEnumerator PlayCoinRain()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCoinRainSound();

        float containerWidth = coinRainContainer.rect.width;
        float containerHeight = coinRainContainer.rect.height;

        float interval = coinRainDuration / coinRainCount;

        for (int i = 0; i < coinRainCount; i++)
        {
            SpawnCoin(containerWidth, containerHeight);
            yield return new WaitForSeconds(interval);
        }
    }

    private void SpawnCoin(float containerWidth, float containerHeight)
    {
        GameObject coin = Instantiate(coinRainPrefab, coinRainContainer);
        RectTransform rt = coin.GetComponent<RectTransform>();

        if (rt == null) return;

        rt.sizeDelta = new Vector2(coinSize, coinSize);

        float spawnX = Random.Range(-containerWidth / 2f, containerWidth / 2f);
        float spawnY = containerHeight / 2f + coinSize;

        rt.anchoredPosition = new Vector2(spawnX, spawnY);
        rt.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        float speed = Random.Range(coinRainMinSpeed, coinRainMaxSpeed);
        float spinSpeed = Random.Range(-360f, 360f);

        StartCoroutine(AnimateCoin(rt, speed, spinSpeed, containerHeight));
    }

    private IEnumerator AnimateCoin(RectTransform rt, float fallSpeed, float spinSpeed, float containerHeight)
    {
        float bottomBound = -containerHeight / 2f - coinSize;

        while (rt != null && rt.anchoredPosition.y > bottomBound)
        {
            rt.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;
            rt.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
            yield return null;
        }

        if (rt != null)
            Destroy(rt.gameObject);
    }

    private IEnumerator PunchScale(RectTransform target)
    {
        Vector3 originalScale = target.localScale;
        Vector3 bigScale = originalScale * (1f + punchScale);
        float halfDuration = punchDuration / 2f;
        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            target.localScale = Vector3.Lerp(originalScale, bigScale, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < halfDuration)
        {
            target.localScale = Vector3.Lerp(bigScale, originalScale, elapsed / halfDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localScale = originalScale;
    }

    private IEnumerator AnimateCoinCount(int targetAmount)
    {
        float elapsed = 0f;

        while (elapsed < countDuration)
        {
            float t = elapsed / countDuration;
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            int displayValue = Mathf.RoundToInt(eased * targetAmount);

            coinCounterText.text = "+" + displayValue;

            if (winAmountText != null)
                winAmountText.text = "+" + displayValue;

            elapsed += Time.deltaTime;
            yield return null;
        }

        coinCounterText.text = "+" + targetAmount;

        if (winAmountText != null)
            winAmountText.text = "+" + targetAmount;
    }

    public void ShowLoseMessage()
    {
        ShowMessage("Better luck next time!");
        StartCoroutine(ShowBetMenuAfterDelay(messageDuration));
    }

    private IEnumerator ShowBetMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowBetMenu();
    }

    public void ShowMessage(string message)
    {
        if (messageText == null) return;

        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        messageCoroutine = StartCoroutine(ShowMessageCoroutine(message));
    }

    private IEnumerator ShowMessageCoroutine(string message)
    {
        messageText.text = message;
        messageText.color = messageGoldenColor;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageDuration);

        messageText.gameObject.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        SetSpinButtonInteractable(false);
    }

    public void OnPlayAgainPressed()
    {
        PlayButtonClickSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnQuitPressed()
    {
        PlayButtonClickSound();
#if !UNITY_WEBGL
        Application.Quit();
#endif
    }
}