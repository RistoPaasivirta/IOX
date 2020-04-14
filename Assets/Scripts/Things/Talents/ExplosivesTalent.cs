public class ExplosivesTalent : Talent
{
    MonsterCharacter owner;

    public override void Activate(MonsterCharacter character)
    {
        owner = character;
        character.OnAmmoPickup.AddListener(AmmoPickupBonus);
    }

    public override void DeActivate(MonsterCharacter character)
    {
        character.OnAmmoPickup.RemoveListener(AmmoPickupBonus);
        owner = null;
    }

    void AmmoPickupBonus(int ammo)
    {
        if (ammo == 3)
            if (owner.ammunition[3] < owner.maxAmmo[3])
                owner.ammunition[3] += 1;
    }
}
