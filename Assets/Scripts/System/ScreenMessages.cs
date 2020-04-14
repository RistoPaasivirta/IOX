using UnityEngine;
using UnityEngine.UI;

public class ScreenMessages : MonoBehaviour
{
    [SerializeField] private Font font;
    [SerializeField] private GameObject FloatingTextPrefab = null;
    [SerializeField] private float MessageCooldown = 1f;
    [SerializeField] private float MessageFadeout = 1f;
    [SerializeField] private float MessageRiseSpeed = 12f;

    float messagesTime = 0f;

    private void Awake()
    {
        Messaging.GUI.ClearDynamicGUI.AddListener(() => 
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        });

        Messaging.GUI.ScreenMessage.AddListener((text, color) => 
        {
            if (string.IsNullOrEmpty(text))
                return;

            //move previous messages up to make room
            if (messagesTime > 0f)
                foreach (FloatingMessage m in GetComponentsInChildren<FloatingMessage>())
                    m.transform.position += Vector3.up * messagesTime * MessageRiseSpeed * 1.25f;

            messagesTime = 1f;

            GameObject message = Instantiate(FloatingTextPrefab);

            Text t = message.GetComponent<Text>();
            t.text = text;
            t.color = color;

            FloatingMessage f = message.AddComponent<FloatingMessage>();
            f.riseSpeed = MessageRiseSpeed;
            f.cooldown = MessageCooldown;
            f.fadeout = MessageFadeout;

            message.transform.SetParent(transform);
            message.transform.localPosition = Vector3.zero;
            (message.transform as RectTransform).pivot = new Vector2(0f, 1f);
        });
    }

    void Update()
    {
        if (messagesTime > 0f)
            messagesTime -= Time.deltaTime;

        if (messagesTime < 0f)
            messagesTime = 0f;
    }

    public class FloatingMessage : MonoBehaviour
    {
        public float cooldown = 6;
        public float fadeout = 3;
        public float riseSpeed = 12;
        float fade;

        void Start()
        {
            text = GetComponent<Text>();
            fade = fadeout;
        }

        Text text;

        void Update()
        {
            transform.position = transform.position += Vector3.up * Time.unscaledDeltaTime * riseSpeed;

            if (cooldown <= 0)
            {
                fadeout -= Time.unscaledDeltaTime;

                if (text != null)
                    text.color = new Color(text.color.r, text.color.g, text.color.b, fadeout / fade);

                if (fadeout <= 0)
                    Destroy(gameObject);
            }
            else
                cooldown -= Time.unscaledDeltaTime;
        }
    }
}
