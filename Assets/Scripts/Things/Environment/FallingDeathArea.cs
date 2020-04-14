using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FallingDeathArea : MonoBehaviour
{
    [SerializeField] private GameObject FallKillEffect = null;
    [SerializeField] private float GracePeriod = .5f;
    [SerializeField] private float FallTime = .8f;

    Collider2D col;

    private void Awake() =>
        col = GetComponent<Collider2D>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.GetComponent<FallingDeathEffect>())
        {
            FallingDeathEffect effect = collision.gameObject.AddComponent<FallingDeathEffect>();
            effect.GracePeriod = GracePeriod;
            effect.KillTime = FallTime;
            effect.FallKillEffect = FallKillEffect;
            effect.targetArea = col;
        }
    }

    public class FallingDeathEffect : MonoBehaviour
    {
        public GameObject FallKillEffect;
        public float GracePeriod;
        float timer;
        public bool Killing = false;
        public float KillTime;
        MonsterCharacter mc;
        public Collider2D targetArea;

        private void Awake() =>
            mc = GetComponent<MonsterCharacter>();

        private void Update()
        {
            timer += Time.deltaTime;

            if (Killing)
            {
                transform.localScale = Vector3.one * (1 - (timer / KillTime));

                if (timer >= KillTime)
                    if (mc == null)
                        Destroy(gameObject);
                    else
                    {
                        (mc as Damageable).Kill();

                        if (FallKillEffect != null)
                            Instantiate(FallKillEffect, transform.position, Quaternion.identity, LevelLoader.TemporaryObjects);
                    }
            }
            else
            {
                if (!targetArea.OverlapPoint(transform.position))
                    Destroy(this);

                if (timer >= GracePeriod)
                {
                    timer = 0;
                    Killing = true;

                    if (mc != null)
                        mc.OnInputRequest = null;

                    //need to remove some scripts that would look bad or cause trouble
                    //pretty hacky way, need to be made better
                    foreach (RagdollOnDeath r in GetComponentsInChildren<RagdollOnDeath>())
                        r.Disabled = true;
                    foreach (SpawnOnEvent s in GetComponentsInChildren<SpawnOnEvent>())
                        s.Disabled = true;

                    //set game object to draw beneath the ground and wall sprites
                    gameObject.SetLayerRecursively(Layers.Ground);
                    transform.position = new Vector3(transform.position.x, transform.position.y, 1f);
                }
            }
        }
    }
}
