using UnityEngine;

public class GrenadeSkill : Skill
{
    [SerializeField] private string GrenadeDesignation = "";
    [SerializeField] private string AttachmentPoint = "LeftWeapon";
    [SerializeField] private float spread = 5f;
    [SerializeField] private int AmmoType = 3;
    [SerializeField] private int AmmoAmount = 1;
    [SerializeField] private float Cooldown = .4f;
    [SerializeField] private int Damage = 100;
    [SerializeField] private DamageType damageType = DamageType.Physical;
    [SerializeField] private float LifeTime = 1.5f;
    [SerializeField] private float Force = 400f;
    [SerializeField] private float ArmDistance = 0f;
    [SerializeField] private Vector2 ExplosionRadius = new Vector2(2f, 4f);
    [SerializeField] private Vector2 ArmTimeRange = new Vector2(.2f, .5f);
    [SerializeField] private Sounds.SoundDef ActivationSound = new Sounds.SoundDef();
    [SerializeField] private Sounds.SoundDef FailSound = new Sounds.SoundDef();
    [SerializeField] private GameObject[] UpgradesInto = new GameObject[0];
    [SerializeField] private Vector2 AttackDistance = new Vector2(3, 20);
    [SerializeField] private Vector2 ThrowSpeed = new Vector2(6, 30);
    [SerializeField] private float AnimationDelay = .2f;
    [SerializeField] private float Delay = .3f;

    public override GameObject[] Upgrades => UpgradesInto;

    MonsterCharacter targetCharacter;
    bool animationReturn = false;
    float timer = 0f;
    bool thrown = false;
    float speed;

    public override string GetShortStats
    {
        get
        {
            return
                ItemName + System.Environment.NewLine +
                System.Environment.NewLine +
                "Throw a grenade that deals " + (Damage / 1000) + " damage in a " + ExplosionRadius.y + " radius." + System.Environment.NewLine +
                System.Environment.NewLine +
                "Cooldown " + Cooldown + " seconds" + System.Environment.NewLine +
                System.Environment.NewLine +
                "Uses a rocket" + System.Environment.NewLine +
                (UpgradesInto.Length > 0 ? System.Environment.NewLine + "Can be augmented" : "");
        }
    }

    public override void Activate(MonsterCharacter character, int index)
    {
        if (string.IsNullOrEmpty(GrenadeDesignation))
            return;

        if (character.SkillCooldowns[index] > 0)
        {
            Sounds.CreateSound(FailSound);
            return;
        }

        if (AmmoType != -1)
        {
            if (character.ammunition[AmmoType] < AmmoAmount)
            {
                Messaging.GUI.ScreenMessage.Invoke("OUT OF AMMO", Color.red);
                Sounds.CreateSound(FailSound);
                return;
            }

            character.ammunition[AmmoType] -= AmmoAmount;
        }

        Sounds.CreateSound(ActivationSound);

        character.SkillCooldowns[index] = Cooldown;

        GrenadeSkill g = Instantiate(this, LevelLoader.DynamicObjects);
        g.targetCharacter = character;

        Mouse.UpdateWorldPosition();
        float distance = (Mouse.WorldPosition - character.transform.position).magnitude;
        g.speed = Mathf.Lerp(ThrowSpeed.x, ThrowSpeed.y, Mathf.InverseLerp(AttackDistance.x, AttackDistance.y, distance));
    }

    private void Start() =>
        targetCharacter.CallAnimation(MonsterCharacter.Animations.LeftArmGrenadeThrow);

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= Cooldown)
            Destroy(gameObject);

        if (!animationReturn && timer > AnimationDelay)
        {
            targetCharacter.CallAnimation(targetCharacter.CurrentWeaponAnimation);
            animationReturn = true;
        }

        if (!thrown && timer > Delay)
        {
            thrown = true;

            if (!ThingDesignator.Designations.ContainsKey(GrenadeDesignation))
            {
                Debug.LogError("GrenadeSkill \"" + gameObject.name + "\" grenade designation \"" + GrenadeDesignation + "\" not found in designator");
                return;
            }

            GameObject prefab = ThingDesignator.Designations[GrenadeDesignation];
            Grenade GrenadePrefab = prefab.GetComponent<Grenade>();
            if (GrenadePrefab == null)
            {
                Debug.LogError("GrenadeSkill \"" + gameObject.name + "\" grenade designation \"" + GrenadeDesignation + "\" has no <Grenade> component");
                return;
            }

            Grenade grenade = Instantiate(GrenadePrefab, LevelLoader.DynamicObjects);
            grenade.owner = targetCharacter;
            grenade.Damage = Damage;
            grenade.damageType = damageType;
            grenade.Force = Force;
            grenade.LifeTime = LifeTime;
            grenade.ArmDistance = ArmDistance;
            grenade.ArmTimeRange = ArmTimeRange;
            grenade.Speed = speed;
            grenade.ExplosionRadius = ExplosionRadius;

            //make a special raycasting move from character to barrel
            grenade.transform.position = targetCharacter.GetAttachmentPoint(AttachmentPoint).position;

            Mouse.UpdateWorldPosition();
            Vector2 AttackDirection = Mouse.WorldPosition - targetCharacter.transform.position;

            if (AttackDirection == Vector2.zero)
                grenade.transform.rotation = targetCharacter.LookDirection;
            else
                grenade.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(AttackDirection.y, AttackDirection.x) * Mathf.Rad2Deg);

            //add spread
            float firstRoll = Random.Range(-spread, spread);
            float secondRoll = Random.Range(-spread, spread);
            grenade.transform.rotation *= Quaternion.Euler(0, 0, Mathf.Abs(firstRoll) < Mathf.Abs(secondRoll) ? firstRoll : secondRoll);

            grenade.GetComponent<SaveGameObject>().SpawnName = GrenadeDesignation;
        }
    }

}
