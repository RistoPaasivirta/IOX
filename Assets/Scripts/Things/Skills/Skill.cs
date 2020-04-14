using UnityEngine;

public abstract class Skill : MonoBehaviour, InventoryGUIObject
{
    [SerializeField] protected string ItemName = "Skill";
    [SerializeField] private string InventoryDesignation = "null";
    [SerializeField] private Sprite CursorIcon = null;
    [SerializeField] private Sprite InventoryIcon = null;
    [SerializeField] private Color MainColor = new Color(.5f, .6f, 1f);
    [SerializeField] private Color SecondaryColor = new Color(0f, 0f, 1f);
    [SerializeField] private CraftingCost AssembleCost = new CraftingCost(0, 0, 0, 0);
    [SerializeField] private CraftingCost DisassembleExtra = new CraftingCost(0, 0, 0, 0);

    public virtual void Activate(MonsterCharacter character, int index) { }
    public virtual void DeActivate(MonsterCharacter character, int index) { }
    public virtual void DeActivateInstance() { }
    public virtual Lump ToLump() { return new Lump(InventoryDesignation, new byte[0]); }

    public float ReferenceCooldown = 1f;
    public bool ActiveInsteadOfCooldown = false;
    public Sprite AbilityIcon;

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
