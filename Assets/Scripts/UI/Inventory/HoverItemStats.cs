using UnityEngine;
using UnityEngine.EventSystems;

public class HoverItemStats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public InventoryGUIObject Item;

    bool hover;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (!hover)
            if (Item != null)
            {
                Messaging.GUI.HoverBox.Invoke(Item.GetShortStats);
                hover = true;
            }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (hover)
        {
            Messaging.GUI.HoverBox.Invoke("");
            hover = false;
        }
    }

    private void OnEnable() =>
        hover = false;
}
