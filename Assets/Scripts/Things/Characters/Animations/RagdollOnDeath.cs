using UnityEngine;

public class RagdollOnDeath : MonoBehaviour
{
    [SerializeField] private PhysicsMaterial2D gibbet_material = null;
    [SerializeField] private float VelocityMin = 1f;
    [SerializeField] private float VelocityMax = 2f;
    [SerializeField] private float MinLife = 12f;
    [SerializeField] private float MaxLife = 20f;
    [SerializeField] private float MaxSpin = 10;
    [SerializeField] private float LinearDrag = .95f;
    [SerializeField] private float AngularDrag = .95f;
    [SerializeField] private GameObject BleedAttachment = null;
    [SerializeField] private Material RagdollReplaceMaterial = null;
    [SerializeField] private float ImpulseMultiplier = 3f;
    [SerializeField] private SpriteRenderer[] Parts = new SpriteRenderer[0];

    public bool Disabled = false; //hack needed by falling death area 

    void Start()
    {
        MonsterCharacter character = GetComponentInParent<MonsterCharacter>();

        character.OnDeath.AddListener(() => 
        {
            //hack needed by falling death area 
            if (Disabled)
                return;

            if (Parts.Length == 0)
                Parts = GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer sr in Parts)
            {
                sr.transform.SetParent(LevelLoader.TemporaryObjects);

                if (RagdollReplaceMaterial != null)
                    sr.material = RagdollReplaceMaterial;

                Gibbet gibbet = sr.gameObject.AddComponent<Gibbet>();
                gibbet.LifeTime = Random.Range(MinLife, MaxLife);
                gibbet.minVelocity = VelocityMin;
                gibbet.maxVelocity = VelocityMax;
                gibbet.material = gibbet_material;
                gibbet.maxSpin = MaxSpin;
                gibbet.linearDrag = LinearDrag;
                gibbet.angularDrag = AngularDrag;
                gibbet.impulse = character.impulseVector * ImpulseMultiplier;

                if (BleedAttachment != null)
                {
                    GameObject bleed = Instantiate(BleedAttachment, sr.transform);
                    bleed.transform.localPosition = Vector3.zero;
                }
            }
        });
    }
}
