using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class PlayerWeaponInventoryButton : MonoBehaviour
{
    GUIButton button;

    private void Refresh()
    {
        InventoryGUIObject w = PlayerInfo.CurrentLocal.PlayerWeapons[transform.parent.GetSiblingIndex()];

        if (w == null)
            button.SetSprite(null);
        else
            button.SetSprite(w.InventoryIcon);
    }

    private void Awake()
    {
        button = GetComponent<GUIButton>();
        int index = transform.parent.GetSiblingIndex();

        button.onClick.AddListener(() =>
        {
            InventoryGUIObject w = PlayerInfo.CurrentLocal.CursorItem;

            if (w != null)
            {
                if (!(w is Weapon))
                    return;

                if (PlayerInfo.CurrentLocal.PlayerWeapons[index] != null)
                    if (PlayerInfo.CurrentLocal.LastPlayerWeaponSwap == index)
                        Messaging.Player.ForceHolster.Invoke();

                PlayerInfo.CurrentLocal.CursorItem = PlayerInfo.CurrentLocal.PlayerWeapons[index];
                PlayerInfo.CurrentLocal.PlayerWeapons[index] = (Weapon)w;
                button.SetSprite(w.InventoryIcon);
                Messaging.Player.EquipPlayerItems.Invoke();
                Messaging.Player.LoadoutRefresh.Invoke();
                Messaging.GUI.HoverBox.Invoke(w.GetShortStats);
            }
            else
            {
                if (HotkeyAssigment.InventoryKey)
                {
                    switch (Stash.RightStashMode)
                    {
                        default:
                        case Stash.StashMode.Disabled:
                            return;

                        case Stash.StashMode.Assembler:
                            if (PlayerInfo.CurrentLocal.AssemblerItem != null)
                                return;

                            PlayerInfo.CurrentLocal.AssemblerItem = PlayerInfo.CurrentLocal.PlayerWeapons[index];
                            Messaging.Crafting.RefreshAssembler.Invoke();
                            break;

                        case Stash.StashMode.Stash:
                            Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.PlayerWeapons[index], out int slot);
                            Messaging.Player.RefreshStash.Invoke(slot);
                            break;

                        case Stash.StashMode.Disassembler:
                            if (PlayerInfo.CurrentLocal.DisassemblerItem != null)
                                return;

                            PlayerInfo.CurrentLocal.DisassemblerItem = PlayerInfo.CurrentLocal.PlayerWeapons[index];
                            Messaging.Crafting.RefreshAssembler.Invoke();
                            break;

                        case Stash.StashMode.Augment:
                            if (PlayerInfo.CurrentLocal.AugmentItem != null)
                                return;

                            PlayerInfo.CurrentLocal.AugmentItem = PlayerInfo.CurrentLocal.PlayerWeapons[index];
                            Messaging.Crafting.RefreshAssembler.Invoke();
                            break;
                    }
                }
                else
                    PlayerInfo.CurrentLocal.CursorItem = PlayerInfo.CurrentLocal.PlayerWeapons[index];

                if (PlayerInfo.CurrentLocal.PlayerWeapons[index] != null)
                    if (PlayerInfo.CurrentLocal.LastPlayerWeaponSwap == index)
                        Messaging.Player.ForceHolster.Invoke();

                PlayerInfo.CurrentLocal.PlayerWeapons[index] = null;
                button.SetSprite(null);
                Messaging.Player.EquipPlayerItems.Invoke();
                Messaging.Player.LoadoutRefresh.Invoke();
                Messaging.GUI.HoverBox.Invoke("");
            }
        });


        button.OnCursorEnter.AddListener(() =>
        {
            if (PlayerInfo.CurrentLocal.PlayerWeapons[transform.parent.GetSiblingIndex()] == null)
                Messaging.GUI.HoverBox.Invoke("");
            else
                Messaging.GUI.HoverBox.Invoke(PlayerInfo.CurrentLocal.PlayerWeapons[transform.parent.GetSiblingIndex()].GetShortStats);
        });

        button.OnCursorExit.AddListener(() =>
        {
            Messaging.GUI.HoverBox.Invoke("");
            Messaging.Player.LoadoutRefresh.AddListener(() => { Refresh(); });
        });
    }

    private void OnEnable() =>
        Refresh();
}
