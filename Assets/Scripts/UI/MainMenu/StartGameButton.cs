using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class StartGameButton : MonoBehaviour
{
    [SerializeField] private PlayerInfo DefaultPlayerInfo = null;

    private void Awake()
    {
        if (DefaultPlayerInfo == null)
        {
            Debug.LogError("StartGameButton: OnAwake: DefaultPlayerInfo == null");
            return;
        }

        GetComponent<GUIButton>().onClick.AddListener(() =>
        {
            PlayerInfo.CurrentLocal = DefaultPlayerInfo;
            SaveLoadSystem.NewPlayerSlot();
            Messaging.System.ChangeLevel.Invoke(PlayerInfo.CurrentLocal.CurrentMap, 0);
        });
    }
}
