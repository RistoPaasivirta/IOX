using UnityEngine;

public class ResetTimeScale : MonoBehaviour
{
    private void Awake() =>
        Messaging.System.SetTimeScale.Invoke(TimeScale.Standard);
}
