
using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class ContinueStartButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<GUIButton>().onClick.AddListener(() =>
        {
            SaveLoadSystem.LoadGame();
            SaveLoadSystem.LoadStash();
        });
    }
}
