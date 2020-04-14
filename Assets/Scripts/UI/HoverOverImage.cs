using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HoverOverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float Speed = 20f;

    Image image;
    bool pointerOver;
    float alpha = 0f;

    private void Awake() =>
        image = GetComponent<Image>();

    private void OnEnable()
    {
        alpha = 0f;
        pointerOver = false;
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => pointerOver = true;
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => pointerOver = false;

    private void Update()
    {
        alpha = Mathf.Lerp(alpha, pointerOver ? 1 : 0, Time.unscaledDeltaTime * Speed);

        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }
}
