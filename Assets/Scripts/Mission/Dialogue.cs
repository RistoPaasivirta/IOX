using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [System.Serializable]
    public class Replique
    {
        public float WaitTime;
        public int Index;
        public string Message;
        public Sprite Portrait;
        public string InvokeMissionTrigger;
    }

    [SerializeField] private List<Replique> Repliques = new List<Replique>();

    float timer = 0f;
    int index = 0;

    public void StartDialogue()
    {
        if (Repliques.Count == 0)
            return;

        timer = Repliques[0].WaitTime;
        index = 0;

        enabled = true;
    }

    private void Start() =>
        enabled = false;

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Replique r = Repliques[index];
            Messaging.GUI.CommsMessage.Invoke(r.Index, r.Portrait, r.Message);

            if (!string.IsNullOrEmpty(r.InvokeMissionTrigger))
                Messaging.Mission.MissionTrigger.Invoke(r.InvokeMissionTrigger);

            index++;
            if (index >= Repliques.Count)
                enabled = false;
            else
                timer = Repliques[index].WaitTime;
        }
    }
}
