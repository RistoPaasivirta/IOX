using UnityEngine;

[RequireComponent(typeof(CommonUsableObject))]
public class CraftingStation : MonoBehaviour
{
    [SerializeField] private string OpenGuiWindowName = "Crafting Station";

    private void Awake()
    {
        GetComponent<CommonUsableObject>().OnUse.AddListener((_) => 
        {
            Messaging.GUI.PauseCurtain.Invoke();
            Messaging.GUI.OpenWindow.Invoke(OpenGuiWindowName);
            Messaging.System.SetTimeScale.Invoke(TimeScale.Paused);
            Messaging.GUI.ChangeCursor.Invoke(0);
        });
    }
}