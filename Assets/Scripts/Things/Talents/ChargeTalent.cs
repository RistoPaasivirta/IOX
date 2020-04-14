public class ChargeTalent : Talent
{
    public override void Activate(MonsterCharacter character)
    {
        character.MaxChargePoints /= 2;
        character.OnChargeRegen.AddListener(TalentBonus);

        if (character.ChargePoints > character.MaxChargePoints)
            character.ChargePoints = character.MaxChargePoints;
    }

    public override void DeActivate(MonsterCharacter character)
    {
        character.MaxChargePoints *= 2;
        character.OnChargeRegen.RemoveListener(TalentBonus);
    }

    void TalentBonus(MonsterCharacter.ChargeRegenFrame frame) =>
        frame.RechargeAmount *= 2;
}
