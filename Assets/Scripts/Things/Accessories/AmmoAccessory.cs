using UnityEngine;

public class AmmoAccessory : Accessory
{
    [SerializeField] private int[] ammoBonus = new int[6] { 100, 50, 10, 2, 100, 20 };
    [SerializeField] private int[] pickupBonus = new int[6] { 10, 5, 2, 0, 20, 5 };
    [SerializeField] private GameObject[] UpgradesInto = new GameObject[0];

    public override GameObject[] Upgrades => UpgradesInto;

    MonsterCharacter owner;

    public override void Activate(MonsterCharacter character)
    {
        owner = character;

        for(int i = 0; i < Mathf.Min(ammoBonus.Length, character.maxAmmo.Length); i++)
            character.maxAmmo[i] += ammoBonus[i];

        character.OnAmmoPickup.AddListener(AmmoBonus);
    }

    public override void DeActivate(MonsterCharacter character)
    {
        for (int i = 0; i < Mathf.Min(ammoBonus.Length, character.maxAmmo.Length); i++)
            character.maxAmmo[i] -= ammoBonus[i];

        character.OnAmmoPickup.RemoveListener(AmmoBonus);

        owner = null;
    }

    public void AmmoBonus(int a) =>
        owner.ammunition[a] = Mathf.Min(owner.ammunition[a] + pickupBonus[a], owner.maxAmmo[a]);

    public override string GetShortStats
    {
        get
        {
            return
                ItemName + System.Environment.NewLine +
                System.Environment.NewLine +
                "AMMO CAPACITY" + System.Environment.NewLine +
                "Short rounds: +" + ammoBonus[0] + System.Environment.NewLine +
                "Long rounds: +" + ammoBonus[1] + System.Environment.NewLine +
                "Shells: +" + ammoBonus[2] + System.Environment.NewLine +
                "Rockets: +" + ammoBonus[3]+ System.Environment.NewLine +
                "Fuel: +" + ammoBonus[4] + System.Environment.NewLine +
                "Cells: +" + ammoBonus[5] + System.Environment.NewLine +
                System.Environment.NewLine +
                "AMMO PICKUP" + System.Environment.NewLine +
                "Short rounds: +" + pickupBonus[0] + System.Environment.NewLine +
                "Long rounds: +" + pickupBonus[1] + System.Environment.NewLine +
                "Shells: +" + pickupBonus[2] + System.Environment.NewLine +
                "Rockets: +" + pickupBonus[3] + System.Environment.NewLine +
                "Fuel: +" + pickupBonus[4] + System.Environment.NewLine +
                "Cells: +" + pickupBonus[5] +
                (UpgradesInto.Length > 0 ? System.Environment.NewLine + "Item can be augmented" : "");
        }
    }
}
