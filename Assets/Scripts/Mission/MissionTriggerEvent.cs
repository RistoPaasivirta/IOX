using UnityEngine;
using UnityEngine.Events;

public class MissionTriggerEvent : MonoBehaviour
{
    [SerializeField] private string MissionTrigger = "";
    [SerializeField] private UnityEvent InvokeEvent = new UnityEvent();

    private void Awake()
    {
        Messaging.Mission.MissionTrigger.AddListener((s) => 
        {
            if (s == MissionTrigger)
                InvokeEvent.Invoke();
        });
    }
}
