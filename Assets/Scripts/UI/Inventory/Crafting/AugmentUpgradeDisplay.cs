using UnityEngine;
using UnityEngine.UI;

public class AugmentUpgradeDisplay : MonoBehaviour
{
    [SerializeField] private HoverItemStats ItemHover = null;
    [SerializeField] private Image ItemIcon = null;
    [SerializeField] private CostUIGrid CostGrid = null;
    [SerializeField] private GUIButton UpgradeButton = null;

    public void SetUpgradeItem(InventoryGUIObject item)
    {
        if (item == null)
        {
            Debug.LogError("AugmentUpgradeDisplay: SetUpgradeItem: item == null");
            gameObject.SetActive(false);
            return;
        }

        UpgradeButton.gameObject.SetActive(true);
        CostGrid.gameObject.SetActive(true);

        CostGrid.SetCost(item.AssembleCost);
        ItemIcon.sprite = item.InventoryIcon;
        ItemIcon.color = Color.white;

        ItemHover.Item = item;

        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(() =>
        {
            if (Stash.CraftingMaterials < item.AssembleCost)
            {
                Messaging.GUI.ScreenMessage.Invoke("NOT ENOUGH MATERIALS!", Color.red);
                return;
            }

            Stash.CraftingMaterials -= item.AssembleCost;
            PlayerInfo.CurrentLocal.AugmentItem = item;
            Messaging.Crafting.RefreshAssembler.Invoke();
            Messaging.Crafting.CraftingMaterialsUpdated.Invoke();
        });
    }
}
