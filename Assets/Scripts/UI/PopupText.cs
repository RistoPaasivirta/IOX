using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text), typeof(Outline))]
public class PopupText : MonoBehaviour
{
    [HideInInspector]
    public Color color;
    [HideInInspector]
    public Color outlineColor;
    [HideInInspector]
    public Vector3 worldPos;
    [HideInInspector]
    public Vector2 dir;
    [HideInInspector]
    public string Text;
    [HideInInspector]
    public float LifeTime;
    [HideInInspector]
    public float gravity;

    Text text;
    float timer;

    private void Awake() =>
        text = GetComponent<Text>();

    private void Start()
    {
        text.text = Text;

        text.color = color;
        GetComponent<Outline>().effectColor = outlineColor;
        transform.position = Camera.main.WorldToScreenPoint(worldPos);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > LifeTime)
            Destroy(gameObject);

        dir.y -= Time.deltaTime * gravity;
        worldPos += new Vector3(dir.x, dir.y) * Time.deltaTime;

        text.color = new Color(color.r, color.g, color.b, (1 - timer / LifeTime) * color.a);
        transform.position = Camera.main.WorldToScreenPoint(worldPos);
    }
}