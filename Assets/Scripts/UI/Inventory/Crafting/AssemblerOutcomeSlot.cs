using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class AssemblerOutcomeSlot : MonoBehaviour
{
    InventoryGUIObject selectedRecipe;
    GUIButton button;

    void Refresh()
    {
        if (PlayerInfo.CurrentLocal.AssemblerItem == null)
            button.SetSprite(null);
        else
            button.SetSprite(PlayerInfo.CurrentLocal.AssemblerItem.InventoryIcon);
    }

    private void OnEnable() =>
        Refresh();

    private void Awake()
    {
        button = GetComponent<GUIButton>();

        Messaging.Crafting.SelectRecipe.AddListener((r) => { selectedRecipe = r; });

        Messaging.Crafting.AssembleSelectedRecipe.AddListener(() => 
        {
            if (selectedRecipe == null)
                return;

            if (PlayerInfo.CurrentLocal.AssemblerItem != null)
                return;

            if (Stash.CraftingMaterials < selectedRecipe.AssembleCost)
            {
                Messaging.GUI.ScreenMessage.Invoke("NOT ENOUGH MATERIALS!", Color.red);
                return;
            }

            Stash.CraftingMaterials -= selectedRecipe.AssembleCost;

            PlayerInfo.CurrentLocal.AssemblerItem = selectedRecipe;
            Messaging.Crafting.SelectRecipe.Invoke(null);
            Messaging.Crafting.CraftingMaterialsUpdated.Invoke();
            Refresh();
        });

        button.onClick.AddListener(() =>
        {
            InventoryGUIObject item = PlayerInfo.CurrentLocal.CursorItem;

            if (item != null)
            {
                PlayerInfo.CurrentLocal.CursorItem = PlayerInfo.CurrentLocal.AssemblerItem;
                PlayerInfo.CurrentLocal.AssemblerItem = item;
                Refresh();
                Messaging.GUI.HoverBox.Invoke(item.GetShortStats);
            }
            else
            {
                if (HotkeyAssigment.InventoryKey)
                {
                    switch (Stash.LeftStashMode)
                    {
                        default:
                        case Stash.StashMode.Disabled:
                            return;

                        case Stash.StashMode.Loadout:
                            if (PlayerInfo.CurrentLocal.AssemblerItem is Weapon)
                            {
                                for (int i = 0; i < PlayerInfo.CurrentLocal.PlayerWeapons.Length; i++)
                                    if (PlayerInfo.CurrentLocal.PlayerWeapons[i] == null)
                                    {
                                        PlayerInfo.CurrentLocal.PlayerWeapons[i] = (Weapon)PlayerInfo.CurrentLocal.AssemblerItem;
                                        PlayerInfo.CurrentLocal.AssemblerItem = null;
                                        Refresh();
                                        Messaging.Player.EquipPlayerItems.Invoke();
                                        Messaging.Player.LoadoutRefresh.Invoke();
                                        Messaging.GUI.HoverBox.Invoke("");
                                        return;
                                    }
                            }
                            else if (PlayerInfo.CurrentLocal.AssemblerItem is Skill)
                            {
                                for (int i = 0; i < PlayerInfo.CurrentLocal.PlayerSkills.Length; i++)
                                    if (PlayerInfo.CurrentLocal.PlayerSkills[i] == null)
                                    {
                                        PlayerInfo.CurrentLocal.PlayerSkills[i] = (Skill)PlayerInfo.CurrentLocal.AssemblerItem;
                                        PlayerInfo.CurrentLocal.AssemblerItem = null;
                                        Refresh();
                                        Messaging.Player.EquipPlayerItems.Invoke();
                                        Messaging.Player.LoadoutRefresh.Invoke();
                                        Messaging.GUI.HoverBox.Invoke("");
                                        return;
                                    }
                            }
                            else if (PlayerInfo.CurrentLocal.AssemblerItem is Accessory)
                            {
                                for (int i = 0; i < PlayerInfo.CurrentLocal.PlayerAccessories.Length; i++)
                                    if (PlayerInfo.CurrentLocal.PlayerAccessories[i] == null)
                                    {
                                        PlayerInfo.CurrentLocal.PlayerAccessories[i] = (Accessory)PlayerInfo.CurrentLocal.AssemblerItem;
                                        PlayerInfo.CurrentLocal.AssemblerItem = null;
                                        Refresh();
                                        Messaging.Player.ActivateAccessory.Invoke(i);
                                        Messaging.Player.LoadoutRefresh.Invoke();
                                        Messaging.GUI.HoverBox.Invoke("");
                                        return;
                                    }
                            }
                            return;

                        case Stash.StashMode.Stash:
                            Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.AssemblerItem, out int slot);
                            Messaging.Player.RefreshStash.Invoke(slot);
                            break;
                    }
                }
                else
                    PlayerInfo.CurrentLocal.CursorItem = PlayerInfo.CurrentLocal.AssemblerItem;

                PlayerInfo.CurrentLocal.AssemblerItem = null;
                Refresh();
                Messaging.GUI.HoverBox.Invoke("");
            }
        });

        button.OnCursorEnter.AddListener(() => 
        {
            if (PlayerInfo.CurrentLocal.AssemblerItem == null)
                Messaging.GUI.HoverBox.Invoke("");
            else
                Messaging.GUI.HoverBox.Invoke(PlayerInfo.CurrentLocal.AssemblerItem.GetShortStats);
        });

        button.OnCursorExit.AddListener(() => { Messaging.GUI.HoverBox.Invoke(""); });
        
        Messaging.Crafting.RefreshAssembler.AddListener(() => { Refresh(); });
    }

    /*private void OnDisable()
    {
        if (PlayerInfo.CurrentLocal.AssemblerItem != null)
        {
            Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.AssemblerItem, out int slot);
            PlayerInfo.CurrentLocal.AssemblerItem = null;
            Refresh();
            Messaging.Player.RefreshStash.Invoke(slot);
        }
    }*/
}
