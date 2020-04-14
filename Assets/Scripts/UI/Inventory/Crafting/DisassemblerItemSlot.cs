using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class DisassemblerItemSlot : MonoBehaviour
{
    [SerializeField] private CostUIGrid OutcomeProfitCost = null;
    [SerializeField] private GameObject NoCostText = null;

    GUIButton button;

    void Refresh()
    {
        if (PlayerInfo.CurrentLocal.DisassemblerItem == null)
        {
            button.SetSprite(null);
            OutcomeProfitCost.SetCost(CraftingCost.none);
            NoCostText.gameObject.SetActive(false);
        }
        else
        {
            button.SetSprite(PlayerInfo.CurrentLocal.DisassemblerItem.InventoryIcon);
            OutcomeProfitCost.SetCost(PlayerInfo.CurrentLocal.DisassemblerItem.DisassembleProfit);
            NoCostText.gameObject.SetActive(PlayerInfo.CurrentLocal.DisassemblerItem.DisassembleProfit == CraftingCost.none);
        }
    }

    private void OnEnable() =>
        Refresh();

    private void Awake()
    {
        button = GetComponent<GUIButton>();

        button.onClick.AddListener(() =>
        {
            InventoryGUIObject item = PlayerInfo.CurrentLocal.CursorItem;

            if (item != null)
            {
                PlayerInfo.CurrentLocal.CursorItem = PlayerInfo.CurrentLocal.DisassemblerItem;
                PlayerInfo.CurrentLocal.DisassemblerItem = item;
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
                            if (PlayerInfo.CurrentLocal.DisassemblerItem is Weapon)
                            {
                                for (int i = 0; i < PlayerInfo.CurrentLocal.PlayerWeapons.Length; i++)
                                    if (PlayerInfo.CurrentLocal.PlayerWeapons[i] == null)
                                    {
                                        PlayerInfo.CurrentLocal.PlayerWeapons[i] = (Weapon)PlayerInfo.CurrentLocal.DisassemblerItem;
                                        PlayerInfo.CurrentLocal.DisassemblerItem = null;
                                        Refresh();
                                        Messaging.Player.EquipPlayerItems.Invoke();
                                        Messaging.Player.LoadoutRefresh.Invoke();
                                        Messaging.GUI.HoverBox.Invoke("");
                                        return;
                                    }
                            }
                            else if (PlayerInfo.CurrentLocal.DisassemblerItem is Skill)
                            {
                                for (int i = 0; i < PlayerInfo.CurrentLocal.PlayerSkills.Length; i++)
                                    if (PlayerInfo.CurrentLocal.PlayerSkills[i] == null)
                                    {
                                        PlayerInfo.CurrentLocal.PlayerSkills[i] = (Skill)PlayerInfo.CurrentLocal.DisassemblerItem;
                                        PlayerInfo.CurrentLocal.DisassemblerItem = null;
                                        Refresh();
                                        Messaging.Player.EquipPlayerItems.Invoke();
                                        Messaging.Player.LoadoutRefresh.Invoke();
                                        Messaging.GUI.HoverBox.Invoke("");
                                        return;
                                    }
                            }
                            else if (PlayerInfo.CurrentLocal.DisassemblerItem is Accessory)
                            {
                                for (int i = 0; i < PlayerInfo.CurrentLocal.PlayerAccessories.Length; i++)
                                    if (PlayerInfo.CurrentLocal.PlayerAccessories[i] == null)
                                    {
                                        PlayerInfo.CurrentLocal.PlayerAccessories[i] = (Accessory)PlayerInfo.CurrentLocal.DisassemblerItem;
                                        PlayerInfo.CurrentLocal.DisassemblerItem = null;
                                        Refresh();
                                        Messaging.Player.ActivateAccessory.Invoke(i);
                                        Messaging.Player.LoadoutRefresh.Invoke();
                                        Messaging.GUI.HoverBox.Invoke("");
                                        return;
                                    }
                            }
                            return;

                        case Stash.StashMode.Stash:
                            Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.DisassemblerItem, out int slot);
                            Messaging.Player.RefreshStash.Invoke(slot);
                            break;
                    }
                }
                else
                    PlayerInfo.CurrentLocal.CursorItem = PlayerInfo.CurrentLocal.DisassemblerItem;

                PlayerInfo.CurrentLocal.DisassemblerItem = null;
                Refresh();
                Messaging.GUI.HoverBox.Invoke("");
            }
        });

        button.OnCursorEnter.AddListener(() => 
        {
            if (PlayerInfo.CurrentLocal.DisassemblerItem == null)
                Messaging.GUI.HoverBox.Invoke("");
            else
                Messaging.GUI.HoverBox.Invoke(PlayerInfo.CurrentLocal.DisassemblerItem.GetShortStats);
        });

        button.OnCursorExit.AddListener(() =>
        {
            Messaging.GUI.HoverBox.Invoke("");
        });

        Messaging.Crafting.RefreshAssembler.AddListener(() => { Refresh(); });
    }

    /*private void OnDisable()
    {
        if (PlayerInfo.CurrentLocal.DisassemblerItem != null)
        {
            Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.DisassemblerItem, out int slot);
            PlayerInfo.CurrentLocal.DisassemblerItem = null;
            Messaging.Player.RefreshStash.Invoke(slot);
            Refresh();
        }
    }*/
}
