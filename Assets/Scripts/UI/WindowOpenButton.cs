using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class WindowOpenButton : MonoBehaviour
{
    [SerializeField] private string OpenWindow = "Crafting Station";

    private void Awake()
    {
        GetComponent<GUIButton>().onClick.AddListener(() => 
        {
            Messaging.GUI.OpenWindow.Invoke(OpenWindow);
        });
    }
}
