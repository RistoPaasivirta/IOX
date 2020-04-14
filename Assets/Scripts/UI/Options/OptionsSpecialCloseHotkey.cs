using UnityEngine;

public class OptionsSpecialCloseHotkey : MonoBehaviour
{
    [SerializeField] private string MainMenuOpenWindow = "Main Buttons Menu";
    [SerializeField] private string IngameOpenWindow = "Ingame Menu";
    [SerializeField] private KeyCode Hotkey = KeyCode.Escape;
    [SerializeField] private GameObject PriorityObject = null;

    private void Update()
    {
        if (Input.GetKeyDown(Hotkey))
        {
            if (PriorityObject.activeSelf)
                return;

            if (LevelLoader.LevelLoaded)
                Messaging.GUI.OpenWindow.Invoke(IngameOpenWindow);
            else
                Messaging.GUI.OpenWindow.Invoke(MainMenuOpenWindow);
        }
    }
}
