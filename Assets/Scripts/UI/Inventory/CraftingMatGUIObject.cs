using UnityEngine;

public class CraftingMatGUIObject : MonoBehaviour, InventoryGUIObject
{
    [SerializeField] private string InventoryDesignation = "null";
    [SerializeField] private string ItemName = "Dummy";
    [SerializeField] private Sprite CursorIcon = null;
    [SerializeField] private Sprite InventoryIcon = null;
    [SerializeField] private Color MainColor = new Color(1f, 1f, 1f);
    [SerializeField] private Color SecondaryColor = new Color(0f, 0f, 0f);
    [SerializeField] private CraftingCost AssembleCost = new CraftingCost(0, 0, 0, 0);

    public CraftingCost craftingMats = new CraftingCost(1, 0, 0, 0);

    CraftingCost InventoryGUIObject.AssembleCost => AssembleCost;
    CraftingCost InventoryGUIObject.DisassembleProfit => AssembleCost;
    Sprite InventoryGUIObject.InventoryIcon => InventoryIcon;
    Sprite InventoryGUIObject.CursorIcon => CursorIcon;
    string InventoryGUIObject.ItemName => ItemName;
    Color InventoryGUIObject.MainColor => MainColor;
    Color InventoryGUIObject.SecondaryColor => SecondaryColor;
    Lump InventoryGUIObject.Serialize() => new Lump(InventoryDesignation, new byte[0]);
    string InventoryGUIObject.GetShortStats => "";
    GameObject[] InventoryGUIObject.Upgrades => new GameObject[0];
}
