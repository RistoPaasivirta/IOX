using UnityEngine;

public class SpeedAccessory : Accessory
{
    [SerializeField] private int ChargeBonus = 10;
    [SerializeField] private int MoveBonus = 5;
    [SerializeField] private bool RechargesCells = false;
    [SerializeField] private int ChargeCostForRecharge = 100;
    [SerializeField] private GameObject[] UpgradesInto = new GameObject[0];

    public override GameObject[] Upgrades => UpgradesInto;

    MonsterCharacter owner;

    public override void Activate(MonsterCharacter character)
    {
        owner = character;
        character.walkSpeed += MoveBonus;
        character.OnChargeRegen.AddListener(ChargeBoost);
    }

    public override void DeActivate(MonsterCharacter character)
    {
        character.walkSpeed -= MoveBonus;
        character.OnChargeRegen.RemoveListener(ChargeBoost);
        owner = null;
    }

    void ChargeBoost(MonsterCharacter.ChargeRegenFrame frame)
    {
        if (RechargesCells)
            if (owner.ammunition[5] < owner.maxAmmo[5])
                if (owner.ChargePoints >= owner.MaxChargePoints)
                {
                    owner.ChargePoints -= ChargeCostForRecharge;
                    owner.ammunition[5]++;
                }

        frame.RechargeAmount *= 100 + ChargeBonus;
        frame.RechargeAmount /= 100;
    }

    public override string GetShortStats
    {
        get
        {
            return
                ItemName + System.Environment.NewLine +
                System.Environment.NewLine +
                "MOVEMENT SPEED" + System.Environment.NewLine +
                "+" + MoveBonus + System.Environment.NewLine +
                System.Environment.NewLine +
                "CHARGE REGENERATION" + System.Environment.NewLine +
                "+" + ChargeBonus + System.Environment.NewLine +
                (RechargesCells ? System.Environment.NewLine + "When in full charge gives you one cell ammo for " + (ChargeCostForRecharge/1000) + " charge.": "") + System.Environment.NewLine +
                (UpgradesInto.Length > 0 ? System.Environment.NewLine + "Can be augmented" : "");
        }
    }
}
