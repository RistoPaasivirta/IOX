using UnityEngine;

public class HealSkill : Skill
{
    [SerializeField] private float ActiveTime = 10f;
    [SerializeField] private int ChargeCost = 20;
    [SerializeField] private float Portion = .1f;
    [SerializeField] private float SoftDamageGenerationMultiplier = 0f;
    [SerializeField] private Sounds.SoundDef ActivationSound = new Sounds.SoundDef();
    [SerializeField] private Sounds.SoundDef FailSound = new Sounds.SoundDef();
    [SerializeField] private GameObject[] UpgradesInto = new GameObject[0];

    public override GameObject[] Upgrades => UpgradesInto;

    MonsterCharacter targetCharacter;
    int healPerTick;
    float timer;

    public override string GetShortStats
    {
        get
        {
            return
                ItemName + System.Environment.NewLine +
                System.Environment.NewLine +
                "Activate to heal " + (int)(Portion * 100) + "% of accumulated damage over " + ActiveTime + " seconds" + System.Environment.NewLine +
                System.Environment.NewLine +
                "Costs " + (ChargeCost / 1000) + " charge." + System.Environment.NewLine +
                System.Environment.NewLine +
                (SoftDamageGenerationMultiplier == 0f ? "Prevents you from gaining more accumulated damage during healing." + System.Environment.NewLine : "") +
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

        if (character.ChargePoints < ChargeCost)
        {
            Messaging.GUI.ScreenMessage.Invoke("NOT ENOUGH CHARGE", Color.red);
            Sounds.CreateSound(FailSound);
            return;
        }

        if (character.SoftDamage <= 0)
        {
            Messaging.GUI.ScreenMessage.Invoke("NO ACCUMULATED DAMAGE", Color.red);
            Sounds.CreateSound(FailSound);
            return;
        }

        int HealPerTick = (int)(character.SoftDamage * Portion / ActiveTime / 50);

        if (HealPerTick <= 0)
        {
            Messaging.GUI.ScreenMessage.Invoke("NOT ENOUGH ACCUMULATED DAMAGE", Color.red);
            Sounds.CreateSound(FailSound);
            return;
        }

        Sounds.CreateSound(ActivationSound);

        HealSkill s = Instantiate(this, character.transform);
        s.targetCharacter = character;
        s.healPerTick = HealPerTick;

        character.ChargePoints -= ChargeCost;
        character.SoftDamage = 0;
        character.SkillCooldowns[index] = ActiveTime;
    }

    private void FixedUpdate() =>
        targetCharacter.HitPoints += healPerTick;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= ActiveTime)
            Destroy(gameObject);
    }

    private void Start() =>
        targetCharacter.GenerateSoftDamage.AddListener((f) => { f.amount = (int)(f.amount * SoftDamageGenerationMultiplier); });

}
