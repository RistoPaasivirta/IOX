using UnityEngine;
using UnityEngine.Events;

public class ShieldSkill : Skill
{
    [SerializeField] private float ChargeRechargeMultiplier = .5f;
    [SerializeField] private int ChargeCostPerSecond = 10;
    [SerializeField] private float DamageReduction = .2f;
    [SerializeField] private float LingerTime = .4f;
    [SerializeField] private Sounds.SoundDef ActivationSound = new Sounds.SoundDef();
    [SerializeField] private Sounds.SoundDef DeactivationSound = new Sounds.SoundDef();
    [SerializeField] private Sounds.SoundDef FailSound = new Sounds.SoundDef();
    [SerializeField] private GameObject[] UpgradesInto = new GameObject[0];

    public UnityEvent OnDeactivateInstance = new UnityEvent();
    public override GameObject[] Upgrades => UpgradesInto;

    MonsterCharacter targetCharacter;
    int skillIndex;
    float costTimer;
    float timer = 0f;
    bool GoingDown = false;

    public override string GetShortStats
    {
        get
        {
            return
                ItemName + System.Environment.NewLine +
                System.Environment.NewLine +
                "Shield that grants " + (int)(DamageReduction*100) + "% damage reduction." + System.Environment.NewLine +
                System.Environment.NewLine + "Costs " + (ChargeCostPerSecond / 1000) + " charge per second and "+
                "reduces charge regeneration by "+ Mathf.RoundToInt((1f-ChargeRechargeMultiplier) * 100)+ "%" + System.Environment.NewLine +
                (UpgradesInto.Length > 0 ? System.Environment.NewLine + "Can be augmented" : "");
        }
    }

    public override void Activate(MonsterCharacter character, int index)
    {
        if (character.SkillCooldowns[index] > 0)
        {
            if (character.Buffs.ContainsKey((int)StatusEffect.Shield))
            {
                character.Buffs[(int)StatusEffect.Shield].GetComponent<Skill>().DeActivateInstance();
                character.Buffs.Remove((int)StatusEffect.Shield);
            }
        }
        else
        {
            if (character.ChargePoints < ChargeCostPerSecond)
                return;

            Sounds.CreateSound(ActivationSound);

            ShieldSkill s = Instantiate(this, character.transform);
            s.targetCharacter = character;
            s.skillIndex = index;
        }
    }

    public override void DeActivate(MonsterCharacter character, int index)
    {
        if (character.SkillCooldowns[index] > 0)
        {
            if (character.Buffs.ContainsKey((int)StatusEffect.Shield))
            {
                character.Buffs[(int)StatusEffect.Shield].GetComponent<Skill>().DeActivateInstance();
                character.Buffs.Remove((int)StatusEffect.Shield);
            }
        }
    }

    public override void DeActivateInstance()
    {
        GoingDown = true;
        Sounds.CreateSound(DeactivationSound);
        OnDeactivateInstance.Invoke();
    }

    private void OnDestroy()
    {
        if (targetCharacter.Buffs.ContainsKey((int)StatusEffect.Shield))
            if (targetCharacter.Buffs[(int)StatusEffect.Shield] == gameObject)
                targetCharacter.Buffs.Remove((int)StatusEffect.Shield);
    }

    private void Start()
    {
        if (targetCharacter.Buffs.ContainsKey((int)StatusEffect.Shield))
        {
            targetCharacter.Buffs[(int)StatusEffect.Shield].GetComponent<Skill>().DeActivateInstance();
            targetCharacter.Buffs.Remove((int)StatusEffect.Shield);
        }

        targetCharacter.Buffs.Add((int)StatusEffect.Shield, gameObject);
        targetCharacter.OnDamage.AddListener((f) => { f.amount = (int)(f.amount * (1f - DamageReduction)); });
        targetCharacter.OnChargeRegen.AddListener((f) => { f.RechargeAmount = (int)(f.RechargeAmount * ChargeRechargeMultiplier); });
    }

    private void Update()
    {
        targetCharacter.SkillCooldowns[skillIndex] = LingerTime;

        if (GoingDown)
        {
            timer += Time.deltaTime;
            if (timer > LingerTime)
            {
                targetCharacter.SkillCooldowns[skillIndex] = 0f;
                Destroy(gameObject);
            }
        }

        if (!GoingDown)
        {
            costTimer -= Time.deltaTime;
            if (costTimer <= 0)
            {
                costTimer += 1f;

                if (targetCharacter.ChargePoints < ChargeCostPerSecond)
                    DeActivateInstance();

                targetCharacter.ChargePoints -= ChargeCostPerSecond;
            }
        }
    }
}