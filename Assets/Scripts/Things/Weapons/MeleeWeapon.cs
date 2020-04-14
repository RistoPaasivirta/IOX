using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] private GameObject WeaponPrefab = null;
    [SerializeField] private string attachmentPoint = "";
    [SerializeField] private GameObject EffectPrefab = null;
    [SerializeField] private Vector3 EffectPosition = Vector3.zero;
    [SerializeField] private MonsterCharacter.Animations IdleAnimation = MonsterCharacter.Animations.TwoHandedMeleeIdle;
    [SerializeField] private MonsterCharacter.Animations AttackAnimation = MonsterCharacter.Animations.TwoHandedMeleeSwing;
    [SerializeField] private int RechargeChargeAmount = 200000;
    [SerializeField] private GameObject[] UpgradesInto = new GameObject[0];
    [SerializeField] private float reboundCooldown = .2f;
    [SerializeField] private float attackCooldown = .6f;
    [SerializeField] private int Damage = 40;
    [SerializeField] private float Force = 100;
    [SerializeField] private float radius = 4f;
    [SerializeField] private Vector3 attackCenter = Vector3.zero;
    [SerializeField] private Sounds.SoundDef AttackSound = new Sounds.SoundDef();

    public override GameObject[] Upgrades => UpgradesInto;

    GameObject _weaponPrefab;
    float cooldownTimer;
    float reboundTimer;

    public override string GetShortStats
    {
        get
        {
            return
                ItemName + System.Environment.NewLine +
                System.Environment.NewLine +
                "PRIMARY ATTACK:" + System.Environment.NewLine +
                (Damage / 1000) + " damage in a " + radius + " radius" + System.Environment.NewLine +
                (1 / attackCooldown).ToString("F2") + " attacks per second" + System.Environment.NewLine +
                System.Environment.NewLine +
                "Recharges " + (RechargeChargeAmount / 1000) + " charge on hit" + System.Environment.NewLine+
                (UpgradesInto.Length > 0 ? System.Environment.NewLine + "Can be augmented" : "");
        }
    }

    public override void Equip(MonsterCharacter caller)
    {
        base.Equip(caller);

        if (_weaponPrefab != null)
            Destroy(_weaponPrefab);

        _weaponPrefab = Instantiate(WeaponPrefab);
        _weaponPrefab.transform.SetParent(character.GetAttachmentPoint(attachmentPoint), false);

        character.CallAnimation(IdleAnimation);
        character.CurrentWeaponAnimation = IdleAnimation;
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

        cooldownTimer = attackCooldown;
        reboundTimer = reboundCooldown;

        character.CallAnimation(AttackAnimation);
        character.CurrentWeaponAnimation = AttackAnimation;

        Vector3 pos = _weaponPrefab.transform.position + character.LookDirection * attackCenter;

        Collider2D[] targets = Physics2D.OverlapCircleAll(pos, radius);
        bool hitEnemy = false;
        foreach (Collider2D target in targets)
        {
            Damageable d = target.GetComponent<Damageable>();
            if (d == null) continue;
            if (d == character as Damageable) continue;

            Vector2 dif = target.transform.position - pos;
            Vector2 dir = dif == Vector2.zero ? Vector2.zero : dif.normalized;
            Vector2 hitPoint = (Vector2)target.transform.position - dir;

            d.HitSound(hitPoint, AttackType.Melee);

            d.Impulse(dir, Force);
            d.Damage(new DamageFrame(Damage, AttackType.Melee, DamageType.Physical, character, hitPoint));

            hitEnemy = true;
        }

        if (hitEnemy)
            character.ChargePoints += RechargeChargeAmount;

        if (EffectPrefab)
        {
            GameObject effect = Instantiate(EffectPrefab, LevelLoader.TemporaryObjects);
            effect.transform.position = _weaponPrefab.transform.position + character.LookDirection * EffectPosition;
            effect.transform.rotation = character.LookDirection;
        }

        Sounds.CreateSound(AttackSound);

        return true;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (reboundTimer > 0f)
        {
            reboundTimer -= Time.deltaTime;

            if (reboundTimer <= 0f)
            {
                character.CallAnimation(IdleAnimation);
                character.CurrentWeaponAnimation = IdleAnimation;
            }
        }

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

}
