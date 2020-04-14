using UnityEngine;

public class DashSkill : Skill
{
    [SerializeField] private float Strength = 10f;
    [SerializeField] private float ActiveTime = .2f;
    [SerializeField] private int ChargeCost = 30;
    [SerializeField] private float Cooldown = .2f;
    [SerializeField] private Sounds.SoundDef ActivationSound = new Sounds.SoundDef();
    [SerializeField] private Sounds.SoundDef FailSound = new Sounds.SoundDef();
    [SerializeField] private GameObject AttachmentEffect = null;
    [SerializeField] private string[] AttachmentPoints = new string[0];
    [SerializeField] private GameObject[] UpgradesInto = new GameObject[0];

    public override GameObject[] Upgrades => UpgradesInto;

    Vector2 direction;
    MonsterCharacter targetCharacter;

    public override string GetShortStats
    {
        get
        {
            return
                ItemName + System.Environment.NewLine +
                System.Environment.NewLine +
                "Short duration speed boost. Costs " + (ChargeCost/1000) + " charge and has a very short cooldown." + System.Environment.NewLine+
                (UpgradesInto.Length > 0 ? System.Environment.NewLine + "Can be augmented" : "");
        }
    }

    public override void Activate(MonsterCharacter character, int index)
    {
        //would result in illegal normalization, need a direction to dash into
        if (character.physicsBody.velocity == Vector2.zero)
            return;

        //need 65% of full power at minimum
        if (character.ChargePoints < (ChargeCost * .65f))
        {
            Sounds.CreateSound(FailSound);
            Messaging.GUI.ScreenMessage.Invoke("NOT ENOUGH CHARGE", Color.red);
            return;
        }

        if (character.SkillCooldowns[index] > 0)
        {
            Sounds.CreateSound(FailSound);
            return;
        }

        Sounds.CreateSound(ActivationSound);

        character.SkillCooldowns[index] = Cooldown;

        float strength = Strength;

        if (character.ChargePoints < ChargeCost)
        {
            strength *= (float)character.ChargePoints / ChargeCost;
            character.ChargePoints = 0;
        }
        else
            character.ChargePoints -= ChargeCost;

        DashSkill d = Instantiate(this, LevelLoader.DynamicObjects);
        d.Strength = strength;
        d.direction = character.physicsBody.velocity.normalized;
        d.targetCharacter = character;
        d.timer = ActiveTime;

        if (AttachmentEffect != null)
            foreach(string attachPoint in AttachmentPoints)
            {
                GameObject effect = Instantiate(AttachmentEffect);

                GenericTimer e = effect.AddComponent<GenericTimer>();
                e.LifeTime = ActiveTime * 5;
                e.OnTimer.AddListener(() => 
                {
                    TrailRenderer tr = effect.GetComponentInChildren<TrailRenderer>();
                    if (tr != null)
                        tr.emitting = false;
                });

                DestroyAfterTime t = effect.AddComponent<DestroyAfterTime>();
                t.LifeTime = ActiveTime * 10;

                effect.transform.SetParent(character.GetAttachmentPoint(attachPoint), false);
            }
    }

    float timer;
    private void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;

            if (targetCharacter != null)
                targetCharacter.impulseVector += direction * Strength;
        }
        else
            Destroy(gameObject);
    }

}
