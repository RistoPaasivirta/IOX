using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class PlayerSkillInventoryButton : MonoBehaviour
{
    GUIButton button;

    private void Refresh()
    {
        InventoryGUIObject s = PlayerInfo.CurrentLocal.PlayerSkills[transform.parent.GetSiblingIndex()];

        if (s == null)
            button.SetSprite(null);
        else
            button.SetSprite(s.InventoryIcon);
    }

    private void Awake()
    {
        button = GetComponent<GUIButton>();
        int index = transform.parent.GetSiblingIndex();

        button.onClick.AddListener(() =>
        {
            InventoryGUIObject s = PlayerInfo.CurrentLocal.CursorItem;

            if (s != null)
            {
                if (!(s is Skill))
                    return;

                Messaging.Player.DeActivateSkill.Invoke(index);
                PlayerInfo.CurrentLocal.CursorItem = PlayerInfo.CurrentLocal.PlayerSkills[index];
                PlayerInfo.CurrentLocal.PlayerSkills[index] = (Skill)s;
                button.SetSprite(s.InventoryIcon);
                Messaging.Player.EquipPlayerItems.Invoke();
                Messaging.Player.LoadoutRefresh.Invoke();
                Messaging.GUI.HoverBox.Invoke(s.GetShortStats);
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

                            PlayerInfo.CurrentLocal.AssemblerItem = PlayerInfo.CurrentLocal.PlayerSkills[index];
                            Messaging.Crafting.RefreshAssembler.Invoke();
                            break;

                        case Stash.StashMode.Stash:
                            Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.PlayerSkills[index], out int slot);
                            Messaging.Player.RefreshStash.Invoke(slot);
                            break;

                        case Stash.StashMode.Disassembler:
                            if (PlayerInfo.CurrentLocal.DisassemblerItem != null)
                                return;

                            PlayerInfo.CurrentLocal.DisassemblerItem = PlayerInfo.CurrentLocal.PlayerSkills[index];
                            Messaging.Crafting.RefreshAssembler.Invoke();
                            break;

                        case Stash.StashMode.Augment:
                            if (PlayerInfo.CurrentLocal.AugmentItem != null)
                                return;

                            PlayerInfo.CurrentLocal.AugmentItem = PlayerInfo.CurrentLocal.PlayerSkills[index];
                            Messaging.Crafting.RefreshAssembler.Invoke();
                            break;
                    }
                }
                else
                    PlayerInfo.CurrentLocal.CursorItem = PlayerInfo.CurrentLocal.PlayerSkills[index];

                Messaging.Player.DeActivateSkill.Invoke(index);
                PlayerInfo.CurrentLocal.PlayerSkills[index] = null;
                button.SetSprite(null);
                Messaging.Player.EquipPlayerItems.Invoke();
                Messaging.Player.LoadoutRefresh.Invoke();
                Messaging.GUI.HoverBox.Invoke("");
            }
        });

        button.OnCursorEnter.AddListener(() =>
        {
            if (PlayerInfo.CurrentLocal.PlayerSkills[transform.parent.GetSiblingIndex()] == null)
                Messaging.GUI.HoverBox.Invoke("");
            else
                Messaging.GUI.HoverBox.Invoke(PlayerInfo.CurrentLocal.PlayerSkills[transform.parent.GetSiblingIndex()].GetShortStats);
        });

        button.OnCursorExit.AddListener(() => { Messaging.GUI.HoverBox.Invoke(""); });

        Messaging.Player.LoadoutRefresh.AddListener(() => { Refresh(); });
    }

    private void OnEnable() =>
        Refresh();
}
