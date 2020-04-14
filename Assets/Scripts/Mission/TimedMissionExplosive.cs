using UnityEngine;
using UnityEngine.Events;

public class TimedMissionExplosive : MonoBehaviour
{
    [SerializeField] private string MissionTrigger = "";
    [SerializeField] private int CountDown = 5;
    [SerializeField] private float RandomTimer = 0f;
    [SerializeField] private GameObject OnZeroSpawnObject = null;
    [SerializeField] private string OnZeroMissionTriggerInvoke = "";
    [SerializeField] private UnityEvent OnZeroEvent = new UnityEvent();
    [SerializeField] private Vector2 ExplosionRadius = new Vector2(1f, 3.5f);
    [SerializeField] private float ExplosionDamage = 60f;
    [SerializeField] private float ExplosionForce = 200f;
    [SerializeField] private DamageType damageType = DamageType.Physical;

    float timer = 0f;

    private void Awake()
    {
        Messaging.Mission.MissionTrigger.AddListener((s) => 
        {
            if (s != MissionTrigger)
                return;

            timer = Random.Range(0f, RandomTimer);
            enabled = true;
        });

        enabled = false;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = 1f;

            if (CountDown > 0)
            {
                Messaging.GUI.RisingText.Invoke(transform.position, CountDown.ToString(), Color.red, Color.black);
                CountDown--;
            }
            else
            {
                if (OnZeroSpawnObject != null)
                {
                    GameObject g = Instantiate(OnZeroSpawnObject, LevelLoader.TemporaryObjects);
                    g.transform.position = transform.position;

                    foreach (Collider2D target in Physics2D.OverlapCircleAll(transform.position, ExplosionRadius.y))
                    {
                        Damageable d = target.GetComponent<Damageable>();
                        if (d == null) continue;

                        Vector2 dif = target.transform.position - transform.position;
                        Vector2 dir = dif == Vector2.zero ? Vector2.zero : dif.normalized;
                        Vector2 hitPoint = (Vector2)target.transform.position - dir;

                        float lerp = Mathf.InverseLerp(ExplosionRadius.x, ExplosionRadius.y, dif.magnitude - d.Radius);
                        int dmg = Mathf.CeilToInt(Mathf.Lerp(ExplosionDamage, 1, lerp));

                        d.Damage(new DamageFrame(dmg, AttackType.Explosion, damageType, null, hitPoint));
                        d.Impulse(dir, Mathf.Lerp(ExplosionForce, 1, lerp));
                    }
                }

                Messaging.Mission.MissionTrigger.Invoke(OnZeroMissionTriggerInvoke);
                OnZeroEvent.Invoke();

                enabled = false;
            }
        }
    }
}
