using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GUIButton))]
public class AugmentItemSlot : MonoBehaviour
{
    [SerializeField] private Text ItemNameText = null;
    [SerializeField] private AugmentUpgradeGrid AugmentGrid = null;

    GUIButton button;

    void Refresh()
    {
        if (PlayerInfo.CurrentLocal.AugmentItem == null)
        {
            button.SetSprite(null);
            ItemNameText.text = "";
            AugmentGrid.SetUpgradeOptions(new GameObject[0]);
        }
        else
        {
            button.SetSprite(PlayerInfo.CurrentLocal.AugmentItem.InventoryIcon);
            ItemNameText.text = PlayerInfo.CurrentLocal.AugmentItem.ItemName;
            AugmentGrid.SetUpgradeOptions(PlayerInfo.CurrentLocal.AugmentItem.Upgrades);
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
                PlayerInfo.CurrentLocal.CursorItem = PlayerInfo.CurrentLocal.AugmentItem;
                PlayerInfo.CurrentLocal.AugmentItem = item;
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
                            if (PlayerInfo.CurrentLocal.AugmentItem is Weapon)
                            {
                                for (int i = 0; i < PlayerInfo.CurrentLocal.PlayerWeapons.Length; i++)
                                    if (PlayerInfo.CurrentLocal.PlayerWeapons[i] == null)
                                    {
                                        PlayerInfo.CurrentLocal.PlayerWeapons[i] = (Weapon)PlayerInfo.CurrentLocal.AugmentItem;
                                        PlayerInfo.CurrentLocal.AugmentItem = null;
                                        Refresh();
                                        Messaging.Player.EquipPlayerItems.Invoke();
                                        Messaging.Player.LoadoutRefresh.Invoke();
                                        Messaging.GUI.HoverBox.Invoke("");
                                        return;
                                    }
                            }
                            else if (PlayerInfo.CurrentLocal.AugmentItem is Skill)
                            {
                                for (int i = 0; i < PlayerInfo.CurrentLocal.PlayerSkills.Length; i++)
                                    if (PlayerInfo.CurrentLocal.PlayerSkills[i] == null)
                                    {
                                        PlayerInfo.CurrentLocal.PlayerSkills[i] = (Skill)PlayerInfo.CurrentLocal.AugmentItem;
                                        PlayerInfo.CurrentLocal.AugmentItem = null;
                                        Refresh();
                                        Messaging.Player.EquipPlayerItems.Invoke();
                                        Messaging.Player.LoadoutRefresh.Invoke();
                                        Messaging.GUI.HoverBox.Invoke("");
                                        return;
                                    }
                            }
                            else if (PlayerInfo.CurrentLocal.AugmentItem is Accessory)
                            {
                                for (int i = 0; i < PlayerInfo.CurrentLocal.PlayerAccessories.Length; i++)
                                    if (PlayerInfo.CurrentLocal.PlayerAccessories[i] == null)
                                    {
                                        PlayerInfo.CurrentLocal.PlayerAccessories[i] = (Accessory)PlayerInfo.CurrentLocal.AugmentItem;
                                        PlayerInfo.CurrentLocal.AugmentItem = null;
                                        Refresh();
                                        Messaging.Player.ActivateAccessory.Invoke(i);
                                        Messaging.Player.LoadoutRefresh.Invoke();
                                        Messaging.GUI.HoverBox.Invoke("");
                                        return;
                                    }
                            }
                            return;

                        case Stash.StashMode.Stash:
                            Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.AugmentItem, out int slot);
                            Messaging.Player.RefreshStash.Invoke(slot);
                            break;
                    }
                }
                else
                    PlayerInfo.CurrentLocal.CursorItem = PlayerInfo.CurrentLocal.AugmentItem;

                PlayerInfo.CurrentLocal.AugmentItem = null;
                Refresh();
                Messaging.GUI.HoverBox.Invoke("");
            }
        });

        button.OnCursorEnter.AddListener(() =>
        {
            if (PlayerInfo.CurrentLocal.AugmentItem == null)
                Messaging.GUI.HoverBox.Invoke("");
            else
                Messaging.GUI.HoverBox.Invoke(PlayerInfo.CurrentLocal.AugmentItem.GetShortStats);
        });

        button.OnCursorExit.AddListener(() => { Messaging.GUI.HoverBox.Invoke(""); });


    Messaging.Crafting.RefreshAssembler.AddListener(() => { Refresh(); });
    }

    /*private void OnDisable()
    {
        if (PlayerInfo.CurrentLocal.AugmentItem != null)
        {
            Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.AugmentItem, out int slot);
            PlayerInfo.CurrentLocal.AugmentItem = null;
            Refresh();
            Messaging.Player.RefreshStash.Invoke(slot);
        }
    }*/
}
