using UnityEngine;

public class ClusterExplosion : MonoBehaviour
{
    [SerializeField] private Vector2 Interval = new Vector2(.4f, .6f);
    [SerializeField] private float Duration = 5f;
    [SerializeField] private float Radius = 2f;
    [SerializeField] private GameObject[] SpawnObjects = new GameObject[0];
    [SerializeField] private Vector2 ExplosionRadius = new Vector2(1f, 3.5f);
    [SerializeField] private float ExplosionDamage = 60f;
    [SerializeField] private float ExplosionForce = 200f;
    [SerializeField] private DamageType damageType = DamageType.Physical;

    float duration;
    float timer;

    public void Activate()
    {
        duration = Duration;
        enabled = true;
    }

    private void Awake() =>
        enabled = false;

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = Random.Range(Interval.x, Interval.y);

            int i = Random.Range(0, SpawnObjects.Length);
            if (i >= 0 && i < SpawnObjects.Length)
            {
                GameObject g = Instantiate(SpawnObjects[i], LevelLoader.TemporaryObjects);
                Vector3 position = transform.position + (Vector3)Random.insideUnitCircle * Radius;
                g.transform.position = position;

                foreach (Collider2D target in Physics2D.OverlapCircleAll(position, ExplosionRadius.y))
                {
                    Damageable d = target.GetComponent<Damageable>();
                    if (d == null) continue;

                    Vector2 dif = target.transform.position - position;
                    Vector2 dir = dif == Vector2.zero ? Vector2.zero : dif.normalized;
                    Vector2 hitPoint = (Vector2)target.transform.position - dir;

                    float lerp = Mathf.InverseLerp(ExplosionRadius.x, ExplosionRadius.y, dif.magnitude - d.Radius);
                    int dmg = Mathf.CeilToInt(Mathf.Lerp(ExplosionDamage, 1, lerp));

                    d.Damage(new DamageFrame(dmg, AttackType.Explosion, damageType, null, hitPoint));
                    d.Impulse(dir, Mathf.Lerp(ExplosionForce, 1, lerp));
                }
            }
        }

        duration -= Time.deltaTime;
        if (duration <= 0f)
            enabled = false;
    }
}
