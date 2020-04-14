using UnityEngine;

[RequireComponent(typeof(GUIWindow))]
public class SaveConfigOnClose : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<GUIWindow>().OnClose.AddListener(() => 
        {
            SaveLoadSystem.SaveConfig();
        });
    }
}
