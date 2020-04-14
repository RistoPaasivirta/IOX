using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class InventoryGUISlot : MonoBehaviour
{
    [SerializeField] private Allowance Allow = Allowance.All;

    GUIButton button;
    int index;

    public enum Allowance
    {
        All,
        Weapons,
        Skills, // Equipment,
        Accessories
    }

    private void Refresh()
    {
        if (Stash.StashItems[index] == null)
            button.SetSprite(null);
        else
            button.SetSprite(Stash.StashItems[index].InventoryIcon);
    }

    private void Awake()
    {
        button = GetComponent<GUIButton>();
        index = transform.parent.GetSiblingIndex();

        Refresh();

        button.onClick.AddListener(() =>
        {
            InventoryGUIObject item = PlayerInfo.CurrentLocal.CursorItem;

            if (item != null)
            {
                switch (Allow)
                {
                    default:
                        return;

                    case Allowance.All:
                        break;

                    case Allowance.Weapons:
                        if (!(item is Weapon))
                            return;
                        break;

                    case Allowance.Skills:
                        if (!(item is Skill))
                            return;
                        break;
                }

                PlayerInfo.CurrentLocal.CursorItem = Stash.StashItems[index];
                Stash.StashItems[index] = item;
                Refresh();
            }
            else
            {
                if (Stash.StashItems[index] == null)
                    return;

                if (HotkeyAssigment.InventoryKey)
                {
                    switch (Stash.LeftStashMode)
                    {
                        default:
                        case Stash.StashMode.Disabled:
                            return;

                        case Stash.StashMode.Loadout:
                            if (Stash.StashItems[index] is Weapon)
                            {
                                for (int i = 0; i < PlayerInfo.CurrentLocal.PlayerWeapons.Length; i++)
                                    if (PlayerInfo.CurrentLocal.PlayerWeapons[i] == null)
                                    {
                                        PlayerInfo.CurrentLocal.PlayerWeapons[i] = (Weapon)Stash.StashItems[index];
                                        Stash.StashItems[index] = null;
                                        Refresh();
                                        Messaging.Player.EquipPlayerItems.Invoke();
                                        Messaging.Player.LoadoutRefresh.Invoke();
                                        return;
                                    }
                            }
                            else if (Stash.StashItems[index] is Skill)
                            {
                                for (int i = 0; i < PlayerInfo.CurrentLocal.PlayerSkills.Length; i++)
                                    if (PlayerInfo.CurrentLocal.PlayerSkills[i] == null)
                                    {
                                        PlayerInfo.CurrentLocal.PlayerSkills[i] = (Skill)Stash.StashItems[index];
                                        Stash.StashItems[index] = null;
                                        Refresh();
                                        Messaging.Player.EquipPlayerItems.Invoke();
                                        Messaging.Player.LoadoutRefresh.Invoke();
                                        return;
                                    }
                            }
                            else if (Stash.StashItems[index] is Accessory)
                            {
                                for (int i = 0; i < PlayerInfo.CurrentLocal.PlayerAccessories.Length; i++)
                                    if (PlayerInfo.CurrentLocal.PlayerAccessories[i] == null)
                                    {
                                        PlayerInfo.CurrentLocal.PlayerAccessories[i] = (Accessory)Stash.StashItems[index];
                                        Stash.StashItems[index] = null;
                                        Refresh();
                                        Messaging.Player.ActivateAccessory.Invoke(i);
                                        Messaging.Player.LoadoutRefresh.Invoke();
                                        return;
                                    }
                            }
                            return;

                        case Stash.StashMode.Stash:
                            switch (Stash.RightStashMode)
                            {
                                default:
                                case Stash.StashMode.Disabled:
                                case Stash.StashMode.Stash:
                                    return;

                                case Stash.StashMode.Assembler:
                                    if (PlayerInfo.CurrentLocal.AssemblerItem != null)
                                        return;

                                    PlayerInfo.CurrentLocal.AssemblerItem = Stash.StashItems[index];
                                    Messaging.Crafting.RefreshAssembler.Invoke();
                                    break;

                                case Stash.StashMode.Disassembler:
                                    if (PlayerInfo.CurrentLocal.DisassemblerItem != null)
                                        return;

                                    PlayerInfo.CurrentLocal.DisassemblerItem = Stash.StashItems[index];
                                    Messaging.Crafting.RefreshAssembler.Invoke();
                                    break;

                                case Stash.StashMode.Augment:
                                    if (PlayerInfo.CurrentLocal.AugmentItem != null)
                                        return;

                                    PlayerInfo.CurrentLocal.AugmentItem = Stash.StashItems[index];
                                    Messaging.Crafting.RefreshAssembler.Invoke();
                                    break;
                            }
                            break;
                    }
                }
                else
                    PlayerInfo.CurrentLocal.CursorItem = Stash.StashItems[index];

                Stash.StashItems[index] = null;
                Refresh();
            }
        });

        button.OnCursorEnter.AddListener(() => {
            if (Stash.StashItems[index] == null)
                Messaging.GUI.HoverBox.Invoke("");
            else
                Messaging.GUI.HoverBox.Invoke(Stash.StashItems[index].GetShortStats);
        });

        button.OnCursorExit.AddListener(() =>
        {
            Messaging.GUI.HoverBox.Invoke("");
        });

        Messaging.Player.RefreshStash.AddListener((i) => 
        {
            if (i == index)
                Refresh();
        });
    }
}
