using UnityEngine;

[RequireComponent(typeof(Behavior), typeof(MonsterCharacter))]
public class LoseInvulnerableOnGainTarget : MonoBehaviour
{
    private void Awake()
    {
        MonsterCharacter character = GetComponent<MonsterCharacter>();

        GetComponent<Behavior>().OnGainTarget.AddListener(() =>
        {
            character.Invulnerable = false;
        });
    }
}
