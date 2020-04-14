using UnityEngine;

public class GUIInvokes : MonoBehaviour
{
    public void OpenWindow(string window) =>
        Messaging.GUI.OpenWindow.Invoke(window);
}
