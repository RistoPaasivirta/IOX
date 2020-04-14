using UnityEngine;
using UnityEngine.Events;

public class UpdateCallHook : MonoBehaviour
{
    public UnityEvent OnUpdateCall = new UnityEvent();

    private void Update() =>
        OnUpdateCall.Invoke();

}
