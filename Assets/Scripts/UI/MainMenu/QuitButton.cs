using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class QuitButton : MonoBehaviour
{
    [SerializeField] private bool SaveGameOnQuit = true;

    private void Awake()
    {
        GetComponent<GUIButton>().onClick.AddListener(() => 
        {
            if (SaveGameOnQuit)
            {
                SaveLoadSystem.SaveStash();
                SaveLoadSystem.SaveGame();
            }

            Application.Quit();
        });
    }
}
