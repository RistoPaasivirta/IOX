using UnityEngine;

[RequireComponent(typeof(CommonUsableObject))]
public class CommsMessageAfterUse : MonoBehaviour
{
    [SerializeField] private int Index = 0;
    [SerializeField] private string Message = "";
    [SerializeField] private Sprite Portrait = null;
    [SerializeField] private int MinimumObjectiveRequired = 0;

    int MissionObjective = 0;

    private void Awake()
    {
        Messaging.Mission.MissionObjective.AddListener((i) => { MissionObjective = i; });

        GetComponent<CommonUsableObject>().AfterUse.AddListener(() => 
        {
            if (MissionObjective >= MinimumObjectiveRequired)
                Messaging.GUI.CommsMessage.Invoke(Index, Portrait, Message);
        });
    }
}
