using UnityEngine;

public class ArmorAccessory : Accessory
{
    /*public UpgradeableIntStat ArmorAmount = new UpgradeableIntStat(
        "Armor Bonus",
        new int[3] { 5, 10, 15 },
        new CraftingCost[3] { new CraftingCost(1, 1, 0, 0), new CraftingCost(2, 2, 0, 0), new CraftingCost(3, 3, 0, 1) });

    public UpgradeableIntStat DecayBonus = new UpgradeableIntStat(
        "Damage Decay Reduction",
        new int[3] { 10, 20, 30 },
        new CraftingCost[3] { new CraftingCost(1, 1, 0, 0), new CraftingCost(2, 2, 0, 0), new CraftingCost(3, 3, 0, 1) });

    public override UpgradeableIntStat[] GetUpgradeableStats =>
        new UpgradeableIntStat[2]{ ArmorAmount, DecayBonus };*/

    [SerializeField] private int ArmorAmount = 5;
    [SerializeField] private int DecayBonus = 10;
    [SerializeField] private int SoftDamageDelay = 0;
    [SerializeField] private GameObject[] UpgradesInto = new GameObject[0];

    public override GameObject[] Upgrades => UpgradesInto;

    MonsterCharacter owner;

    public override void Activate(MonsterCharacter character)
    {
        owner = character;
        character.Armor += ArmorAmount;
        character.SoftDamageDecayPerTick -= DecayBonus;
        if (SoftDamageDelay > 0)
            character.GenerateSoftDamage.AddListener(SoftDamageCooldown);
    }

    public override void DeActivate(MonsterCharacter character)
    {
        character.Armor -= ArmorAmount;
        character.SoftDamageDecayPerTick += DecayBonus;
        if (SoftDamageDelay > 0)
            character.GenerateSoftDamage.RemoveListener(SoftDamageCooldown);
        owner = null;
    }

    void SoftDamageCooldown(MonsterCharacter.SoftDamageFrame _) =>
        owner.SoftDamageDelay = SoftDamageDelay;

    public override string GetShortStats
    {
        get
        {
            return
                ItemName + System.Environment.NewLine +
                System.Environment.NewLine +
                "ARMOR BONUS" + System.Environment.NewLine +
                "+" + ArmorAmount + System.Environment.NewLine +
                System.Environment.NewLine +
                "DAMAGE DECAY REDUCTION" + System.Environment.NewLine +
                "+" + DecayBonus + System.Environment.NewLine +
                (SoftDamageDelay > 0 ? System.Environment.NewLine + "When hit your accumulated damage decay is delayed for " + ((float)SoftDamageDelay / 50f).ToString("F1") + " seconds." + System.Environment.NewLine : "")+
                (UpgradesInto.Length > 0 ? System.Environment.NewLine + "Item can be augmented" : "");
        }
    }
}
