# Slot Machine Game — Unity

A slot machine game made in Unity with spinning reel animations, a weighted random system, win detection, and a full UI.

---

## How to Run the WebGL Build Locally

1. Open a terminal or command prompt
2. Navigate to your WebGL build folder:
   ```
   cd D:\Unity\Assignment\Build\WebGL
   ```
3. Start a local server:
   ```
   python -m http.server 8080
   ```
4. Open your browser and go to:
   ```
   http://localhost:8080
   ```

> Make sure Python is installed. If `python` doesn't work, try `python3`.

---

## How the Game Works

You start with **1000 coins**. Press **SPIN**, pick a bet amount from the bet menu, and watch the three reels spin. If all three land on the same symbol, you win coins. The game ends when you can't afford the minimum bet.

### Main Menu
- **Play** — starts the game
- **Quit** — closes the game (on WebGL it closes the browser tab)

### Bet Menu (opens when you press SPIN)
- Choose how much to bet: **10 / 50 / 100** coins
- **Quit** — returns you to the main menu

### Symbols & Payouts

| Symbol | Rarity | Payout |
|--------|--------|--------|
| Symbol 1 | Very common (weight: 40) | 2× your bet |
| Symbol 2 | Common (weight: 35) | 5× your bet |
| Symbol 3 | Uncommon (weight: 15) | 10× your bet |
| Symbol 4 | Rare / jackpot (weight: 10) | 20× your bet |

---

## Project Structure

| Script | What it does |
|--------|--------------|
| `MainMenuManager.cs` | Handles the Play and Quit buttons on the main menu |
| `SlotMachineController.cs` | Runs the main game loop — spins reels, checks results, handles bets |
| `ReelController.cs` | Controls the spin animation for one reel |
| `UIManager.cs` | Updates everything the player sees: coins, bet menu, win popup, game over screen |
| `AudioManager.cs` | Plays all music and sound effects |
| `PlayerData.cs` | Stores the player's coins and bet — no Unity code, just plain data |
| `HandleController.cs` | Swaps between the normal and pulled lever sprites |
| `SymbolData.cs` | ScriptableObject — each symbol has its own asset with a sprite, multiplier, and weight |

---

## How Things Work (Beginner Friendly)

### RNG (Random Number Generator)
Each symbol has a `weight` number in its ScriptableObject asset. A higher weight means it shows up more often. When the reel spins, the code picks a random number and finds which symbol it lands on based on weight. To make a symbol rarer, lower its weight. To make it more common, raise it.

### Spin Animation
Each reel runs in two phases:
1. **Fast phase** — random symbols flash quickly to look like a blur
2. **Slow phase** — the reel eases out and snaps to the final result

Reels stop 0.4 seconds apart so there's a left-to-right reveal effect.

### Audio
`AudioManager` uses a **singleton** pattern — meaning only one exists at a time and it carries over between scenes. Any script can access it from anywhere using `AudioManager.Instance`. It plays separate music tracks for the main menu and the game.

---

## Features

- Main menu with Play and Quit buttons
- Main menu background music
- In-game background music (switches automatically when game loads)
- Click sound on every button
- Bet menu with three bet sizes (10 / 50 / 100)
- Quit button in bet menu returns to main menu
- Weighted random symbol system
- Staggered reel stop animation
- Win popup with bounce animation and coin counter
- Confetti particle effect on wins
- Coin rain effect on wins
- Game Over screen when coins run out
- Play Again reloads the scene

---

## Requirements

- Unity 2022.3 LTS or newer
- TextMeshPro — install via **Window > Package Manager** if not already installed
- Python (only needed to run the WebGL build locally)