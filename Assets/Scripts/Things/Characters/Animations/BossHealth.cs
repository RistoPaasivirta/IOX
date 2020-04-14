using UnityEngine;

[RequireComponent(typeof(MonsterCharacter),typeof(Behavior))]
public class BossHealth : MonoBehaviour
{
    MonsterCharacter character;

    private void Awake()
    {
        character = GetComponent<MonsterCharacter>();

        character.OnDeath.AddListener(() => 
        {
            Messaging.Mission.BossHealth.Invoke(0);
        });

        GetComponent<Behavior>().OnGainTarget.AddListener(() => 
        {
            enabled = true;
        });

        enabled = false;
    }

    private void Update() =>
        Messaging.Mission.BossHealth.Invoke((float)character.HitPoints / (float)character.MaxHitPoints);
}
