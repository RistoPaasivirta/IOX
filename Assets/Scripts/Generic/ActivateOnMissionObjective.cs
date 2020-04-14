using UnityEngine;
using UnityEngine.Events;

public class ActivateOnMissionObjective : MonoBehaviour
{
    [SerializeField] private int MinimumMissionObjective = 1;
    [SerializeField] private UnityEvent OnActivate = new UnityEvent();

    private void Awake()
    {
        Messaging.Mission.MissionObjective.AddListener((i) => 
        {
            if (i >= MinimumMissionObjective)
            {
                gameObject.SetActive(true);
                OnActivate.Invoke();
            }
        });

        gameObject.SetActive(false);
    }
}
