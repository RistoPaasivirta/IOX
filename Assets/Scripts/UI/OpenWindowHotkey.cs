using UnityEngine;

public class OpenWindowHotkey : MonoBehaviour
{
    [SerializeField] private KeyCode Hotkey = KeyCode.Escape;
    [SerializeField] private string WindowToOpen = "Main Buttons Menu";

    private void Update()
    {
        if (Input.GetKeyDown(Hotkey))
            Messaging.GUI.OpenWindow.Invoke(WindowToOpen);
    }

}
