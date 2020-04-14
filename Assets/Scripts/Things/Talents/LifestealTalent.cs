using UnityEngine;

public class LifestealTalent : Talent
{
    [SerializeField] private int Percent = 20;

    MonsterCharacter owner;

    public override void Activate(MonsterCharacter character)
    {
        owner = character;
        character.GenerateSoftDamage.AddListener(SoftDamageMod);
        character.OnDealDamage.AddListener(DealDamageMod);
    }

    public override void DeActivate(MonsterCharacter character)
    {
        character.GenerateSoftDamage.RemoveListener(SoftDamageMod);
        character.OnDealDamage.RemoveListener(DealDamageMod);
        owner = null;
    }

    void SoftDamageMod (MonsterCharacter.SoftDamageFrame frame) =>
        frame.amount = 0;

    void DealDamageMod(int amount) =>
        owner.SoftDamage += amount * Percent / 100;
}
