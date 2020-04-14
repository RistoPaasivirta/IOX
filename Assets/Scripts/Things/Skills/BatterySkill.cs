using UnityEngine;

public class BatterySkill : Skill
{
    [SerializeField] private int ChargeRecharge = 100;
    [SerializeField] private int CellsCost = 20;
    [SerializeField] private float Cooldown = 5f;
    [SerializeField] private Sounds.SoundDef ActivationSound = new Sounds.SoundDef();
    [SerializeField] private Sounds.SoundDef FailSound = new Sounds.SoundDef();
    [SerializeField] private GameObject EffectPrefab = null;
    [SerializeField] private GameObject[] UpgradesInto = new GameObject[0];

    public override GameObject[] Upgrades => UpgradesInto;

    public override string GetShortStats
    {
        get
        {
            return
                ItemName + System.Environment.NewLine +
                System.Environment.NewLine +
                "Activate to regain " + (ChargeRecharge / 1000) + " charge instantly." + System.Environment.NewLine +
                System.Environment.NewLine +
                "Costs " + CellsCost + " cells." + System.Environment.NewLine +
                System.Environment.NewLine +
                "Cooldown " + Cooldown.ToString("F1") + " seconds" + System.Environment.NewLine+
                (UpgradesInto.Length > 0 ? System.Environment.NewLine + "Can be augmented" : "");
        }
    }

    public override void Activate(MonsterCharacter character, int index)
    {
        if (character.SkillCooldowns[index] > 0)
        {
            Sounds.CreateSound(FailSound);
            return;
        }

        if (character.ChargePoints >= character.MaxChargePoints)
        {
            Messaging.GUI.ScreenMessage.Invoke("AT MAXIMUM CHARGE", Color.red);
            return;
        }

        if (character.ammunition[5] < CellsCost)
        {
            Messaging.GUI.ScreenMessage.Invoke("NOT ENOUGH CELLS", Color.red);
            Sounds.CreateSound(FailSound);
            return;
        }

        Sounds.CreateSound(ActivationSound);

        character.ChargePoints += ChargeRecharge;
        character.ammunition[5] -= CellsCost;
        character.SkillCooldowns[index] = Cooldown;

        if (EffectPrefab != null)
            Instantiate(EffectPrefab, character.transform.position, Quaternion.identity, LevelLoader.TemporaryObjects);
    }
}
