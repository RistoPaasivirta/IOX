using UnityEngine;

public class CommsMessage : MonoBehaviour
{
    [SerializeField] private int Index = 0;
    [SerializeField] private string Message = "";
    [SerializeField] private Sprite Portrait = null;

    public void SendCommsMessage() => 
        Messaging.GUI.CommsMessage.Invoke(Index, Portrait, Message);
}