using UnityEngine;

public class CloseWindowsHotkey : MonoBehaviour
{
    [SerializeField] private KeyCode Hotkey = KeyCode.Escape;

    private void Update()
    {
        if (Input.GetKeyDown(Hotkey))
        {
            Messaging.GUI.CloseWindows.Invoke();

            if (LevelLoader.LevelLoaded)
            {
                Messaging.System.SetTimeScale.Invoke(TimeScale.Standard);
                Messaging.GUI.ChangeCursor.Invoke(1);
            }
        }
    }
}
