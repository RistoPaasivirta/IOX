using UnityEngine;

public class BulletWeapon : Weapon
{
    [SerializeField] private GameObject WeaponPrefab = null;
    [SerializeField] private string attachmentPoint = "";
    [SerializeField] private string BulletDesignation = "";
    [SerializeField] private Vector3 BulletPosition = Vector3.zero;
    [SerializeField] private GameObject MuzzlePrefab = null;
    [SerializeField] private Vector3 MuzzlePosition = Vector3.zero;
    [SerializeField] private Vector3 MuzzleRotation = Vector3.zero;
    [SerializeField] private int UsesAmmo = -1;
    [SerializeField] private float spread = 10f;
    [SerializeField] private float attackCooldown = .6f;
    [SerializeField] private int Burst = 1;
    [SerializeField] private int AmmoUseAmount = 1;
    [SerializeField] private float altFireSpread = 6f;
    [SerializeField] private float altFireCooldown = .4f;
    [SerializeField] private int altFireBurst = 1;
    [SerializeField] private int altFireAmmoUseAmount = 1;
    [SerializeField] private int BurnTotalDamage = 0;
    [SerializeField] private int BurnDamagePerApplication = 0;
    [SerializeField] private Sounds.SoundDef FireSound = new Sounds.SoundDef();
    [SerializeField] private Sounds.SoundDef EmptySound = new Sounds.SoundDef();
    [SerializeField] private Sounds.SoundDef AltFireSound = new Sounds.SoundDef();
    [SerializeField] private Sounds.SoundDef AltFireEmptySound = new Sounds.SoundDef();
    [SerializeField] private int BulletDamage = 10;
    [SerializeField] private int AltFireBulletDamage = 12;
    [SerializeField] private float BulletForce = 40f;
    [SerializeField] private float BulletSpeed = 100f;
    [SerializeField] private float BulletLifeTime = 0f;
    [SerializeField] private DamageType BulletDamageType = DamageType.Physical;
    [SerializeField] private GameObject[] UpgradesInto = new GameObject[0];

    public override GameObject[] Upgrades => UpgradesInto;

    float cooldownTimer;
    GameObject _weaponPrefab;
    Bullet BulletPrefab = null;

    public override string GetShortStats
    {
        get
        {
            string s =
                ItemName + System.Environment.NewLine +
                System.Environment.NewLine +
                "PRIMARY ATTACK:" + System.Environment.NewLine +
                (BulletDamage / 1000) + " damage" + System.Environment.NewLine +
                (BurnTotalDamage > 0 ? (BurnTotalDamage / 1000) + " damage over " + (BurnTotalDamage / (BurnDamagePerApplication * 5)) + "s" + System.Environment.NewLine : "") +
                (1 / attackCooldown).ToString("F2") + " attacks per second" + System.Environment.NewLine +
                (100 / spread).ToString("F1") + " accuracy" + System.Environment.NewLine +
                (Burst > 1 ? Burst + " projectiles" + System.Environment.NewLine : "") +
                System.Environment.NewLine;

            if (AltFireBulletDamage > 0)
                s += "SECONDARY ATTACK:" + System.Environment.NewLine +
                (AltFireBulletDamage / 1000) + " damage" + System.Environment.NewLine +
                (1 / altFireCooldown).ToString("F2") + " attacks per second" + System.Environment.NewLine +
                (100 / altFireSpread).ToString("F1") + " accuracy" + System.Environment.NewLine +
                (altFireBurst > 1 ? altFireBurst + " projectiles" + System.Environment.NewLine : "") +
                (altFireAmmoUseAmount > 1 ? "Uses " + altFireAmmoUseAmount + " ammunition" + System.Environment.NewLine : "") +
                System.Environment.NewLine;

            switch (UsesAmmo)
            {
                case 0:
                    s += "Uses short rounds";
                    break;
                case 1:
                    s += "Uses long rounds";
                    break;
                case 2:
                    s += "Uses shells";
                    break;
                case 3:
                    s += "Uses rockets";
                    break;
                case 4:
                    s += "Uses fuel";
                    break;
                case 5:
                    s += "Uses cells";
                    break;
            }

            s += System.Environment.NewLine;

            if (UpgradesInto.Length > 0)
                s += System.Environment.NewLine + "Can be augmented";

            return s;
        }
    }

