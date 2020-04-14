using UnityEngine;

public class DualBulletWeapon : Weapon
{
    [SerializeField] private GameObject WeaponPrefab1 = null;
    [SerializeField] private string attachmentPoint1 = "";
    [SerializeField] private GameObject WeaponPrefab2 = null;
    [SerializeField] private string attachmentPoint2 = "";
    [SerializeField] private string BulletDesignation = "";
    [SerializeField] private Vector3 BulletPosition = Vector3.zero;
    [SerializeField] private GameObject MuzzlePrefab = null;
    [SerializeField] private Vector3 MuzzlePosition = Vector3.zero;
    [SerializeField] private Vector3 MuzzleRotation = Vector3.zero;
    [SerializeField] private int UsesAmmo = -1;
    [SerializeField] private float spread = 10f;
    [SerializeField] private float attackCooldown = .6f;
    [SerializeField] private int AmmoUseAmount = 1;
    [SerializeField] private Sounds.SoundDef FireSound = new Sounds.SoundDef();
    [SerializeField] private Sounds.SoundDef EmptySound = new Sounds.SoundDef();
    [SerializeField] private int BulletDamage = 10;
    [SerializeField] private float BulletForce = 40f;
    [SerializeField] private float BulletSpeed = 100f;
    [SerializeField] private float BulletLifeTime = 0f;
    [SerializeField] private DamageType BulletDamageType = DamageType.Physical;

    public override string GetShortStats => "Dual Bullet Weapon";

    Bullet BulletPrefab = null;
    GameObject _weaponPrefab2;
    GameObject _weaponPrefab1;
    float cooldownTimer;
    bool swap;

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

    public override bool FireTrigger() { return Fire(); }
    public override bool FireContinuous() { return Fire(); }

    public override Vector2 AttackRayStartPosition
    {
        get
        {
            if (swap)
                return _weaponPrefab1.transform.position + _weaponPrefab1.transform.rotation * BulletPosition;
            else
                return _weaponPrefab2.transform.position + _weaponPrefab2.transform.rotation * BulletPosition;
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

        if (BulletPrefab != null)
        {
            Bullet bullet = Instantiate(BulletPrefab, LevelLoader.DynamicObjects);
            bullet.owner = character;
            bullet.Damage = BulletDamage;
            bullet.damageType = BulletDamageType;
            bullet.LifeTime = BulletLifeTime;
            bullet.Force = BulletForce;
            bullet.Speed = BulletSpeed;

            //make a special raycasting move from character to barrel
            bullet.transform.position = character.transform.position;

            if (swap)
                bullet.InitialMove((_weaponPrefab1.transform.position + _weaponPrefab1.transform.rotation * BulletPosition) - character.transform.position);
            else
                bullet.InitialMove((_weaponPrefab2.transform.position + _weaponPrefab2.transform.rotation * BulletPosition) - character.transform.position);

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
