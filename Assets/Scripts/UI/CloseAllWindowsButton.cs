using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class CloseAllWindowsButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<GUIButton>().onClick.AddListener(() => 
        {
            Messaging.GUI.CloseWindows.Invoke();

            if (LevelLoader.LevelLoaded)
            {
                Messaging.System.SetTimeScale.Invoke(TimeScale.Standard);
                Messaging.GUI.ChangeCursor.Invoke(1);
            }
        });
    }
}
