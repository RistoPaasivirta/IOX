using System.IO;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ThingController), typeof(Rigidbody2D))]
public class MonsterCharacter : MonoBehaviour, Damageable, SaveGameObject
{
    public float AnimationSpeedMultiplier = 1f;

    public Factions Faction;

    [HideInInspector]
    public Weapon CurrentWeapon;
    [HideInInspector]
    public Weapon CurrentOffhand;
    [HideInInspector]
    public ThingController thingController;
    [HideInInspector]
    public Rigidbody2D physicsBody;

    public float walkSpeed = 50f;
    public float Dampen = .85f;

    //used by others when targeting this
    public float TargetingRadius = 1f;

    public Skill[] Skills = new Skill[0];
    public float[] SkillCooldowns = new float[0];

    [SerializeField] private bool AutoEquip = true;
    public int Armor = 0;

    public System.Collections.Generic.Dictionary<int, GameObject> Buffs = new System.Collections.Generic.Dictionary<int, GameObject>();
    public System.Collections.Generic.Dictionary<int, GameObject> Debuffs = new System.Collections.Generic.Dictionary<int, GameObject>();
    public Sounds.SoundDef HitSound = new Sounds.SoundDef();
    public float DamageCheckRadius = 1f;
    public int SoftDamageDecayPerTick = 100;
    public int MaxHitPoints = 1000;
    public UnityEvent OnDeath = new UnityEvent();
    public float Mass = 100f;
    public Vector2 impulseVector;
    public float impulseGroundDampening = .8f;
    public float impulseAirDampening = .9f;
    public int MaxChargePoints = 100;
    public int SoftDamageDelay = 0;
    public Animations CurrentWeaponAnimation = Animations.UnarmedIdle;
    ChargeRegenFrame chargeRegenFrame = new ChargeRegenFrame();
    public ChargeRegenEvent OnChargeRegen = new ChargeRegenEvent();
    Vector2 move;
    public int ChargeRegenPerTick = 100;

    public float TurnSpeed = 4;
    public Quaternion WantDirection;
    public AttachmentPoint[] attachmentPoints = new AttachmentPoint[0];

    int nextWeapon = -1;
    int nextOffhand = -1;
    int currentWeaponIndex = -1;
    int currentOffhandIndex = -1;

    public Weapon[] weapons = new Weapon[0];
    public int[] ammunition = new int[6] { -1, -1, -1, -1, -1, -1 };
    public int[] maxAmmo = new int[6] { -1, -1, -1, -1, -1, -1 };

    public DamageEvent OnDotStackApply = new DamageEvent();
    DamageFrame dotStackFrame = new DamageFrame();
    public float UseObjectRadius = 3f;
    int tick = 0;
    public bool Invulnerable = false;

    #region InputFrame
    [System.Serializable]
    public class InputFrame
    {
        public bool LocalTransform;
        public float up;
        public float right;
        public bool PrimaryAttackTrigger;
        public bool SecondaryAttackTrigger;
        public bool PrimaryAttackContinuous;
        public bool SecondaryAttackContinuous;
        public bool UseObject;
        public Quaternion WantDirection;
        public int SwapWeapon;

        public void Clear()
        {
            up = 0f;
            right = 0f;
            PrimaryAttackTrigger = false;
            SecondaryAttackTrigger = false;
            PrimaryAttackContinuous = false;
            SecondaryAttackContinuous = false;
            WantDirection = Quaternion.identity;
            LocalTransform = false;
            UseObject = false;
            SwapWeapon = -1;
        }
    }

    private InputFrame inputFrame = new InputFrame();
    public delegate void InputRequestDelegate(ref InputFrame frame);
    public InputRequestDelegate OnInputRequest;

    bool allowMove = true;
    public System.Func<bool> OnAllowMove;

    [HideInInspector]
    public Quaternion LookDirection = Quaternion.identity;
    public System.Func<Quaternion> OnLookDirection;

    public delegate void CombineMoveDelegate(ref Vector2 move);
    public event CombineMoveDelegate OnCombineMove;

    public UnityEvent<int> OnWeaponChange = new UnityEventInteger();

