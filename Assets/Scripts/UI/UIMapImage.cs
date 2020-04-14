using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIMapImage : MonoBehaviour
{
    [SerializeField] private Vector2 Padding = new Vector2(400, 200);
    [SerializeField] private float MinScale = .5f;
    [SerializeField] private float MaxScale = 2;

    float currentScale = 1;
    Image image;
    RectTransform parentRT;

    private void Awake()
    {
        image = GetComponent<Image>();
        parentRT = transform.parent as RectTransform;

        Messaging.GUI.UIMapSprite.AddListener((s, _, __) =>
        {
            image.sprite = s;
            image.SetNativeSize();

            parentRT.sizeDelta = image.rectTransform.sizeDelta + Padding;
        });
    }

    void Update() =>
        Zoom(Input.GetAxis("Mouse ScrollWheel"));

    void Zoom(float increment)
    {
        currentScale += increment;
        currentScale = Mathf.Clamp(currentScale, MinScale, MaxScale);
        image.rectTransform.localScale = new Vector2(currentScale, currentScale);
    }
}
