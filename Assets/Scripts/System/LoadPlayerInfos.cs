using UnityEngine;

public class LoadPlayerInfos : MonoBehaviour
{
    private void Awake() =>
        SaveLoadSystem.LoadPlayerInfos();
}
