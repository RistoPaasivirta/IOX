using UnityEngine;

public class Gibbet : MonoBehaviour
{
    public PhysicsMaterial2D material;
    public float minVelocity = 1f;
    public float maxVelocity = 10f;
    public float maxSpin = 10f;
    public float linearDrag = .95f;
    public float angularDrag = .95f;
    public Vector2 impulse;
    public float LifeTime = 4;

    SpriteRenderer sr;
    MaterialPropertyBlock materialProperties;
    float timer = 0f;

    void Start()
    {
        gameObject.layer = Layers.Decor;

        gameObject.AddComponent<PolygonCollider2D>();
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.velocity = impulse + Random.insideUnitCircle * Random.Range(minVelocity, maxVelocity);
        rb.sharedMaterial = material;
        rb.angularVelocity = Random.Range(-maxSpin, maxSpin);
        rb.drag = linearDrag;
        rb.angularDrag = angularDrag;

        materialProperties = new MaterialPropertyBlock();
        sr = GetComponent<SpriteRenderer>();
        sr.GetPropertyBlock(materialProperties);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        materialProperties.SetFloat("_Dissolve", Mathf.Min(timer / (LifeTime * .5f), 2) - 1);
        sr.SetPropertyBlock(materialProperties);

        if (timer > LifeTime)
            Destroy(gameObject);
    }
}
