using System.Collections.Generic;
using UnityEngine;

public class LootMessages : MonoBehaviour
{
    [SerializeField] private GameObject LootMessagePrefab = null;
    [SerializeField] private float PendingTime = .6f;
    [SerializeField] private float MessageCooldown = 1f;
    [SerializeField] private float MessageFadeout = 1f;
    [SerializeField] private float MessageRiseSpeed = 12f;

    float messagesTime = 0f;
    List<InventoryGUIObject> Pending = new List<InventoryGUIObject>();

    private void Awake()
    {
        Messaging.GUI.ClearDynamicGUI.AddListener(() => 
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        });

        Messaging.GUI.LootMessage.AddListener((item, time) => 
        {
            if (item == null)
                return;

            if (messagesTime < time)
                messagesTime = time;

            Pending.Add(item);
        });
    }

    void Update()
    {
        if (messagesTime > 0f)
            messagesTime -= Time.unscaledDeltaTime;

        if (messagesTime <= 0f)
            if (Pending.Count > 0)
            {
                messagesTime = PendingTime;

                GameObject message = Instantiate(LootMessagePrefab);

                LootMessage l = message.GetComponentInChildren<LootMessage>();
                l?.SetItemInfo(Pending[0]);
                Pending.RemoveAt(0);

                FloatingMessage f = message.AddComponent<FloatingMessage>();
                f.riseSpeed = MessageRiseSpeed;
                f.cooldown = MessageCooldown;
                f.fadeout = MessageFadeout;

                message.transform.SetParent(transform);
                message.transform.localPosition = Vector3.zero;
                (message.transform as RectTransform).pivot = new Vector2(1f, 1f);
            }
    }

    public class FloatingMessage : MonoBehaviour
    {
        CanvasGroup cg;

        public float cooldown = 6;
        public float fadeout = 3;
        public float riseSpeed = 12;
        float fade;

        private void Awake()
        {
            cg = GetComponent<CanvasGroup>();
        }

        void Start()
        {
            fade = fadeout;
        }

        void Update()
        {
            transform.position = transform.position += Vector3.up * Time.unscaledDeltaTime * riseSpeed;

            if (cooldown <= 0)
            {
                fadeout -= Time.unscaledDeltaTime;

                cg.alpha = fadeout / fade;

                if (fadeout <= 0)
                    Destroy(gameObject);
            }
            else
                cooldown -= Time.unscaledDeltaTime;
        }
    }
}
