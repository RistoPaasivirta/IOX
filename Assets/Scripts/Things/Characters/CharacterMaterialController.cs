using UnityEngine;

public class CharacterMaterialController : MonoBehaviour
{
    [SerializeField] private Material DeathMaterial = null;
    [SerializeField] private Color OutlineColor = Color.red;
    [SerializeField] private float OutlineWidthDisabled = 0f;
    [SerializeField] private float OutlineWidthEnabled = .1f;
    [SerializeField] private float deathTime = 4;

    float delayTimer;
    float deathTimer;

    SkinnedMeshRenderer mr;
    MaterialPropertyBlock materialProperties;

    private void Awake()
    {
        mr = GetComponentInChildren<SkinnedMeshRenderer>();

        if (mr == null)
        {
            Debug.LogError("CharacterMaterialController: Awake: Could not find skinned mesh renderer");
            return;
        }

        materialProperties = new MaterialPropertyBlock();

        ThingController thingController = GetComponent<ThingController>();
        if (thingController != null)
        {
            thingController.OnMouseEnter.AddListener(() => { Highlight(true); });
            thingController.OnMouseLeave.AddListener(() => { Highlight(false); });
        }

        if (DeathMaterial != null)
        {
            MonsterCharacter character = GetComponent<MonsterCharacter>();
            if (character != null)
                character.OnDeath.AddListener(OnDeath);
        }

        materialProperties.SetColor("_OutlineColor", OutlineColor);
        materialProperties.SetFloat("_Outline", OutlineWidthDisabled);
        mr.SetPropertyBlock(materialProperties);

        enabled = false;
    }

    private void OnDeath()
    {
        mr.material = DeathMaterial;
        enabled = true;

        materialProperties.SetFloat("_Dissolve", -1);
        mr.SetPropertyBlock(materialProperties);
    }

    public void Highlight(bool enabled)
    {
        materialProperties.SetFloat("_Outline", enabled ? OutlineWidthEnabled : OutlineWidthDisabled);
        mr.SetPropertyBlock(materialProperties);
    }

    private void Update()
    {
        if (delayTimer < Options.RagdollTime)
            delayTimer += Time.deltaTime;
        else
        {
            deathTimer += Time.deltaTime;

            materialProperties.SetFloat("_Dissolve", Mathf.Min(deathTimer / (deathTime * .5f), 2) - 1);
            mr.SetPropertyBlock(materialProperties);

            if (deathTimer > deathTime * .6f)
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            if (deathTimer > deathTime)
                Destroy(gameObject);
        }
    }
}
