# Slot Machine Game — Unity

A slot machine game made in Unity with spinning reel animations, a weighted random system, win detection, and a full UI.

## How the Game Works

You start with **1000 coins**. Pick a bet amount, press SPIN, and watch the three reels spin. If all three land on the same symbol, you win coins based on that symbol's multiplier. The game ends when you can't afford the minimum bet.

### Symbols & Payouts

| Symbol | Rarity | Payout (3-of-a-kind)

| Symbol 1 | Very common (weight: 40)    | 2× your bet
| Symbol 2 | Common (weight: 35)         | 5× your bet 
| Symbol 3 | Uncommon (weight: 15)       | 10× your bet 
| Symbol 4 | Rare / jackpot (weight: 10) | 20× your bet 


## How the Code Works

**SlotMachineController.cs** runs the game loop. It calls the RNG, tells each reel to spin, waits for them all to stop, then checks if the player won. It also contains the `SlotRNG` class which handles weighted random symbol picking.

**ReelController.cs** handles the spin animation for one reel. It runs in two phases: a fast blur of random symbols, then a slow ease-out that snaps to the final result.

**PlayerData.cs** stores the player's coins and current bet. No Unity code in here — just plain data and simple methods.

**UIManager.cs** updates everything the player sees: the coin counter, win popup, coin rain effect, bet menu, and game over screen.

**AudioManager.cs** plays background music and all sound effects. It uses a singleton so any script can call it with `AudioManager.Instance`.

**HandleController.cs** swaps between the normal and pulled lever sprites.

**SymbolData.cs** is a ScriptableObject — each symbol in the game has its own asset with a sprite, payout multiplier, and weight value.

### How the RNG works

Each symbol has a `weight` number. Higher weight = appears more often. When spinning, the code picks a random number and walks through the list until it finds a match. To make a symbol rarer or more common, just change its weight in the Inspector.

### How the spin animation works

Each reel runs as a coroutine with two phases:
1. **Fast phase** — random symbols flash quickly to look like a blur
2. **Ease-out phase** — the reel slows down using quadratic easing and snaps to the final symbol

Reels stop 0.4 seconds apart by default to create a left-to-right reveal effect.

## Features
- Weighted random system (easy to tweak symbol odds)
- Staggered reel stop animation
- Win popup with bounce animation
- Coin counter that counts up to your winnings
- Confetti particle effect on wins
- Coin rain effect on wins
- Background music and sound effects
- Bet menu with three bet sizes (10 / 50 / 100)
- Game Over screen when coins run out
- Play Again reloads the scene


## Requirements
- Unity 2022.3 LTS or newer
- TextMeshPro (install via Window > Package Manager)
