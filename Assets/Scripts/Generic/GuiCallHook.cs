using UnityEngine;
using UnityEngine.Events;

public class GuiCallHook : MonoBehaviour
{
    public UnityEvent OnGuiCall = new UnityEvent();

    private void OnGUI() => 
        OnGuiCall.Invoke();
}