    private void Awake()
    {
        if (!string.IsNullOrEmpty(BulletDesignation))
        {
            if (!ThingDesignator.Designations.ContainsKey(BulletDesignation))
            {
                Debug.LogError("BulletWeapon \"" + gameObject.name + "\" bullet designation \"" + BulletDesignation + "\" not found in designator");
                return;
            }

            GameObject prefab = ThingDesignator.Designations[BulletDesignation];
            BulletPrefab = prefab.GetComponent<Bullet>();

            if (BulletPrefab == null)
            {
                Debug.LogError("BulletWeapon \"" + gameObject.name + "\" bullet designation \"" + BulletDesignation + "\" has no <Bullet> component");
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
        foreach (WeaponPrefabEffect p in prefabEffects)
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

    public override bool FireTrigger() { return Fire(); }
    public override bool FireContinuous() { return Fire(); }
    public override bool AltFireTrigger() { return AltFire(); }
    public override bool AltFireContinuous() { return AltFire(); }

    public override Vector2 AttackRayStartPosition
    {
        get
        {
            return _weaponPrefab.transform.position + _weaponPrefab.transform.rotation * BulletPosition;
        }
    }

    public override bool IsReady
    {
        get
        {
            return cooldownTimer <= 0f;
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

        Sounds.CreateSound(FireSound, transform.position);

        if (MuzzlePrefab != null)
        {
            GameObject muzzle = Instantiate(MuzzlePrefab);
            muzzle.transform.rotation = _weaponPrefab.transform.rotation * Quaternion.Euler(MuzzleRotation);
            muzzle.transform.position = _weaponPrefab.transform.position + muzzle.transform.TransformDirection(MuzzlePosition);
            muzzle.transform.SetParent(_weaponPrefab.transform);
        }

        int burst = Burst;
        while(burst-- > 0)
            if (BulletPrefab != null)
            {
                Bullet bullet = Instantiate(BulletPrefab, LevelLoader.DynamicObjects);
                bullet.owner = character;
                bullet.Damage = BulletDamage;
                bullet.damageType = BulletDamageType;
                bullet.LifeTime = BulletLifeTime;
                bullet.Force = BulletForce;
                bullet.Speed = BulletSpeed;
                bullet.BurnTotalDamage = BurnTotalDamage;
                bullet.DamagePerBurn = BurnDamagePerApplication;

                //make a special raycasting move from character to barrel
                bullet.transform.position = character.transform.position;
                bullet.InitialMove((_weaponPrefab.transform.position + _weaponPrefab.transform.rotation * BulletPosition) - character.transform.position);

                if (AttackDirection == Vector2.zero)
                    bullet.transform.rotation = character.LookDirection;
                else
                    bullet.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(AttackDirection.x, -AttackDirection.y) * Mathf.Rad2Deg);

                //add spread
                float firstRoll = Random.Range(-spread, spread);
                float secondRoll = Random.Range(-spread, spread);

                bullet.transform.rotation *= Quaternion.Euler(0, 0, Mathf.Abs(firstRoll) < Mathf.Abs(secondRoll) ? firstRoll : secondRoll);

                bullet.GetComponent<SaveGameObject>().SpawnName = BulletDesignation;
            }

        OnFire.Invoke();

        return true;
    }

    private bool AltFire()
    {
        if (AltFireBulletDamage <= 0) return false;
        if (cooldownTimer > 0f) return false;
        if (RaiseTimer > 0f) return false;
        if (HolsterTimer > 0f) return false;
        if (UsesAmmo >= 0)
        {
            if (character.ammunition[UsesAmmo] < altFireAmmoUseAmount)
            {
                Messaging.GUI.ScreenMessage.Invoke("OUT OF AMMO", Color.red);
                Sounds.CreateSound(AltFireEmptySound);
                cooldownTimer = .4f;
                return false;
            }

            character.ammunition[UsesAmmo] -= altFireAmmoUseAmount;
        }

        cooldownTimer = altFireCooldown;

        Sounds.CreateSound(AltFireSound);

        if (MuzzlePrefab != null)
        {
            GameObject muzzle = Instantiate(MuzzlePrefab);
            muzzle.transform.rotation = _weaponPrefab.transform.rotation * Quaternion.Euler(MuzzleRotation);
            muzzle.transform.position = _weaponPrefab.transform.position + muzzle.transform.TransformDirection(MuzzlePosition);
            muzzle.transform.SetParent(_weaponPrefab.transform);
        }

        int burst = altFireBurst;

        while (burst-- > 0)
            if (BulletPrefab != null)
            {
                Bullet bullet = Instantiate(BulletPrefab, LevelLoader.DynamicObjects);
                bullet.owner = character;
                bullet.Damage = AltFireBulletDamage;
                bullet.damageType = BulletDamageType;
                bullet.LifeTime = BulletLifeTime;
                bullet.Force = BulletForce;
                bullet.Speed = BulletSpeed;

                //make a special raycasting move from character to barrel
                bullet.transform.position = character.transform.position;
                bullet.InitialMove((_weaponPrefab.transform.position + _weaponPrefab.transform.rotation * BulletPosition) - character.transform.position);

                if (AttackDirection == Vector2.zero)
                    bullet.transform.rotation = character.LookDirection;
                else
                    bullet.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(AttackDirection.x, -AttackDirection.y) * Mathf.Rad2Deg);

                //add spread
                float firstRoll = Random.Range(-altFireSpread, altFireSpread);
                float secondRoll = Random.Range(-altFireSpread, altFireSpread);

                bullet.transform.rotation *= Quaternion.Euler(0, 0, Mathf.Abs(firstRoll) < Mathf.Abs(secondRoll) ? firstRoll : secondRoll);

                bullet.GetComponent<SaveGameObject>().SpawnName = BulletDesignation;
            }

        OnFire.Invoke();

        return true;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

}
