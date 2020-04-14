using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class OptionsSpecialCloseButton : MonoBehaviour
{
    [SerializeField] private string MainMenuOpenWindow = "Main Buttons Menu";
    [SerializeField] private string IngameOpenWindow = "Ingame Menu";

    private void Awake()
    {
        GetComponent<GUIButton>().onClick.AddListener(() => 
        {
            if (LevelLoader.LevelLoaded)
                Messaging.GUI.OpenWindow.Invoke(IngameOpenWindow);
            else
                Messaging.GUI.OpenWindow.Invoke(MainMenuOpenWindow);
        });
    }
}
