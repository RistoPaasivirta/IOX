using UnityEngine;
using UnityEngine.UI;

public class LootMessage : MonoBehaviour
{
    [SerializeField] private Image ItemIcon = null;
    [SerializeField] private Text ItemName = null;
    [SerializeField] private Outline Outline = null;

    public void SetItemInfo(InventoryGUIObject item)
    {
        ItemIcon.sprite = item.InventoryIcon;
        ItemName.text = item.ItemName;
        ItemName.color = item.MainColor;
        Outline.effectColor = item.SecondaryColor;
    }
}
