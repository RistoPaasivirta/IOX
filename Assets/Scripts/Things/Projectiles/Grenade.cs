using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Grenade : MonoBehaviour, SaveGameObject
{
    [SerializeField] private float DeletionTime = .5f;
    [SerializeField] private Sounds.SoundDef ExplosionSound = new Sounds.SoundDef();

    public Vector2 ExplosionRadius = new Vector2(1f, 3f);
    public int Damage = 10;
    public float Force = 400f;
    public float Speed = 15f;
    public float LifeTime = 0f;
    public float ArmDistance = 3f;
    public Vector2 ArmTimeRange = new Vector2(1f, 3f);
    public DamageType damageType;
    public GameObject[] OnHitObjects;

    [HideInInspector]
    public MonsterCharacter owner;

    bool PendingForDeletion = false;
    float deletionTimer;
    float lifeTimer;
    float armTime;
    bool armed = false;
    float armTimer;

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), owner.GetComponent<Collider2D>());
        rb.velocity = transform.right * Speed;
    }

    void Update ()
    {
        if (!PendingForDeletion)
        {
            if (armed)
            {
                armTimer += Time.deltaTime;
                if (armTimer >= armTime)
                    Detonate();
            }

            if (!armed && owner != null && ArmDistance > 0)
            {
                Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, ArmDistance);

                foreach (Collider2D target in targets)
                {
                    Damageable d = target.GetComponent<Damageable>();
                    if (d == null) continue;

                    if (owner.Faction == Factions.AgainstAll || owner.Faction != d.Faction)
                    {
                        armTime = Random.Range(ArmTimeRange.x, ArmTimeRange.y);
                        armed = true;
                    }
                }
            }
        }

        if (LifeTime > 0f)
        {
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= LifeTime)
                Detonate();
        }

        if (PendingForDeletion)
        {
            deletionTimer += Time.deltaTime;
            if (deletionTimer > DeletionTime)
                Destroy(gameObject);
        }
    }

    private void Detonate()
    {
        if (PendingForDeletion)
            return;

        PendingForDeletion = true;

        Sounds.CreateSound(ExplosionSound, transform.position);

        if (OnHitObjects.Length > 0)
        {
            GameObject onHitObject = Instantiate(OnHitObjects[Random.Range(0, OnHitObjects.Length)], LevelLoader.TemporaryObjects);
            onHitObject.transform.position = transform.position;
            onHitObject.transform.rotation = transform.rotation;
        }

        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, ExplosionRadius.y);

        foreach(Collider2D target in targets)
        {
            Damageable d = target.GetComponent<Damageable>();
            if (d == null) continue;

            Vector2 dif = target.transform.position - transform.position;
            Vector2 dir = dif == Vector2.zero ? Vector2.zero : dif.normalized;
            Vector2 hitPoint = (Vector2)target.transform.position - dir;

            float lerp = Mathf.InverseLerp(ExplosionRadius.x, ExplosionRadius.y, dif.magnitude - d.Radius);
            int dmg = Mathf.CeilToInt(Mathf.Lerp(Damage, 1, lerp));

            d.Damage(new DamageFrame(dmg, AttackType.Explosion, damageType, owner, hitPoint));
            d.Impulse(dir, Mathf.Lerp(Force, 1, lerp));
        }

        //camera shake
        float distanceToCamera = ((Vector2)transform.position - (Vector2)Camera.main.transform.position).magnitude;
        Messaging.CameraControl.Shake.Invoke((1f - Mathf.InverseLerp(1, 30, distanceToCamera)) * .5f);
    }

    byte[] SaveGameObject.Serialize()
    {
        return new byte[0];
    }

    void SaveGameObject.Deserialize(byte[] data)
    {
    }

    void SaveGameObject.AfterCreated() { }

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
