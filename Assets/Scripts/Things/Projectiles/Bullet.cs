using System.IO;
using UnityEngine;

public class Bullet : MonoBehaviour, SaveGameObject
{
    [SerializeField] private Sounds.SoundDef DefaultHitSound = null;
    [SerializeField] private float DeletionTime = .5f;

    public int Damage = 10;
    public float Force = 100f;
    public float Speed = 40f;
    public float LifeTime = 0f;
    public int BurnTotalDamage = 0;
    public int DamagePerBurn = 0;
    public DamageType damageType;
    public GameObject OnHitObject;
    public Vector2 ExtraVelocity;

    [HideInInspector]
    public MonsterCharacter owner;

    bool PendingForDeletion = false;
    float deletionTimer;
    float lifeTimer;
    float travelled = 0f;
    readonly RaycastHit2D[] hits = new RaycastHit2D[4];

    void Update ()
    {
        if (!PendingForDeletion)
            Move(((Vector2)transform.right * Time.deltaTime * Speed) + ExtraVelocity * Time.deltaTime);
        
        if (LifeTime > 0f)
        {
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= LifeTime)
                PendingForDeletion = true;
        }

        if (PendingForDeletion)
        {
            deletionTimer += Time.deltaTime;
            if (deletionTimer > DeletionTime)
                Destroy(gameObject);
        }
    }

    private void Move(Vector2 step)
    {
        if (step == Vector2.zero)
            return;

        Vector2 dir = step.normalized; 
        float distance = step.magnitude;

        int count = Physics2D.RaycastNonAlloc(transform.position, dir, hits, distance, LayerMask.WallsWalkersAndShootables);

        Damageable target = null;
        float closest = float.MaxValue;
        Vector2 hitpoint = Vector2.zero;

        for (int i = 0; i < count; i++)
        {
            RaycastHit2D hit = hits[i];

            Damageable dmg = hit.collider.GetComponentInParent<Damageable>();
            if (owner != null && dmg != null)
            {
                if (owner.gameObject == hit.collider.gameObject)
                    continue;

                if (owner.Faction != Factions.AgainstAll)
                    if (owner.Faction == dmg.Faction)
                        continue;
            }

            float d = hit.distance;
            if (d < closest)
            {
                hitpoint = hit.point;
                closest = d;
                target = dmg;
            }
        }

        if (closest == float.MaxValue)
            transform.position += (Vector3)step;
        else
        {
            transform.position = hitpoint;
            PendingForDeletion = true;

            if (OnHitObject != null)
            {
                GameObject onHitObject = Instantiate(OnHitObject, LevelLoader.TemporaryObjects);
                onHitObject.transform.position = transform.position;
                onHitObject.transform.rotation = transform.rotation;
            }

            if (target != null)
            {
                if (!target.HitSound(hitpoint, AttackType.Bullet))
                    Sounds.CreateSound(DefaultHitSound, hitpoint);

                target.Damage(new DamageFrame(Damage, AttackType.Bullet, damageType, owner, hitpoint));

                if (BurnTotalDamage > 0)
                    target.Dot(new DotFrame(BurnTotalDamage, DamagePerBurn, DamageType.Fire, owner));

                target.Impulse(dir, Force);
            }
            else
                Sounds.CreateSound(DefaultHitSound, hitpoint);
        }

        travelled += distance;
        if (travelled >= Options.MaxAttackDistance)
            PendingForDeletion = true;
    }

    public void InitialMove(Vector3 step)
    {
        if (step == Vector3.zero)
            return;

        Vector2 dir = step.normalized;
        float distance = step.magnitude;

        int count = Physics2D.RaycastNonAlloc(transform.position, dir, hits, distance, LayerMask.WalkersAndShootables);

        Damageable target = null;
        float closest = float.MaxValue;
        Vector2 hitpoint = Vector2.zero;

        for (int i = 0; i < count; i++)
        {
            RaycastHit2D hit = hits[i];

            Damageable dmg = hit.collider.GetComponentInParent<Damageable>();
            if (owner != null && dmg != null)
            {
                if (owner.gameObject == hit.collider.gameObject)
                    continue;

                if (owner.Faction != Factions.AgainstAll)
                    if (owner.Faction == dmg.Faction)
                        continue;
            }

            float d = hit.distance;
            if (d < closest)
            {
                hitpoint = hit.point;
                closest = d;
                target = dmg;
            }
        }

        transform.position += step;

        if (closest != float.MaxValue)
        { 
            PendingForDeletion = true;

            if (OnHitObject != null)
            {
                GameObject onHitObject = Instantiate(OnHitObject, LevelLoader.TemporaryObjects);
                onHitObject.transform.position = transform.position;
                onHitObject.transform.rotation = transform.rotation;
            }

            if (target != null)
            {
                if (!target.HitSound(hitpoint, AttackType.Bullet))
                    Sounds.CreateSound(DefaultHitSound, hitpoint);

                target.Damage(new DamageFrame(Damage, AttackType.Bullet, damageType, owner, hitpoint));
                target.Impulse(dir, Force);
            }
            else
                Sounds.CreateSound(DefaultHitSound, hitpoint);
        }
    }

    byte[] SaveGameObject.Serialize()
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);

        bw.Write(transform.position.x);
        bw.Write(transform.position.y);
        bw.Write(transform.rotation.x);
        bw.Write(transform.rotation.y);
        bw.Write(transform.rotation.z);
        bw.Write(transform.rotation.w);
        bw.Write(owner == null ? -1 : (owner as SaveGameObject).SaveIndex);
        byte[] data = stream.ToArray();
        bw.Close();
        stream.Close();

        return data;
    }

    private int _ownerIndex;
    void SaveGameObject.Deserialize(byte[] data)
    {
        MemoryStream stream = new MemoryStream(data);
        BinaryReader br = new BinaryReader(stream);

        transform.position = new Vector3(br.ReadSingle(), br.ReadSingle(), 0);
        transform.rotation = new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        _ownerIndex = br.ReadInt32();

        br.Close();
        stream.Close();
    }

    void SaveGameObject.AfterCreated()
    {
        if (_ownerIndex >= 0)
        {
            owner = SaveLoadSystem.indexedMonsters[_ownerIndex];
            if (owner == null)
                Debug.LogError("Bullet: AfterCreated: owner == null even thou owner index was set");
        }
    }

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
        get { return SaveObjectType.Projectile; }
    }
}
