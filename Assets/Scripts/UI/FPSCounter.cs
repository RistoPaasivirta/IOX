using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPSCounter : MonoBehaviour
{
    Text text;
    float cooldown = 0f;

    private void Awake()
    {
        text = GetComponent<Text>();

        Messaging.GUI.ShowFPS.AddListener((b) => { gameObject.SetActive(b); });
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (cooldown > 0f)
        {
            cooldown -= Time.unscaledDeltaTime;
            return;
        }
        cooldown = .25f;

        text.text = (int)(1f / Time.unscaledDeltaTime) + " FPS";
    }
}
