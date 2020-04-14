using UnityEngine;
using UnityEngine.Events;

public class OnBeginMissionEvent : MonoBehaviour
{
    public UnityEvent OnBeginMission = new UnityEvent();

    private void Awake() =>
        Messaging.Mission.BeginMission.AddListener(() => { OnBeginMission.Invoke(); });
}
