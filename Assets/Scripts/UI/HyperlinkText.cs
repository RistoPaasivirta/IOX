using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Text), typeof(Outline))]
public class HyperlinkText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Color MouseOverColor = Color.green;
    [SerializeField] private Color MouseOverOutline = new Color(0f,.5f,0f,1f);

    Text text;
    Outline outline;
    Color normalColor;
    Color outlineColor;

    private void Awake()
    {
        text = GetComponent<Text>();
        normalColor = text.color;

        outline = GetComponent<Outline>();
        outlineColor = outline.effectColor;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        text.color = MouseOverColor;
        outline.effectColor = MouseOverOutline;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        text.color = normalColor;
        outline.effectColor = outlineColor;
    }

    private void OnEnable()
    {
        text.color = normalColor;
        outline.effectColor = outlineColor;
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData) =>
        Application.OpenURL(text.text);
}
