using UnityEngine;

public class LauncherWeapon : Weapon
{
    [SerializeField] private GameObject WeaponPrefab = null;
    [SerializeField] private string attachmentPoint = "";
    [SerializeField] private string RocketDesignation = "";
    [SerializeField] private Vector3 RocketPosition = Vector3.zero;
    [SerializeField] private GameObject MuzzlePrefab = null;
    [SerializeField] private Vector3 MuzzlePosition = Vector3.zero;
    [SerializeField] private Vector3 MuzzleRotation = Vector3.zero;
    [SerializeField] private int UsesAmmo = -1;
    [SerializeField] private int AmmoUseAmount = 1;
    [SerializeField] private Sounds.SoundDef FireSound = new Sounds.SoundDef();
    [SerializeField] private Sounds.SoundDef EmptySound = new Sounds.SoundDef();
    [SerializeField] private float spread = 10f;
    [SerializeField] private float Drunkenness = 0f;
    [SerializeField] private int Salvo = 0;
    [SerializeField] private float SalvoTime = .05f;
    [SerializeField] private int RocketDamage = 60;
    [SerializeField] private float RocketArmDistance = 3f;
    [SerializeField] private Vector2 RocketArmTimeRange = new Vector2(.2f, .6f);
    [SerializeField] private Vector2 RocketExplosionRadius = new Vector2(1f, 3f);
    [SerializeField] private float RocketForce = 200f;
    [SerializeField] private float RocketSpeed = 20f;
    [SerializeField] private float RocketSlowdown = 0f;
    [SerializeField] private float RocketLifeTime = 0f;
    [SerializeField] private DamageType RocketDamageType = DamageType.Physical;
    [SerializeField] private GameObject[] UpgradesInto = new GameObject[0];
    [SerializeField] private float attackCooldown = .6f;

    public override GameObject[] Upgrades => UpgradesInto;

    GameObject _weaponPrefab;
    Rocket RocketPrefab = null;
    int salvoRemain;
    float salvoTimer;
    float cooldownTimer;

    public override string GetShortStats
    {
        get
        {
            return
                ItemName + System.Environment.NewLine +
                System.Environment.NewLine +
                "PRIMARY ATTACK:" + System.Environment.NewLine +
                (RocketDamage / 1000) + " damage" + System.Environment.NewLine +
                (1 / attackCooldown).ToString("F2") + " attacks per second" + System.Environment.NewLine +
                (100 / spread).ToString("F1") + " accuracy" + System.Environment.NewLine +
                RocketExplosionRadius.y.ToString("F1") + " blast radius" + System.Environment.NewLine +
                RocketSpeed.ToString("F1") + " projectile speed" + System.Environment.NewLine +
                System.Environment.NewLine +
                "Uses rockets" + System.Environment.NewLine +
                (UpgradesInto.Length > 0 ? System.Environment.NewLine + "Can be augmented" : "");
        }
    }

    private void Awake()
    {
        if (!string.IsNullOrEmpty(RocketDesignation))
        {
            if (!ThingDesignator.Designations.ContainsKey(RocketDesignation))
            {
                Debug.LogError("LauncherWeapon \"" + gameObject.name + "\" rocket designation \"" + RocketDesignation + "\" not found in designator");
                return;
            }

            GameObject prefab = ThingDesignator.Designations[RocketDesignation];
            RocketPrefab = prefab.GetComponent<Rocket>();
            if (RocketPrefab == null)
            {
                Debug.LogError("RocketWeapon \"" + gameObject.name + "\" rocket designation \"" + RocketDesignation + "\" has no <Rocket> component");
                return;
            }
        }
    }

