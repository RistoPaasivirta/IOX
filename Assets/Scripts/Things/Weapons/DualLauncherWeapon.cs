using UnityEngine;

public class DualLauncherWeapon : Weapon
{
    [SerializeField] private GameObject WeaponPrefab1 = null;
    [SerializeField] private string attachmentPoint1 = "";
    [SerializeField] private GameObject WeaponPrefab2 = null;
    [SerializeField] private string attachmentPoint2 = "";
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
    [SerializeField] private int RocketDamage = 60;
    [SerializeField] private float RocketArmDistance = 3f;
    [SerializeField] private Vector2 RocketArmTimeRange = new Vector2(.2f, .6f);
    [SerializeField] private Vector2 RocketExplosionRadius = new Vector2(1f, 3f);
    [SerializeField] private float RocketForce = 200f;
    [SerializeField] private float RocketSpeed = 20f;
    [SerializeField] private float RocketSlowdown = 0f;
    [SerializeField] private float RocketLifeTime = 0f;
    [SerializeField] private DamageType RocketDamageType = DamageType.Physical;
    [SerializeField] private float attackCooldown = .6f;

    public override string GetShortStats => "Dual Launcher Weapon";

    GameObject _weaponPrefab1;
    GameObject _weaponPrefab2;
    Rocket RocketPrefab = null;
    float cooldownTimer;
    bool swap;

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

        if (_weaponPrefab1 != null)
            Destroy(_weaponPrefab1);

        if (_weaponPrefab2 != null)
            Destroy(_weaponPrefab2);

        _weaponPrefab1 = Instantiate(WeaponPrefab1);
        _weaponPrefab1.transform.SetParent(character.GetAttachmentPoint(attachmentPoint1), false);

        _weaponPrefab2 = Instantiate(WeaponPrefab2);
        _weaponPrefab2.transform.SetParent(character.GetAttachmentPoint(attachmentPoint2), false);

        foreach (WeaponPrefabEffect p in _weaponPrefab1.GetComponents<WeaponPrefabEffect>())
            p.Init(this, caller);

        foreach (WeaponPrefabEffect p in _weaponPrefab2.GetComponents<WeaponPrefabEffect>())
            p.Init(this, caller);

        character.CallAnimation(MonsterCharacter.Animations.TwoHandedRangedIdle);
        character.CurrentWeaponAnimation = MonsterCharacter.Animations.TwoHandedRangedIdle;
    }

    public override void UnEquip()
    {
        base.UnEquip();

        if (_weaponPrefab1 != null)
            Destroy(_weaponPrefab1);

        if (_weaponPrefab2 != null)
            Destroy(_weaponPrefab2);
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
            if (swap)
                return _weaponPrefab1.transform.position + _weaponPrefab1.transform.rotation * RocketPosition;
            else
                return _weaponPrefab2.transform.position + _weaponPrefab2.transform.rotation * RocketPosition;
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

            if (swap)
            {
                //muzzle.transform.rotation = _weaponPrefab1.transform.rotation * Quaternion.Euler(MuzzleRotation);
                muzzle.transform.position = _weaponPrefab1.transform.position + _weaponPrefab2.transform.TransformDirection(MuzzlePosition);
                muzzle.transform.SetParent(_weaponPrefab1.transform);
                muzzle.transform.localRotation = Quaternion.Euler(MuzzleRotation);
            }
            else
            {
                //muzzle.transform.rotation = _weaponPrefab2.transform.rotation * Quaternion.Euler(MuzzleRotation);
                muzzle.transform.position = _weaponPrefab2.transform.position + _weaponPrefab2.transform.TransformDirection(MuzzlePosition);
                muzzle.transform.SetParent(_weaponPrefab2.transform);
                muzzle.transform.localRotation = Quaternion.Euler(MuzzleRotation);
            }
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

            //make a special raycasting move from character to barrel
            rocket.transform.position = character.transform.position;

            if (swap)
                rocket.InitialMove((_weaponPrefab1.transform.position + _weaponPrefab1.transform.rotation * RocketPosition) - character.transform.position);
            else
                rocket.InitialMove((_weaponPrefab2.transform.position + _weaponPrefab2.transform.rotation * RocketPosition) - character.transform.position);

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

        swap = !swap;

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
