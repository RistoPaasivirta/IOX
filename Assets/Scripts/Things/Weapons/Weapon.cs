using UnityEngine;
using UnityEngine.Events;

public abstract class Weapon : MonoBehaviour, InventoryGUIObject
{
    [SerializeField] private float raiseTime = .5f;
    [SerializeField] private float holsterTime = .5f;
    [SerializeField] private string InventoryDesignation = "null";
    [SerializeField] private Sprite CursorIcon = null;
    [SerializeField] private Color MainColor = new Color(1f, .3f, .3f);
    [SerializeField] private Color SecondaryColor = new Color(.6f, 0f, 0f);
    [SerializeField] private CraftingCost AssembleCost = new CraftingCost(0, 0, 0, 0);
    [SerializeField] private CraftingCost DisassembleExtra = new CraftingCost(0, 0, 0, 0);
    [SerializeField] private MonsterCharacter.Animations HolsterAnimation = MonsterCharacter.Animations.Holster;
    [SerializeField] protected string ItemName = "Weapon";
    [SerializeField] protected Vector2 AttackDirection = Vector2.zero;

    public Sprite LeftAbilitySprite = null;
    public Sprite RightAbilitySprite = null;
    public UnityEvent OnFire = new UnityEvent();
    public bool TwoHanded = true;
    public float HolsterTimer;
    public float RaiseTimer;
    public Sprite InventoryIcon = null;

    protected MonsterCharacter character;

    public virtual void Equip(MonsterCharacter caller)
    {
        character = caller;
        RaiseTimer = raiseTime / character.AnimationSpeedMultiplier;
    }

    public virtual void Holster()
    {
        HolsterTimer = holsterTime / character.AnimationSpeedMultiplier;
        character.CallAnimation(HolsterAnimation);
    }

    public virtual void UnEquip() { }
    public virtual bool FireTrigger() { return false; }
    public virtual bool FireContinuous() { return false; }
    public virtual bool AltFireTrigger() { return false; }
    public virtual bool AltFireContinuous() { return false; }
    protected virtual void OnUpdate() { }

    private void Update()
    {
        if (TimeScaler.Paused)
            return;

        if (HolsterTimer > 0f)
            HolsterTimer -= Time.deltaTime;

        if (RaiseTimer > 0f)
            RaiseTimer -= Time.deltaTime;

        OnUpdate();
    }

    public virtual Vector2 AttackRayStartPosition { get { return character.transform.position; } }

    public void PointTowards(Vector2 target)
    {
        Vector2 start = AttackRayStartPosition;
        if (start == target)
            AttackDirection = Vector2.zero;
        else
        {
            AttackDirection = (target - start).normalized;
            AttackDirection = new Vector2(AttackDirection.y, -AttackDirection.x); //since we use X=forward we need to rotate 90 degrees clockwise
        }
    }

    public virtual bool IsReady { get { return true; } }
    public virtual Lump ToLump() { return new Lump(InventoryDesignation, new byte[0]); }

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
