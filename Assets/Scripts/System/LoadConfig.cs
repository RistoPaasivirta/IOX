using UnityEngine;

public class LoadConfig : MonoBehaviour
{
    //needs to be done in start to let static data get activated
    private void Start() => 
        SaveLoadSystem.LoadConfig();
}
