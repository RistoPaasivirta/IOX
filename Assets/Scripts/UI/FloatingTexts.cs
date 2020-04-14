using UnityEngine;
using UnityEngine.UI;

public class FloatingTexts : MonoBehaviour
{
    [SerializeField] private GUIStyle guiStyle = null;

    private void Awake()
    {
        Messaging.GUI.ClearDynamicGUI.AddListener(() =>
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        });

        Messaging.GUI.FloatingText = new System.Func<Vector3, string, Color, FloatingText>((position, text, color) => 
        {
            GameObject go = new GameObject("floating text");
            Outline ol = go.AddComponent<Outline>();
            ol.effectDistance = new Vector2(1, 1);
            ol.effectColor = new Color(0, 0, 0, 1);

            Text t = go.AddComponent<Text>();
            t.text = text;
            t.color = color;
            t.font = guiStyle.font;
            t.fontStyle = guiStyle.fontStyle;
            t.fontSize = guiStyle.fontSize;
            t.raycastTarget = false;
            t.alignment = TextAnchor.MiddleCenter;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;

            (go.transform as RectTransform).pivot = new Vector2(.5f, .5f);
            go.transform.SetParent(transform);

            FloatingText f = go.AddComponent<FloatingText>();
            f.worldPos = position;
            f.text = t;
            f.transform.position = Camera.main.WorldToScreenPoint(f.worldPos);

            return f;
        });
    }

    public class FloatingText : MonoBehaviour
    {
        public Vector3 worldPos;
        public Text text;
        public Color color;

        private void Update()
        {
            transform.position = Camera.main.WorldToScreenPoint(worldPos);
            text.color = TimeScaler.Paused ? Color.clear : color;
        }
    }
}
