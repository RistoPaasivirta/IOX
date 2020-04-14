using UnityEngine;

[RequireComponent(typeof(CommonUsableObject))]
public class MissionObjectiveUseObject : MonoBehaviour
{
    [SerializeField] private int RequiredMinimumObjective = 0;
    int MissionObjective = 0;

    private void Awake()
    {
        Messaging.Mission.MissionObjective.AddListener((i) => { MissionObjective = i; });

        GetComponent<CommonUsableObject>().OnUse.AddListener((_) => 
        {
            if (MissionObjective >= RequiredMinimumObjective)
                MissionObjective++;
        });
    }
}