    public delegate void CallAnimationDelegate(Animations animation);
    public event CallAnimationDelegate OnCallAnimation;
    public void CallAnimation(Animations animation)
    {
        OnCallAnimation?.Invoke(animation);
    }

    public UnityEvent<int> OnAmmoPickup = new UnityEventInteger();
    public UnityEvent OnHealthPickup = new UnityEvent();
    #endregion

    private void Awake()
    {
        thingController = GetComponent<ThingController>();
        physicsBody = GetComponent<Rigidbody2D>();

        HitPoints = MaxHitPoints;
        ChargePoints = MaxChargePoints;

        //for monsters without weapons
        OnLookDirection = RotateTowardsWantDirection;
    }

    private void Start()
    {
        for (int i = 0; i < Mathf.Min(ammunition.Length, maxAmmo.Length); i++)
            ammunition[i] = maxAmmo[i];

        SkillCooldowns = new float[Skills.Length];

        if (AutoEquip)
        {
            SwapWeapon(0, false);
            SwapWeapon(1, true);
        }
    }

    public class ChargeRegenFrame
    {
        public int RechargeAmount;
    }

    [System.Serializable]
    public class ChargeRegenEvent : UnityEvent<ChargeRegenFrame> { }

    void Update ()
    {
        if (TimeScaler.Paused)
            return;

        for (int i = 0; i < SkillCooldowns.Length; i++)
            if (SkillCooldowns[i] > 0)
                SkillCooldowns[i] -= Time.deltaTime;

        //input
        move = Vector2.zero;
        allowMove = true;
        inputFrame.Clear();
        inputFrame.WantDirection = LookDirection;

        OnInputRequest?.Invoke(ref inputFrame);

        //swap weapon
        if (inputFrame.SwapWeapon >= 0)
            SwapWeapon(inputFrame.SwapWeapon);

        //movement control
        if (OnAllowMove != null)
            allowMove = OnAllowMove();

        if (allowMove)
        {
            Vector2 up;
            Vector2 right;
            if (inputFrame.LocalTransform)
            {
                up = LookDirection * (Vector3.right * inputFrame.up);
                right = transform.forward * inputFrame.right;
            }
            else
            {
                up = Vector2.up * inputFrame.up;
                right = Vector2.right * inputFrame.right;
            }

            if (inputFrame.up != 0f || inputFrame.right != 0f)
                move = (up + right).normalized;
        }

        WantDirection = inputFrame.WantDirection;

        //look direction
        if (OnLookDirection != null)
            LookDirection = OnLookDirection();

        //combine
        OnCombineMove?.Invoke(ref move);

        //weapons
        if (CurrentWeapon != null && nextWeapon < 0)
        {
            if (inputFrame.PrimaryAttackTrigger)
                CurrentWeapon.FireTrigger();

            if (inputFrame.PrimaryAttackContinuous)
                CurrentWeapon.FireContinuous();

            if (inputFrame.SecondaryAttackTrigger)
                CurrentWeapon.AltFireTrigger();

            if (inputFrame.SecondaryAttackContinuous)
                CurrentWeapon.AltFireContinuous();
        }

        if (CurrentOffhand != null && nextOffhand < 0)
        {
            if (inputFrame.SecondaryAttackTrigger)
                CurrentOffhand.FireTrigger();

            if (inputFrame.SecondaryAttackContinuous)
                CurrentOffhand.FireContinuous();
        }

        if (nextWeapon >= 0)
            if (CurrentWeapon == null)
                ChangeWeapon();
            else if (CurrentWeapon.HolsterTimer <= 0f)
                ChangeWeapon();

        if (nextOffhand >= 0)
            if (CurrentOffhand == null)
                ChangeWeapon(true);
            else if (CurrentOffhand.HolsterTimer <= 0f)
                ChangeWeapon(true);

        if (inputFrame.UseObject)
            UseObject();
    }

