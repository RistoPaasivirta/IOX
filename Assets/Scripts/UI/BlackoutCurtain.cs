using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlackoutCurtain : MonoBehaviour
{
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();

        Messaging.GUI.Blackout.AddListener((f) =>
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Clamp01(f));
            gameObject.SetActive(f > 0f);
        });
    }
}
