using UnityEngine;

public class Regenerate : MonoBehaviour
{
    [SerializeField] private float LifeTime = 4f;

    private void Awake()
    {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            RegeneratePart r = sr.gameObject.AddComponent<RegeneratePart>();
            r.LifeTime = LifeTime;
        }
    }
}

public class RegeneratePart : MonoBehaviour
{
    public float LifeTime = 4;

    float timer = 0f;

    void Start()
    {
        materialProperties = new MaterialPropertyBlock();
        sr = GetComponent<SpriteRenderer>();
        sr.GetPropertyBlock(materialProperties);
    }

    SpriteRenderer sr;
    MaterialPropertyBlock materialProperties;

    private void Update()
    {
        timer += Time.deltaTime;

        materialProperties.SetFloat("_Dissolve", 1f - timer / (LifeTime * .65f));
        sr.SetPropertyBlock(materialProperties);
    }
}
