using UnityEngine;

public interface InventoryGUIObject
{
    string ItemName { get; }
    Sprite InventoryIcon { get; }
    Sprite CursorIcon { get; }
    Color MainColor { get; }
    Color SecondaryColor { get; }
    Lump Serialize();
    CraftingCost AssembleCost { get; }
    CraftingCost DisassembleProfit { get; }
    string GetShortStats { get; }
    GameObject[] Upgrades { get; }
}