    private void UseObject()
    {
        foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, UseObjectRadius))
        {
            UsableObject u = c.GetComponent<UsableObject>();
            if (u == null) continue;
            u.Use(this);
        }
    }

    private void FixedUpdate()
    {
        if (SoftDamageDelay > 0)
        {
            SoftDamageDelay--;
        }
        else
        {
            if (SoftDamage > 0)
                SoftDamage -= SoftDamageDecayPerTick;
        }

        //if (ChargePoints < MaxChargePoints)
        if (MaxChargePoints > 0)
        {
            chargeRegenFrame.RechargeAmount = ChargeRegenPerTick;
            OnChargeRegen.Invoke(chargeRegenFrame);
            ChargePoints += chargeRegenFrame.RechargeAmount;
        }

        physicsBody.velocity *= Dampen;
        physicsBody.velocity += move * walkSpeed * .02f + impulseVector;
        impulseVector *= impulseGroundDampening;

        tick++;

        if (tick == 10)
        {
            tick = 0;

            dotStackFrame.amount = 0;
            OnDotStackApply.Invoke(dotStackFrame);

            if (dotStackFrame.amount > 0)
                (this as Damageable).Damage(new DamageFrame(dotStackFrame.amount, AttackType.DoT, DamageType.Fire, null, transform.position));
        }
    }

    /*private void OnEnable()
    {
        impulseVector = Vector2.zero;
        physicsBody.velocity = Vector2.zero;
    }

    private void OnDisable()
    {
        impulseVector = Vector2.zero;
        physicsBody.velocity = Vector2.zero;
    }*/

    public void SwapWeapon(int weapon, bool offhand = false)
    {
        if (weapon < 0 || weapon >= weapons.Length)
            return;

        if (weapons[weapon] == null)
            return;

        if (offhand)
        {
            if (currentOffhandIndex == weapon)
                return;

            if (CurrentWeapon != null)
                if (CurrentWeapon.TwoHanded)
                    return;

            if (nextWeapon != -1)
                if (weapons[nextWeapon] != null)
                    if (weapons[nextWeapon].TwoHanded)
                        return;

            if (CurrentOffhand != null)
                CurrentOffhand.Holster();

            nextOffhand = weapon;
            currentOffhandIndex = weapon;
        }
        else
        {
            if (currentWeaponIndex == weapon)
                return;

            if (CurrentWeapon != null)
                CurrentWeapon.Holster();

            nextWeapon = weapon;
            currentWeaponIndex = weapon;
        }

        OnWeaponChange.Invoke(weapon);
    }

    public void UnEquip()
    {
        if (CurrentWeapon != null)
        {
            CurrentWeapon.UnEquip();
            Destroy(CurrentWeapon.gameObject);
        }

        if (CurrentOffhand != null)
        {
            CurrentOffhand.UnEquip();
            Destroy(CurrentOffhand.gameObject);
        }

        nextWeapon = -1;
        nextOffhand = -1;

        CallAnimation(Animations.Holster);

        OnLookDirection = RotateTowardsWantDirection;

        OnWeaponChange.Invoke(-1);
        currentWeaponIndex = -1;
        currentOffhandIndex = -1;
    }

    private void ChangeWeapon(bool offhand = false)
    {
        if (offhand)
        {
            if (nextOffhand < 0 || nextOffhand >= weapons.Length)
            {
                Debug.LogError("MonsterCharacter \"" + name + " \": ChangeWeapon: nextOffhand out of weapon array (" + nextOffhand + ")");
                nextOffhand = -1; //to avoid spam
                return;
            }

            if (CurrentWeapon != null)
                if (CurrentWeapon.TwoHanded)
                { 
                    nextOffhand = -1;
                    return;
                }

            if (CurrentOffhand != null)
            {
                CurrentOffhand.UnEquip();
                Destroy(CurrentOffhand.gameObject);
            }

            CurrentOffhand = Instantiate(weapons[nextOffhand], transform);
            CurrentOffhand.Equip(this);

            //if (OnWeaponChange != null)
                //OnWeaponChange(nextOffhand);

            nextOffhand = -1;
        }
        else
        {
            if (nextWeapon < 0 || nextWeapon >= weapons.Length)
            {
                Debug.LogError("MonsterCharacter \"" + name + " \": ChangeWeapon: nextWeapon out of weapon array (" + nextWeapon + ")");
                OnLookDirection = RotateTowardsWantDirection;
                nextWeapon = -1; //to avoid spam
                return;
            }

            if (CurrentWeapon != null)
            {
                CurrentWeapon.UnEquip();
                Destroy(CurrentWeapon.gameObject);
            }

            OnLookDirection = null;

            CurrentWeapon = Instantiate(weapons[nextWeapon], transform);
            CurrentWeapon.Equip(this);

            //weapon doesn't use special look method, apply default
            if (OnLookDirection == null)
                OnLookDirection = RotateTowardsWantDirection;

            //if (OnWeaponChange != null)
                //OnWeaponChange(nextWeapon);

            nextWeapon = -1;
        }
    }

    [System.Serializable]
    public struct AttachmentPoint
    {
        public Transform transform;
        public string name;
    }

    public Transform GetAttachmentPoint(string name)
    {
        foreach (AttachmentPoint p in attachmentPoints)
            if (p.name == name)
                return p.transform;

        Debug.LogError("MonsterCharacter \""+name+"\": Could not find AttachmentPoint \"" + name + "\"");

        return transform;
    }

    public Quaternion RotateTowardsWantDirection()
    {
        //return Quaternion.RotateTowards(LookDirection, WantDirection, TurnSpeed * Time.deltaTime);
        return Quaternion.Slerp(LookDirection, WantDirection, Time.deltaTime * TurnSpeed);
    }

    #region Damageable
    [System.Serializable]
    public class DamageEvent : UnityEvent<DamageFrame> { }
    public DamageEvent OnDamage = new DamageEvent();
    [System.Serializable]
    public class DotEvent : UnityEvent<DotFrame> { }
    public DotEvent OnDot = new DotEvent();

    public class SoftDamageFrame { public int amount; }
    SoftDamageFrame softDamageFrame = new SoftDamageFrame();
    [System.Serializable]
    public class SoftDamageEvent : UnityEvent<SoftDamageFrame> { }
    public SoftDamageEvent GenerateSoftDamage = new SoftDamageEvent();

    void Damageable.Damage(DamageFrame frame)
    {
        if (Invulnerable)
            return;

        if (Faction != Factions.AgainstAll)
            if (frame.attacker != null)
                if (frame.attacker.Faction != Factions.AgainstAll)
                    if (frame.attacker.Faction == Faction)
                        return;

        OnDamage.Invoke(frame);

        if (frame.attacker != null)
            if (frame.attacker.Faction == Factions.Player)
                frame.amount = (int)System.Math.Round((double)frame.amount * ((double)Difficulty.PlayerDamage / 100d));
            else
                frame.amount = (int)System.Math.Round((double)frame.amount * ((double)Difficulty.EnemyDamage / 100d));

        frame.amount *= 100;
        frame.amount /= 100 + Armor;

        Messaging.GUI.DamageIndicator.Invoke(frame.hitPosition, AxMath.OneThousanth(frame.amount).ToString(), frame.damageType);
        HitPoints -= frame.amount;

        if (frame.attacker != null)
            frame.attacker.OnDealDamage.Invoke(frame.amount);

        softDamageFrame.amount = frame.amount;
        GenerateSoftDamage.Invoke(softDamageFrame);
        SoftDamage += softDamageFrame.amount;

        if (HitPoints <= 0)
        {
            OnDeath.Invoke();

            if (CurrentWeapon != null)
                Destroy(CurrentWeapon.gameObject);

            Destroy(gameObject);
        }
    }

    public UnityEvent<int> OnDealDamage = new UnityEventInteger();

    void Damageable.Dot(DotFrame frame)
    {
        if (Invulnerable)
            return;

        if (Faction != Factions.AgainstAll)
            if (frame.attacker != null)
                if (frame.attacker.Faction != Factions.AgainstAll)
                    if (frame.attacker.Faction == Faction)
                        return;

        //these would stick forever
        if (frame.damagePerApplication <= 0)
            return;

        OnDot.Invoke(frame);

        if (frame.attacker != null)
            if (frame.attacker.Faction == Factions.Player)
                frame.totalDamage = (int)System.Math.Round((double)frame.totalDamage * ((double)Difficulty.PlayerDamage / 100d));
            else
                frame.totalDamage = (int)System.Math.Round((double)frame.totalDamage * ((double)Difficulty.EnemyDamage / 100d));

        GameObject ailment = new GameObject("dot");
        ailment.transform.SetParent(transform);
        DotStack d = ailment.AddComponent<DotStack>();
        d.damageType = frame.damageType;
        d.totalDamage = frame.totalDamage;
        d.damagePerApplication = frame.damagePerApplication;
    }

    void Damageable.Kill()
    {
        OnDeath.Invoke();

        if (CurrentWeapon != null)
            Destroy(CurrentWeapon.gameObject);

        Destroy(gameObject);
    }

    void Damageable.Impulse(Vector2 direction, float force)
    {
        if (Invulnerable)
            return;

        float length = force / Mass;
        impulseVector += direction * length;

        //cap impulse vector magnitude
        if (impulseVector.magnitude > Options.MaxImpulse)
            impulseVector = impulseVector.normalized * Options.MaxImpulse;
    }

    int _chargePoints;
    public int ChargePoints
    {
        get
        {
            return _chargePoints;
        }

        set
        {
            _chargePoints = value;
            if (_chargePoints > MaxChargePoints)
                _chargePoints = MaxChargePoints;

            if (_chargePoints < 0)
                _chargePoints = 0;
        }
    }

    int _hitPoints;
    public int HitPoints
    {
        get
        {
            return _hitPoints;
        }

        set
        {
            _hitPoints = value;
            if (_hitPoints > MaxHitPoints)
                _hitPoints = MaxHitPoints;
        }
    }

    int _softDamage;
    public int SoftDamage
    {
        get
        {
            return _softDamage;
        }

        set
        {
            _softDamage = value;

            if (_softDamage < 0)
                _softDamage = 0;
        }
    }

    bool Damageable.Dead
    {
        get
        {
            return HitPoints <= 0;
        }
    }

    bool Damageable.Bleed
    {
        get
        {
            return true;
        }
    }

    Factions Damageable.Faction
    {
        get
        {
            return Faction;
        }
    }

    float Damageable.Radius
    {
        get
        {
            return DamageCheckRadius;
        }
    }

    bool Damageable.HitSound(Vector3 position, AttackType attackType)
    {
        if (HitSound.clips.Length == 0)
            return false;
        
        Sounds.CreateSound(HitSound, position);
        return true;
    }
    #endregion

    public enum Animations : int
    {
        UnarmedIdle = 0,
        TwoHandedMeleeIdle = 1,
        TwoHandedMeleeSwing = 2,
        TwoHandedRangedIdle = 3,
        DualWieldIdleR = 4,
        DualWieldIdleL = 5,
        DualWieldSwingR = 6,
        DualWieldSwingL = 7,
        LeftArmGrenadeThrow = 8,
        Holster = 9,
    }

    byte[] SaveGameObject.Serialize()
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);

        bw.Write(transform.position.x);
        bw.Write(transform.position.y);
        bw.Write(LookDirection.eulerAngles.z);

        byte[] data = stream.ToArray();
        bw.Close();
        stream.Close();
        return data;
    }

    void SaveGameObject.Deserialize(byte[] data)
    {
        MemoryStream stream = new MemoryStream(data);
        BinaryReader br = new BinaryReader(stream);

        transform.position = new Vector3(br.ReadSingle(), br.ReadSingle(), 0);
        LookDirection = Quaternion.Euler(0, 0, br.ReadSingle());

        br.Close();
        stream.Close();

        SaveLoadSystem.indexedMonsters.Add(this);

        OnDeserialize.Invoke();
    }

    void SaveGameObject.AfterCreated() { }

    public UnityEvent OnDeserialize = new UnityEvent();

    private int _saveIndex = -1;
    int SaveGameObject.SaveIndex
    {
        get { return _saveIndex; }
        set { _saveIndex = value; }
    }

    private string _spawnName = "";
    string SaveGameObject.SpawnName
    {
        get { return _spawnName; }
        set { _spawnName = value; }
    }

    SaveObjectType SaveGameObject.ObjectType
    {
        get { return SaveObjectType.Monster; }
    }
}
