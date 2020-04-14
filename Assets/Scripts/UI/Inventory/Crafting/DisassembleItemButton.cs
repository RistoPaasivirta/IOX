using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class DisassembleItemButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<GUIButton>().onClick.AddListener(() => 
        {
            if (PlayerInfo.CurrentLocal.DisassemblerItem == null)
                return;

            Stash.CraftingMaterials += PlayerInfo.CurrentLocal.DisassemblerItem.DisassembleProfit;

            PlayerInfo.CurrentLocal.DisassemblerItem = null;

            Messaging.Crafting.RefreshAssembler.Invoke();
            Messaging.Crafting.CraftingMaterialsUpdated.Invoke();
        });
    }
}
