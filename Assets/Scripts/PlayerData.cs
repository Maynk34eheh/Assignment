using UnityEngine;

// Keeps track of the player's coins and current bet
[System.Serializable]
public class PlayerData
{
    public int startingCoins = 1000;
    public int[] betTiers = new int[] { 10, 50, 100 };

    private int coins;
    private int currentBetTier = 0;

    public int Coins { get { return coins; } }
    public int CurrentBet { get { return betTiers[currentBetTier]; } }
    public int CurrentBetTier { get { return currentBetTier; } }
    public int[] BetTiers { get { return betTiers; } }

    // Sets up starting values at the beginning of the game
    public void Initialize()
    {
        coins = startingCoins;
        currentBetTier = 0;
    }

    // Deducts the selected bet amount from the player's coins
    public bool PlaceBetAtTier(int tierIndex)
    {
        if (tierIndex < 0 || tierIndex >= betTiers.Length)
        {
            Debug.LogWarning("Invalid bet tier index.");
            return false;
        }

        currentBetTier = tierIndex;
        int betAmount = betTiers[tierIndex];

        if (coins < betAmount)
        {
            Debug.LogWarning("Not enough coins to place bet.");
            return false;
        }

        coins -= betAmount;
        return true;
    }

    // Adds winnings to the player's coins
    public void AwardPayout(int amount)
    {
        coins += amount;
    }

    // Returns true if the player can afford the current bet
    public bool CanSpin()
    {
        return coins >= betTiers[currentBetTier];
    }

    // Returns true if the player can't afford even the cheapest bet
    public bool IsGameOver()
    {
        return coins < betTiers[0];
    }
}
