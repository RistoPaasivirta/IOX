using System.Collections.Generic;
using UnityEngine;

public class Spray : MonoBehaviour, SaveGameObject
{
    [SerializeField] private GameObject OnHitObject;
    [SerializeField] private Sounds.SoundDef HitSound = null;

    public int Damage = 10000;
    public float Force = 10f;
    public float Speed = 10f;
    public float LifeTime = .2f;
    public float DeletionTime = .5f;
    public float ParticleEmitTime = .1f;
    public bool PendingForDeletion = false;
    public float WallDeathMove = .2f;
    public DamageType damageType;
    public int BurnTotalDamage = 20000;
    public int DamagePerBurn = 1000;
    public Vector2 RadiusOverLifetime = new Vector2(.5f, 2f);

    [HideInInspector]
    public MonsterCharacter owner;

    ParticleSystem[] particleSystems = new ParticleSystem[0];
    float deletionTimer;
    float lifeTimer;
    float currentRadius;
    bool emitParticles = true;
    bool hitWall = false;
    readonly RaycastHit2D[] hits = new RaycastHit2D[6];
    List<Damageable> alreadyHit = new List<Damageable>();

    private void Awake() =>
        particleSystems = GetComponentsInChildren<ParticleSystem>();

    private void Start() =>
        currentRadius = RadiusOverLifetime.x;

    void Update ()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= LifeTime)
        {
            PendingForDeletion = true;
            lifeTimer = LifeTime;
        }

        if (emitParticles)
            if (lifeTimer >= ParticleEmitTime)
            {
                foreach (ParticleSystem ps in particleSystems)
                    ps.Stop();

                emitParticles = false;
            }

        if (PendingForDeletion)
        {
            deletionTimer += Time.deltaTime;
            if (deletionTimer > DeletionTime)
                Destroy(gameObject);

            //we move as dead to give some better particle velocity inheritance
            //this is only for visual purposes
            if (hitWall)
                Move(transform.right * Time.deltaTime * Speed * WallDeathMove);
            else
                Move(transform.right * Time.deltaTime * Speed);

            return;
        }

        Move(transform.right * Time.deltaTime * Speed);

        currentRadius = Mathf.Lerp(RadiusOverLifetime.x, RadiusOverLifetime.y, lifeTimer / LifeTime);
    }

    private void Move(Vector2 step)
    {
        if (step == Vector2.zero)
            return;

        Vector2 dir = step.normalized; 
        float distance = step.magnitude;

        if (!hitWall)
            if (Physics2D.Raycast(transform.position, dir, distance, LayerMask.Walls).collider != null)
            {
                hitWall = true;
                PendingForDeletion = true;
            }

        if (hitWall || lifeTimer >= LifeTime)
        {
            transform.position += (Vector3)step;
            return;
        }

        int count = Physics2D.CircleCastNonAlloc(transform.position, currentRadius, dir, hits, distance, LayerMask.WalkersAndShootables);
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

            if (dmg != null)
            {
                if (!alreadyHit.Contains(dmg))
                {
                    alreadyHit.Add(dmg);

                    dmg.Damage(new DamageFrame(Damage, AttackType.Bullet, damageType, owner, hit.point));
                    dmg.Dot(new DotFrame(BurnTotalDamage, DamagePerBurn, damageType, owner));
                    dmg.Impulse(dir, Force);

                    if (HitSound != null)
                        Sounds.Create3DSound(hit.point, HitSound.clips[0], Sounds.MixerGroup.Effects, HitSound.volume, HitSound.minPitch, HitSound.maxPitch, HitSound.priority, HitSound.minDistance);
                }
            }
        }

        transform.position += (Vector3)step;
    }

    public void InitialMove(Vector3 step)
    {
        if (step == Vector3.zero)
            return;

        Vector2 dir = step.normalized;
        float distance = step.magnitude;

        if (Physics2D.Raycast(transform.position, dir, distance, LayerMask.Walls).collider != null)
        {
            hitWall = true;
            transform.position += (Vector3)step;
            PendingForDeletion = true;
        }

        int count = Physics2D.CircleCastNonAlloc(transform.position, currentRadius, dir, hits, distance, LayerMask.WalkersAndShootables);
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

            if (dmg != null)
            {
                if (!alreadyHit.Contains(dmg))
                {
                    alreadyHit.Add(dmg);

                    dmg.Damage(new DamageFrame(Damage, AttackType.Bullet, damageType, owner, hit.point));
                    dmg.Dot(new DotFrame(BurnTotalDamage, DamagePerBurn, damageType, owner));
                    dmg.Impulse(dir, Force);

                    if (HitSound != null)
                        Sounds.Create3DSound(hit.point, HitSound.clips[0], Sounds.MixerGroup.Effects, HitSound.volume, HitSound.minPitch, HitSound.maxPitch, HitSound.priority, HitSound.minDistance);
                }
            }
        }

        if (!hitWall)
            transform.position += step;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, currentRadius);
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
