using UnityEngine;

public class MissionTriggerHook : MonoBehaviour
{
    public void InvokeMissionTrigger(string MissionTrigger) =>
        Messaging.Mission.MissionTrigger.Invoke(MissionTrigger);
}
