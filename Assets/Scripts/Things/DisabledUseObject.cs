using UnityEngine;

[RequireComponent(typeof(CommonUsableObject))]
public class DisabledUseObject : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<CommonUsableObject>().OnUse.AddListener((_) => 
        {
            Messaging.GUI.ScreenMessage.Invoke("CURRENTLY DISABLED", new Color(1, 1, 0));
        });
    }
}
