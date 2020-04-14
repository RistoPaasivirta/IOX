using UnityEngine;
using UnityEngine.EventSystems;

public class HoverOverText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string HoverText = "";
    
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) =>
        Messaging.GUI.HoverBox.Invoke(HoverText);

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData) =>
        Messaging.GUI.HoverBox.Invoke("");
}
