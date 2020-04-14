using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public AudioClip EnterSound = null;
    public AudioClip ClickSound = null;
    public Color NormalColor = Color.white;
    public Color EnterColor = Color.green;
    public UnityEvent onClick = new UnityEvent();
    public UnityEvent OnCursorEnter = new UnityEvent();
    public UnityEvent OnCursorExit = new UnityEvent();

    Image image;

    private void Awake() =>
        image = GetComponent<Image>();

    private void OnEnable() =>
        image.color = image.sprite == null ? Color.clear : NormalColor;

    public void SetSprite(Sprite sprite)
    {
        if (image == null)
            image = GetComponent<Image>();
        
        image.sprite = sprite;
        image.color = image.sprite == null ? Color.clear : NormalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ClickSound != null)
            Sounds.Create2DSound(ClickSound, Sounds.MixerGroup.Interface, .2f, 1f, 1f, 255);

        onClick.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = image.sprite == null ? Color.clear : EnterColor;

        if (EnterSound != null)
            Sounds.Create2DSound(EnterSound, Sounds.MixerGroup.Interface, .2f, 1f, 1f, 255);

        OnCursorEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = image.sprite == null ? Color.clear : NormalColor;

        OnCursorExit.Invoke();
    }
}
