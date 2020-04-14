using UnityEngine;

public class SaveScumCop : MonoBehaviour
{
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveLoadSystem.SaveStash();
            SaveLoadSystem.SaveGame();
        }
    }

    private void OnApplicationQuit()
    {
        SaveLoadSystem.SaveStash();
        SaveLoadSystem.SaveGame();
    }
}
