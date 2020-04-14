using UnityEngine;

public class SprayWeapon : Weapon
{
    [SerializeField] private GameObject WeaponPrefab = null;
    [SerializeField] private string attachmentPoint = "";
    [SerializeField] private Sounds.SoundDef EmptySound = new Sounds.SoundDef();
    [SerializeField] private string SprayDesignation = "";
    [SerializeField] private Vector3 SprayPosition = Vector3.zero;
    [SerializeField] private int UsesAmmo = -1;
    [SerializeField] private int AmmoUseAmount = 1;
    [SerializeField] private float InheritSpeed = 1f;
    [SerializeField] private float spread = 10f;
    [SerializeField] private int SprayDamage = 4000;
    [SerializeField] private float SprayForce = 10f;
    [SerializeField] private float SpraySpeed = 30f;
    [SerializeField] private float SprayLifeTime = 0.4f;
    [SerializeField] private DamageType SprayDamageType = DamageType.Physical;
    [SerializeField] private float attackCooldown = .05f;
    [SerializeField] private int SprayTotalDamage = 20000;
    [SerializeField] private int DamagePerApplication = 1000;
    [SerializeField] private Vector2 SprayRadius = new Vector2(.5f, 2f);
    [SerializeField] private GameObject[] UpgradesInto = new GameObject[0];

    public override GameObject[] Upgrades => UpgradesInto;

    Spray SprayPrefab = null;
    GameObject _weaponPrefab;
    float attackCooldownTimer = .2f;

    public override string GetShortStats
    {
        get
        {
            return
                ItemName + System.Environment.NewLine +
                System.Environment.NewLine +
                "PRIMARY ATTACK:" + System.Environment.NewLine +
                (SprayDamage / 1000) + " damage" + System.Environment.NewLine +
                (SprayTotalDamage / 1000) + " damage over " + (SprayTotalDamage / (DamagePerApplication * 5))+ "s" + System.Environment.NewLine +
                (1 / attackCooldown).ToString("F2") + " attacks per second" + System.Environment.NewLine +
                (100 / spread).ToString("F1") + " accuracy" + System.Environment.NewLine +
                SpraySpeed.ToString("F1") + " projectile speed" + System.Environment.NewLine +
                System.Environment.NewLine +
                "Uses fuel" + System.Environment.NewLine +
                (UpgradesInto.Length > 0 ? System.Environment.NewLine + "Can be augmented" : "");
        }
    }

    private void Awake()
    {
        if (!string.IsNullOrEmpty(SprayDesignation))
        {
            if (!ThingDesignator.Designations.ContainsKey(SprayDesignation))
            {
                Debug.LogError("SprayWeapon \"" + gameObject.name + "\" spray designation \"" + SprayDesignation + "\" not found in designator");
                return;
            }

            GameObject prefab = ThingDesignator.Designations[SprayDesignation];
            SprayPrefab = prefab.GetComponent<Spray>();

            if (SprayPrefab == null)
            {
                Debug.LogError("SprayWeapon \"" + gameObject.name + "\" spray designation \"" + SprayDesignation + "\" has no <Spray> component");
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
    public override bool AltFireTrigger() { return false; }
    public override bool AltFireContinuous() { return false; }

    public override Vector2 AttackRayStartPosition
    {
        get
        {
            return _weaponPrefab.transform.position + _weaponPrefab.transform.rotation * SprayPosition;
        }
    }

    public override bool IsReady
    {
        get
        {
            return true;
        }
    }

    private bool Fire()
    { 
        if (RaiseTimer > 0f) return false;
        if (HolsterTimer > 0f) return false;

        if (attackCooldownTimer <= 0f)
        {
            if (UsesAmmo >= 0)
            {
                if (character.ammunition[UsesAmmo] < AmmoUseAmount)
                {
                    Messaging.GUI.ScreenMessage.Invoke("OUT OF AMMO", Color.red);
                    Sounds.CreateSound(EmptySound);
                    attackCooldownTimer = .4f;
                    return false;
                }

                character.ammunition[UsesAmmo] -= AmmoUseAmount;
            }

            attackCooldownTimer = attackCooldown;

            if (SprayPrefab != null)
            {
                Spray spray = Instantiate(SprayPrefab, LevelLoader.DynamicObjects);
                spray.owner = character;
                spray.Damage = SprayDamage;
                spray.damageType = SprayDamageType;
                spray.LifeTime = SprayLifeTime;
                spray.Force = SprayForce;
                spray.Speed = SpraySpeed;
                spray.BurnTotalDamage = SprayTotalDamage;
                spray.DamagePerBurn = DamagePerApplication;
                spray.RadiusOverLifetime = SprayRadius;

                //make a special raycasting move from character to barrel
                spray.transform.position = character.transform.position;
                spray.InitialMove((_weaponPrefab.transform.position + _weaponPrefab.transform.rotation * SprayPosition) - character.transform.position);

                if (AttackDirection == Vector2.zero)
                    spray.transform.rotation = character.LookDirection;
                else
                    spray.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(AttackDirection.x, -AttackDirection.y) * Mathf.Rad2Deg);

                //add spread
                float firstRoll = Random.Range(-spread, spread);
                float secondRoll = Random.Range(-spread, spread);

                spray.transform.rotation *= Quaternion.Euler(0, 0, Mathf.Abs(firstRoll) < Mathf.Abs(secondRoll) ? firstRoll : secondRoll);

                //inherit speed
                spray.Speed += InheritSpeed * Vector2.Dot(character.physicsBody.velocity, character.LookDirection * Vector3.right);

                spray.GetComponent<SaveGameObject>().SpawnName = SprayDesignation;
            }
        }

        if (UsesAmmo >= 0 && character.ammunition[UsesAmmo] < AmmoUseAmount)
            return false;

        OnFire.Invoke();

        return true;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;
    }

}
