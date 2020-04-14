using UnityEngine;

public class OpenWindowOnStart : MonoBehaviour
{
    [SerializeField] private string OpenWindowName = "Main Buttons Menu";
    
    void Start() =>
        Messaging.GUI.OpenWindow.Invoke(OpenWindowName);
}
