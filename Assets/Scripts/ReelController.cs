using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Controls one spinning reel
// The spin happens in two parts:
//   Part 1 - Spins fast with random symbols flying by
//   Part 2 - Slows down and snaps to the final result
public class ReelController : MonoBehaviour
{
    // The 3 image slots on this reel (top, middle, bottom)
    public List<Image> symbolImages = new List<Image>();

    // The panel that moves up and down to look like it's spinning
    public RectTransform reelStrip;

    [Range(0.5f, 5f)]
    public float spinDuration = 1.5f;

    [Range(200f, 2000f)]
    public float spinSpeed = 800f;

    [Range(0.1f, 1f)]
    public float easeOutDuration = 0.3f;

    // Height of each symbol image in pixels
    public float symbolHeight = 150f;

    private SymbolData targetSymbol;
    private SymbolData[] allSymbols;
    private bool isSpinning = false;
    private Coroutine spinCoroutine;

    public bool IsSpinning { get { return isSpinning; } }

    // Called once at the start — gives this reel the list of possible symbols
    public void Initialize(SymbolData[] symbols)
    {
        allSymbols = symbols;

        if (symbols != null && symbols.Length > 0)
            DisplaySymbol(symbols[0]);
    }

    // Starts the spin and lands on the given symbol
    // delay = how long to wait before starting (used for the stagger effect)
    public void Spin(SymbolData symbolToLandOn, float delay = 0f)
    {
        if (isSpinning) return;

        targetSymbol = symbolToLandOn;

        if (spinCoroutine != null)
            StopCoroutine(spinCoroutine);

        spinCoroutine = StartCoroutine(SpinRoutine(delay));
    }

    private IEnumerator SpinRoutine(float delay)
    {
        isSpinning = true;

        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        float timeElapsed = 0f;
        float fastPhaseDuration = spinDuration - easeOutDuration;

        // Part 1: spin fast with random symbols
        while (timeElapsed < fastPhaseDuration)
        {
            float deltaTime = Time.deltaTime;
            timeElapsed += deltaTime;
            ScrollReel(spinSpeed * deltaTime);
            ShowRandomSymbols();
            yield return null;
        }

        // Part 2: slow down and snap to the result
        float easeTimeElapsed = 0f;
        while (easeTimeElapsed < easeOutDuration)
        {
            float t = easeTimeElapsed / easeOutDuration;
            float slowingSpeed = Mathf.Lerp(spinSpeed, 0f, t * t);
            ScrollReel(slowingSpeed * Time.deltaTime);
            easeTimeElapsed += Time.deltaTime;
            yield return null;
        }

        DisplaySymbol(targetSymbol);
        ResetReelPosition();

        isSpinning = false;
    }

    // Moves the reel strip to simulate spinning
    private void ScrollReel(float amount)
    {
        if (reelStrip == null) return;

        Vector3 currentPos = reelStrip.localPosition;
        currentPos.y -= amount;

        float bounds = symbolHeight;

        if (currentPos.y < -bounds)
        {
            currentPos.y += symbolHeight * 2;
            CycleSymbolsUp();
        }
        else if (currentPos.y > bounds)
        {
            currentPos.y -= symbolHeight * 2;
            CycleSymbolsDown();
        }

        reelStrip.localPosition = currentPos;
    }

    // Shifts symbols up and puts a new random one at the bottom
    private void CycleSymbolsUp()
    {
        if (symbolImages == null || symbolImages.Count < 3 || allSymbols == null) return;

        for (int i = 0; i < symbolImages.Count - 1; i++)
            symbolImages[i].sprite = symbolImages[i + 1].sprite;

        int randomIndex = Random.Range(0, allSymbols.Length);
        symbolImages[symbolImages.Count - 1].sprite = allSymbols[randomIndex].sprite;
    }

    // Shifts symbols down and puts a new random one at the top
    private void CycleSymbolsDown()
    {
        if (symbolImages == null || symbolImages.Count < 3 || allSymbols == null) return;

        for (int i = symbolImages.Count - 1; i > 0; i--)
            symbolImages[i].sprite = symbolImages[i - 1].sprite;

        int randomIndex = Random.Range(0, allSymbols.Length);
        symbolImages[0].sprite = allSymbols[randomIndex].sprite;
    }

    // Shows random symbols on all slots to create a blur effect while spinning
    private void ShowRandomSymbols()
    {
        if (allSymbols == null || allSymbols.Length == 0) return;

        foreach (Image img in symbolImages)
        {
            int randomIndex = Random.Range(0, allSymbols.Length);
            img.sprite = allSymbols[randomIndex].sprite;
        }
    }

    // Shows the final symbol in the middle slot (that's the result)
    private void DisplaySymbol(SymbolData symbol)
    {
        if (symbolImages == null || symbolImages.Count == 0 || symbol == null) return;

        int middleIndex = symbolImages.Count / 2;
        symbolImages[middleIndex].sprite = symbol.sprite;
    }

    // Resets the strip position so it doesn't drift after many spins
    private void ResetReelPosition()
    {
        if (reelStrip != null)
            reelStrip.localPosition = Vector3.zero;
    }

    public SymbolData GetCurrentSymbol()
    {
        return targetSymbol;
    }
}
