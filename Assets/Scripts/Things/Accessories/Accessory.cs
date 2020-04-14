using UnityEngine;

public abstract class Accessory : MonoBehaviour, InventoryGUIObject
{
    [SerializeField] protected string ItemName = "Accessory";
    [SerializeField] private string InventoryDesignation = "null";
    [SerializeField] private Sprite CursorIcon = null;
    [SerializeField] private Sprite InventoryIcon = null;
    [SerializeField] private Color MainColor = new Color(.5f, 1f, .3f);
    [SerializeField] private Color SecondaryColor = new Color(0f, .5f, 0f);
    [SerializeField] private CraftingCost AssembleCost = new CraftingCost(0, 0, 0, 0);
    [SerializeField] private CraftingCost DisassembleExtra = new CraftingCost(0, 0, 0, 0);

    public virtual void Activate(MonsterCharacter character) { }
    public virtual void DeActivate(MonsterCharacter character) { }
    public virtual Lump ToLump() => new Lump(InventoryDesignation, new byte[0]);

    CraftingCost InventoryGUIObject.AssembleCost => AssembleCost;
    CraftingCost InventoryGUIObject.DisassembleProfit => AssembleCost + DisassembleExtra;
    Sprite InventoryGUIObject.InventoryIcon => InventoryIcon;
    Sprite InventoryGUIObject.CursorIcon => CursorIcon;
    string InventoryGUIObject.ItemName => ItemName;
    Color InventoryGUIObject.MainColor => MainColor;
    Color InventoryGUIObject.SecondaryColor => SecondaryColor;
    Lump InventoryGUIObject.Serialize() => ToLump();
    public abstract string GetShortStats { get; }
    public virtual GameObject[] Upgrades { get { return new GameObject[0]; } }
}

