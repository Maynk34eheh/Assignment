using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Picks random symbols using weighted probability
public class SlotRNG
{
    private List<SymbolData> symbolPool;
    private int totalWeight;

    public SlotRNG(SymbolData[] symbols)
    {
        symbolPool = new List<SymbolData>(symbols);
        totalWeight = 0;

        foreach (SymbolData s in symbols)
            totalWeight += s.weight;

        if (totalWeight == 0)
            Debug.LogError("Total weight is 0! Make sure your symbols have weight values.");
    }

    public SymbolData GetRandomSymbol()
    {
        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (SymbolData symbol in symbolPool)
        {
            cumulative += symbol.weight;
            if (roll < cumulative)
                return symbol;
        }

        // Fallback — should never happen if weights are set up correctly
        return symbolPool[symbolPool.Count - 1];
    }

    public SymbolData[] SpinAllReels(int reelCount)
    {
        SymbolData[] results = new SymbolData[reelCount];

        for (int i = 0; i < reelCount; i++)
            results[i] = GetRandomSymbol();

        return results;
    }

    public static bool IsWin(SymbolData[] results)
    {
        if (results == null || results.Length < 2) return false;

        for (int i = 1; i < results.Length; i++)
            if (results[i].symbolType != results[0].symbolType)
                return false;

        return true;
    }

    public static int CalculatePayout(SymbolData[] results, int betAmount)
    {
        if (!IsWin(results)) return 0;
        return betAmount * results[0].payoutMultiplier;
    }
}


// Main controller — runs the whole slot machine game
public class SlotMachineController : MonoBehaviour
{
    public ReelController[] reels = new ReelController[3];
    public SymbolData[] symbolDataAssets;
    public PlayerData playerData = new PlayerData();

    [Range(0f, 1f)]
    public float reelStopDelay = 0.4f;

    public UIManager uiManager;

    [Header("Handle")]
    [SerializeField] private HandleController handleController;

    [Range(0.1f, 1f)]
    public float handlePulledDuration = 0.4f;

    [Header("Audio")]
    [Range(0.1f, 3f)]
    public float reelSoundPitch = 1.0f;

    private SlotRNG rng;
    private bool isSpinning = false;
    private bool betPending = false;

    private void Start()
    {
        SetupGame();
    }

    private void SetupGame()
    {
        rng = new SlotRNG(symbolDataAssets);

        foreach (ReelController reel in reels)
            if (reel != null)
                reel.Initialize(symbolDataAssets);

        playerData.Initialize();

        if (uiManager != null)
        {
            uiManager.UpdateCoinsDisplay(playerData.Coins);
            uiManager.UpdateBetDisplay(playerData.CurrentBet);
            uiManager.SetSpinButtonInteractable(true);
        }

        if (handleController != null)
            handleController.SetNormal();
    }

    // Called when the spin button is pressed
    public void OnSpinPressed()
    {
        if (isSpinning) return;

        if (betPending)
        {
            if (uiManager != null)
                uiManager.ShowMessage("Place a bet first!");
            return;
        }

        StartCoroutine(HandlePressSequence());
    }

    // Plays the lever animation then shows the bet menu
    private IEnumerator HandlePressSequence()
    {
        if (uiManager != null)
            uiManager.SetSpinButtonInteractable(false);

        if (handleController != null)
            handleController.SetPulled();

        yield return new WaitForSeconds(handlePulledDuration);

        if (handleController != null)
            handleController.SetNormal();

        if (!isSpinning && uiManager != null)
            uiManager.SetSpinButtonInteractable(true);

        betPending = true;

        if (uiManager != null)
            uiManager.ShowBetMenu();
    }

    // Called when the player closes the bet menu without picking a bet
    public void OnBetMenuExited()
    {
        betPending = false;

        if (handleController != null)
            handleController.SetNormal();
    }

    // Called when the player picks a bet amount from the bet menu
    public void OnBetSelected(int tierIndex)
    {
        if (isSpinning) return;

        int betAmount = playerData.BetTiers[tierIndex];

        if (playerData.Coins < betAmount)
        {
            uiManager.ShowMessage("Not enough coins!");
            uiManager.ShowBetMenu();
            return;
        }

        if (!playerData.PlaceBetAtTier(tierIndex))
            return;

        betPending = false;

        uiManager.UpdateCoinsDisplay(playerData.Coins);
        uiManager.UpdateBetDisplay(playerData.CurrentBet);
        uiManager.HideBetMenu();
        uiManager.SetSpinButtonInteractable(false);

        StartCoroutine(SpinRoutine(betAmount));
    }

    private IEnumerator SpinRoutine(int betAmount)
    {
        isSpinning = true;

        if (handleController != null)
            handleController.SetPulled();

        StartCoroutine(ResetHandleAfterDelay(handlePulledDuration));

        AudioManager audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.PlayReelSpinSound();
            audioManager.SetReelPitch(reelSoundPitch);
        }

        SymbolData[] results = rng.SpinAllReels(reels.Length);

        // Start each reel with a slight delay so they stop one at a time
        for (int i = 0; i < reels.Length; i++)
        {
            float delay = i * reelStopDelay;
            reels[i]?.Spin(results[i], delay);
        }

        float lastDelay = (reels.Length - 1) * reelStopDelay;

        float longest = 0f;
        foreach (var reel in reels)
            if (reel != null && reel.spinDuration > longest)
                longest = reel.spinDuration;

        yield return new WaitForSeconds(lastDelay + longest + 0.1f);

        if (audioManager != null)
        {
            audioManager.StopReelSpinSound();
            audioManager.ResetReelPitch();
        }

        CheckResult(results, betAmount);

        isSpinning = false;
        betPending = false;

        uiManager.SetSpinButtonInteractable(true);

        if (playerData.IsGameOver())
            uiManager.ShowGameOver();
    }

    private IEnumerator ResetHandleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (handleController != null)
            handleController.SetNormal();
    }

    private void CheckResult(SymbolData[] results, int betAmount)
    {
        if (SlotRNG.IsWin(results))
        {
            int payout = SlotRNG.CalculatePayout(results, betAmount);
            playerData.AwardPayout(payout);

            AudioManager audioManager = AudioManager.Instance;
            if (audioManager != null)
                audioManager.PlayWinSound();

            uiManager.UpdateCoinsDisplay(playerData.Coins);
            uiManager.ShowWinPopup(payout);
        }
        else
        {
            uiManager.ShowLoseMessage();
        }
    }
}
