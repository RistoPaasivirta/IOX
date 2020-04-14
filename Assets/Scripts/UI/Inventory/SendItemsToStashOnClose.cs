using UnityEngine;

[RequireComponent(typeof(GUIWindow))]
public class SendItemsToStashOnClose : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<GUIWindow>().OnClose.AddListener(() => 
        {
            if (PlayerInfo.CurrentLocal.CursorItem != null)
            {
                Messaging.GUI.ScreenMessage.Invoke("Item on cursor sent to stash", Color.white);
                Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.CursorItem, out int slot);
                PlayerInfo.CurrentLocal.CursorItem = null;
            }

            if (PlayerInfo.CurrentLocal.AugmentItem != null)
            {
                Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.AugmentItem, out int slot);
                PlayerInfo.CurrentLocal.AugmentItem = null;
                //Messaging.Player.RefreshStash.Invoke(slot);
            }

            if (PlayerInfo.CurrentLocal.AssemblerItem != null)
            {
                Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.AssemblerItem, out int slot);
                PlayerInfo.CurrentLocal.AssemblerItem = null;
                //Messaging.Player.RefreshStash.Invoke(slot);
            }

            if (PlayerInfo.CurrentLocal.DisassemblerItem != null)
            {
                Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.DisassemblerItem, out int slot);
                PlayerInfo.CurrentLocal.DisassemblerItem = null;
                //Messaging.Player.RefreshStash.Invoke(slot);
            }
        });
    }
}
