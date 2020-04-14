using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class DeleteCharacterButton : MonoBehaviour
{
    [SerializeField] private string SelectionWindow = "Continue Window";
    [SerializeField] private string NoCharactersWindow = "Main Buttons Menu";

    private void Awake()
    {
        GetComponent<GUIButton>().onClick.AddListener(() =>
        {
            SaveLoadSystem.DeletePlayerInfo();

            if (PlayerInfoHolder.LoadedHolders.Count == 0)
                Messaging.GUI.OpenWindow.Invoke(NoCharactersWindow);
            else
                Messaging.GUI.OpenWindow.Invoke(SelectionWindow);
        });
    }
}
