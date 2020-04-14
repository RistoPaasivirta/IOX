using UnityEngine;

public class BeamWeapon : Weapon
{
    [SerializeField] private GameObject WeaponPrefab = null;
    [SerializeField] private string attachmentPoint = "";
    [SerializeField] private Sounds.SoundDef EmptySound = new Sounds.SoundDef();
    [SerializeField] private int UsesAmmo = -1;
    [SerializeField] private int AmmoUseAmount = 5;
    [SerializeField] private Vector3 BeamStartPosition = Vector3.zero;
    [SerializeField] private float MaxDistance = 20f;
    [SerializeField] private int BeamDamage = 1000;
    [SerializeField] private DamageType BeamDamageType = DamageType.Physical;
    [SerializeField] private float attackCooldown = .05f;
    [SerializeField] private int BeamTotalDamage = 10000;
    [SerializeField] private int DamagePerApplication = 1000;
    [SerializeField] private int BeamForce = 100;
    public float BeamDistanceMod = -2f;
    [SerializeField] private GameObject[] UpgradesInto = new GameObject[0];

    public override GameObject[] Upgrades => UpgradesInto;

    GameObject _weaponPrefab;
    float attackCooldownTimer = 0f;
    float ammoUseTimer = 0f;
    public float AmmoUseTime = .5f;
    public float beamLength { get; private set; }
    private float emptyTimer;

    public override string GetShortStats
    {
        get
        {
            return
                ItemName + System.Environment.NewLine +
                System.Environment.NewLine +
                "PRIMARY ATTACK:" + System.Environment.NewLine +
                (BeamDamage / 1000) + " damage" + System.Environment.NewLine +
                (BeamTotalDamage / 1000) + " damage over " + (BeamTotalDamage / (DamagePerApplication * 5))+ "s" + System.Environment.NewLine +
                (1 / attackCooldown).ToString("F2") + " attacks per second" + System.Environment.NewLine +
                MaxDistance.ToString("F1") + " maximum range" + System.Environment.NewLine +
                System.Environment.NewLine +
                "Uses cells" + System.Environment.NewLine +
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
            return _weaponPrefab.transform.position + _weaponPrefab.transform.rotation * BeamStartPosition;
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
        if (attackCooldownTimer > 0f) return false;
        if (emptyTimer > 0f) return false;

        if (UsesAmmo >= 0)
        {
            if (character.ammunition[UsesAmmo] < AmmoUseAmount)
            {
                Messaging.GUI.ScreenMessage.Invoke("OUT OF AMMO", Color.red);
                Sounds.CreateSound(EmptySound);
                emptyTimer = .4f;
                return false;
            }
        }

        if (attackCooldownTimer <= 0f)
        {
            attackCooldownTimer += attackCooldown;
            DamageBeam();
        }

        OnFire.Invoke();

        return true;
    }

    private void DamageBeam()
    {
        //DAMAGE
        RaycastHit2D hit = Physics2D.Raycast(AttackRayStartPosition, _weaponPrefab.transform.right, MaxDistance, LayerMask.WallsWalkersAndShootables);
        if (hit)
        {
            beamLength = hit.distance;

            Damageable dmg = hit.collider.GetComponentInParent<Damageable>();

            if (dmg == null)
                return;

            if (character.Faction != Factions.AgainstAll)
                if (character.Faction == dmg.Faction)
                    return;

            dmg.Damage(new DamageFrame(BeamDamage, AttackType.Bullet, BeamDamageType, character, hit.point));

            if (BeamTotalDamage > 0)
                dmg.Dot(new DotFrame(BeamTotalDamage, DamagePerApplication, DamageType.Fire, character));

            dmg.Impulse(_weaponPrefab.transform.right, BeamForce);
        }
        else
            beamLength = MaxDistance;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (emptyTimer > 0)
        {
            emptyTimer -= Time.deltaTime;
            return;
        }

        if (attackCooldownTimer > 0f)
        {
            OnFire.Invoke();

            attackCooldownTimer -= Time.deltaTime;

            ammoUseTimer += Time.deltaTime;
            if (ammoUseTimer >= AmmoUseTime)
            {
                if (character.ammunition[UsesAmmo] >= AmmoUseAmount)
                    character.ammunition[UsesAmmo] -= AmmoUseAmount;

                ammoUseTimer -= AmmoUseTime;
            }
        }
    }
}
