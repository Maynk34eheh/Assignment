using UnityEngine;

// Holds the data for one slot symbol (like a cherry or a bar)
// Right-click in the Project window to create one: SlotMachine > SymbolData
[CreateAssetMenu(fileName = "SymbolData", menuName = "SlotMachine/SymbolData")]
public class SymbolData : ScriptableObject
{
    public SymbolType symbolType;
    public Sprite sprite;

    // How many times the bet is multiplied on a 3-of-a-kind win
    public int payoutMultiplier;

    // How often this symbol appears — higher number means more common
    [Range(1, 100)]
    public int weight = 10;
}

public enum SymbolType
{
    Symbol1 = 0,
    Symbol2 = 1,
    Symbol3 = 2,
    Symbol4 = 3
}