    public override void Equip(MonsterCharacter caller)
    {
        base.Equip(caller);

        if (_weaponPrefab != null)
            Destroy(_weaponPrefab);

        _weaponPrefab = Instantiate(WeaponPrefab);
        _weaponPrefab.transform.SetParent(character.GetAttachmentPoint(attachmentPoint), false);

        WeaponPrefabEffect[] prefabEffects = _weaponPrefab.GetComponents<WeaponPrefabEffect>();
        foreach(WeaponPrefabEffect p in prefabEffects)
            p.Init(this, caller);

        character.CallAnimation(MonsterCharacter.Animations.TwoHandedRangedIdle);
        character.CurrentWeaponAnimation = MonsterCharacter.Animations.TwoHandedRangedIdle;
    }

    public override void UnEquip()
    {
        base.UnEquip();

        if (_weaponPrefab != null)
            Destroy(_weaponPrefab);
    }

    public override bool FireTrigger()
    {
        return Fire();
    }

    public override bool FireContinuous()
    {
        return Fire();
    }

    public override Vector2 AttackRayStartPosition
    {
        get
        {
            return character.transform.position + character.LookDirection * RocketPosition;
        }
    }

    private bool Fire()
    {
        if (cooldownTimer > 0f) return false;
        if (RaiseTimer > 0f) return false;
        if (HolsterTimer > 0f) return false;
        if (UsesAmmo >= 0)
        {
            if (character.ammunition[UsesAmmo] < AmmoUseAmount)
            {
                Messaging.GUI.ScreenMessage.Invoke("OUT OF AMMO", Color.red);
                Sounds.CreateSound(EmptySound);
                cooldownTimer = .4f;
                return false;
            }

            character.ammunition[UsesAmmo] -= AmmoUseAmount;
        }

        cooldownTimer = attackCooldown;

        CreateRocket();

        salvoRemain = Salvo;
        salvoTimer = SalvoTime;

        return true;
    }

    private void CreateRocket()
    {
        Sounds.CreateSound(FireSound, transform.position);

        if (MuzzlePrefab != null)
        {
            GameObject muzzle = Instantiate(MuzzlePrefab);
            muzzle.transform.rotation = _weaponPrefab.transform.rotation * Quaternion.Euler(MuzzleRotation);
            muzzle.transform.position = _weaponPrefab.transform.position + muzzle.transform.TransformDirection(MuzzlePosition);
            muzzle.transform.SetParent(_weaponPrefab.transform);
        }

        if (RocketPrefab != null)
        {
            Rocket rocket = Instantiate(RocketPrefab, LevelLoader.DynamicObjects);
            rocket.owner = character;
            rocket.Damage = RocketDamage;
            rocket.ArmDistance = RocketArmDistance;
            rocket.ArmTimeRange = RocketArmTimeRange;
            rocket.Force = RocketForce;
            rocket.Speed = RocketSpeed;
            rocket.Slowdown = RocketSlowdown;
            rocket.ExplosionRadius = RocketExplosionRadius;
            rocket.LifeTime = RocketLifeTime;
            rocket.damageType = RocketDamageType;
            rocket.Veer = Random.Range(-Drunkenness, +Drunkenness);

            //make a special raycasting move from character to barrel
            rocket.transform.position = character.transform.position;
            rocket.InitialMove((_weaponPrefab.transform.position + _weaponPrefab.transform.rotation * RocketPosition) - character.transform.position);

            if (AttackDirection == Vector2.zero)
                rocket.transform.rotation = character.LookDirection;
            else
                rocket.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(AttackDirection.x, -AttackDirection.y) * Mathf.Rad2Deg);

            //add spread
            float firstRoll = Random.Range(-spread, spread);
            float secondRoll = Random.Range(-spread, spread);
            rocket.transform.rotation *= Quaternion.Euler(0, 0, Mathf.Abs(firstRoll) < Mathf.Abs(secondRoll) ? firstRoll : secondRoll);

            rocket.GetComponent<SaveGameObject>().SpawnName = RocketDesignation;
        }

        OnFire.Invoke();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (salvoRemain > 0)
            if (salvoTimer > 0f)
                salvoTimer -= Time.deltaTime;
            else
            {
                CreateRocket();
                salvoRemain--;
                salvoTimer += SalvoTime;
            }
    }

}
