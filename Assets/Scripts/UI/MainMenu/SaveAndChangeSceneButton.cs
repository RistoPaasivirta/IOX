using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GUIButton))]
public class SaveAndChangeSceneButton : MonoBehaviour
{
    [SerializeField] private string SceneToLoad = "MainMenu";

    private void Awake()
    {
        GetComponent<GUIButton>().onClick.AddListener(() => 
        {
            SaveLoadSystem.SaveStash();
            SaveLoadSystem.SaveGame();
            LevelLoader.DestroyLevel();
            SceneManager.LoadScene(SceneToLoad);
        });
    }
}
