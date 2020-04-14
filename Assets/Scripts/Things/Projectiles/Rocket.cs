using UnityEngine;

public class Rocket : MonoBehaviour, SaveGameObject
{
    [SerializeField] private Sounds.SoundDef ExplosionSound = new Sounds.SoundDef();

    public Vector2 ExplosionRadius = new Vector2(1f, 3f);
    public float Veer = 0f;
    public int Damage = 10;
    public float Force = 400f;
    public float Speed = 15f;
    public float Slowdown = 0f;
    public float LifeTime = 0f;
    public float DeletionTime = .5f;
    public bool PendingForDeletion = false;
    public DamageType damageType;
    public GameObject[] OnHitObjects;
    public float ArmDistance = 3f;
    public Vector2 ArmTimeRange = new Vector2(1f, 3f);

    [HideInInspector]
    public MonsterCharacter owner;

    float armTime;
    bool armed = false;
    float armTimer;
    float deletionTimer;
    float lifeTimer;
    float travelled = 0f;

    void Update ()
    {
        Speed -= Slowdown * Time.deltaTime;

        if (!PendingForDeletion)
        {
            if (armed)
            {
                armTimer += Time.deltaTime;
                if (armTimer >= armTime)
                    Detonate();
            }

            if (Veer != 0f)
                transform.Rotate(0, 0, Veer * Time.deltaTime);

            Move(transform.right * Time.deltaTime * Speed);

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

    readonly RaycastHit2D[] hits = new RaycastHit2D[4];

    private void Move(Vector2 step)
    {
        if (step == Vector2.zero)
            return;

        Vector2 dir = step.normalized; 
        float distance = step.magnitude;

        int count = Physics2D.RaycastNonAlloc(transform.position, dir, hits, distance, LayerMask.WallsWalkersAndShootables);

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
            }
        }

        if (closest == float.MaxValue)
            transform.position += (Vector3)step;
        else
        {
            transform.position = hitpoint;
            Detonate();
        }

        if (!PendingForDeletion)
        {
            travelled += distance;
            if (travelled >= Options.MaxAttackDistance)
                Detonate();
        }
    }

    public void InitialMove(Vector3 step)
    {
        if (step == Vector3.zero)
            return;

        Vector2 dir = step.normalized;
        float distance = step.magnitude;

        int count = Physics2D.RaycastNonAlloc(transform.position, dir, hits, distance, LayerMask.WalkersAndShootables);

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
            }
        }

        transform.position += step;

        if (closest != float.MaxValue)
            Detonate();
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
