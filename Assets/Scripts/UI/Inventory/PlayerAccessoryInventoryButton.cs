using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class PlayerAccessoryInventoryButton : MonoBehaviour
{
    GUIButton button;

    private void Refresh()
    {
        InventoryGUIObject a = PlayerInfo.CurrentLocal.PlayerAccessories[transform.parent.GetSiblingIndex()];

        if (a == null)
            button.SetSprite(null);
        else
            button.SetSprite(a.InventoryIcon);
    }

    private void Awake()
    {
        button = GetComponent<GUIButton>();

        int index = transform.parent.GetSiblingIndex();
        
        button.onClick.AddListener(() =>
        {
            InventoryGUIObject a = PlayerInfo.CurrentLocal.CursorItem;

            if (a != null)
            {
                if (!(a is Accessory))
                    return;

                Messaging.Player.DeActivateAccessory.Invoke(index);
                PlayerInfo.CurrentLocal.CursorItem = PlayerInfo.CurrentLocal.PlayerAccessories[index];
                PlayerInfo.CurrentLocal.PlayerAccessories[index] = (Accessory)a;
                button.SetSprite(a.InventoryIcon);
                Messaging.Player.ActivateAccessory.Invoke(index);
                Messaging.Player.LoadoutRefresh.Invoke();
                Messaging.GUI.HoverBox.Invoke(a.GetShortStats);
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

                            PlayerInfo.CurrentLocal.AssemblerItem = PlayerInfo.CurrentLocal.PlayerAccessories[index];
                            Messaging.Crafting.RefreshAssembler.Invoke();
                            break;

                        case Stash.StashMode.Stash:
                            Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.PlayerAccessories[index], out int slot);
                            Messaging.Player.RefreshStash.Invoke(slot);
                            break;

                        case Stash.StashMode.Disassembler:
                            if (PlayerInfo.CurrentLocal.DisassemblerItem != null)
                                return;

                            PlayerInfo.CurrentLocal.DisassemblerItem = PlayerInfo.CurrentLocal.PlayerAccessories[index];
                            Messaging.Crafting.RefreshAssembler.Invoke();
                            break;

                        case Stash.StashMode.Augment:
                            if (PlayerInfo.CurrentLocal.AugmentItem != null)
                                return;

                            PlayerInfo.CurrentLocal.AugmentItem = PlayerInfo.CurrentLocal.PlayerAccessories[index];
                            Messaging.Crafting.RefreshAssembler.Invoke();
                            break;
                    }
                }
                else
                    PlayerInfo.CurrentLocal.CursorItem = PlayerInfo.CurrentLocal.PlayerAccessories[index];

                Messaging.Player.DeActivateAccessory.Invoke(index);
                PlayerInfo.CurrentLocal.PlayerAccessories[index] = null;
                button.SetSprite(null);
                Messaging.Player.LoadoutRefresh.Invoke();
                Messaging.GUI.HoverBox.Invoke("");
            }
        });

        button.OnCursorEnter.AddListener(() =>
        {
            if (PlayerInfo.CurrentLocal.PlayerAccessories[transform.parent.GetSiblingIndex()] == null)
                Messaging.GUI.HoverBox.Invoke("");
            else
                Messaging.GUI.HoverBox.Invoke(PlayerInfo.CurrentLocal.PlayerAccessories[transform.parent.GetSiblingIndex()].GetShortStats);
        });

        button.OnCursorExit.AddListener(() => { Messaging.GUI.HoverBox.Invoke(""); });

        Messaging.Player.LoadoutRefresh.AddListener(() => { Refresh(); });
    }

    private void OnEnable() =>
        Refresh();
}
