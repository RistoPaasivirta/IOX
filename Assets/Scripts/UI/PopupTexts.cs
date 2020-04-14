using UnityEngine;

public class PopupTexts : MonoBehaviour
{
    [SerializeField] private PopupText DamageNumberPrefab = null;
    [SerializeField] private PopupText RisingNumberPrefab = null;

    Color ColorFromDamageType(DamageType damageType)
    {
        switch (damageType)
        {
            default:
                return Color.white;

            case DamageType.Fire:
                return new Color(1f, .5f, 0f);
        }
    }

    Color OutlineFromDamageType(DamageType damageType)
    {
        switch (damageType)
        {
            default:
                return Color.black;

            case DamageType.Fire:
                return new Color(.5f, .2f, 0f);
        }
    }

    private void Awake()
    {
        if (DamageNumberPrefab == null)
            return;

        Messaging.GUI.ClearDynamicGUI.AddListener(() =>
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        });

        Messaging.GUI.DamageIndicator.AddListener((position, text, damageType) => 
        {
            PopupText d = Instantiate(DamageNumberPrefab, transform);
            d.worldPos = position;
            d.color = ColorFromDamageType(damageType);
            d.outlineColor = OutlineFromDamageType(damageType);
            d.Text = text;
            d.dir.x = Random.Range(-4f, 4f);
            d.dir.y = Random.Range(4f, 8f);
            d.LifeTime = .6f;
            d.gravity = 10f;
        });

        Messaging.GUI.RisingText.AddListener((position, text, color, outline) =>
        {
            PopupText d = Instantiate(RisingNumberPrefab, transform);
            d.worldPos = position;
            d.color = color;
            d.outlineColor = outline;
            d.Text = text;
            d.dir.x = 0f;
            d.dir.y = 6f;
            d.LifeTime = .8f;
            d.gravity = 0f;
        });
    }
}