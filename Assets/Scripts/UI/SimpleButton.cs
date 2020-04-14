using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SimpleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private AudioClip EnterSound = null;
    [SerializeField] private AudioClip ClickSound = null;

    public UnityEvent OnClick = new UnityEvent();
    public UnityEvent OnCursorEnter = new UnityEvent();
    public UnityEvent OnCursorExit = new UnityEvent();

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ClickSound != null)
            Sounds.Create2DSound(ClickSound, Sounds.MixerGroup.Interface, .2f, 1f, 1f, 255);

        OnClick.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EnterSound != null)
            Sounds.Create2DSound(EnterSound, Sounds.MixerGroup.Interface, .2f, 1f, 1f, 255);

        OnCursorEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnCursorExit.Invoke();
    }
}
