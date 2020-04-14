using UnityEngine;

[RequireComponent(typeof(TrooperBehavior))]
public class AttackCooldownOnGainTarget : MonoBehaviour
{
    [SerializeField] private float Cooldown = 4f;

    TrooperBehavior trooper;
    float originalAggroChance;
    float timer;

    private void Awake()
    {
         trooper = GetComponent<TrooperBehavior>();

        originalAggroChance = trooper.AggroChance;
        trooper.AggroChance = 0f;

        GetComponent<Behavior>().OnGainTarget.AddListener(() =>
        {
            enabled = true;
        });

        enabled = false;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= Cooldown)
        {
            trooper.AggroChance = originalAggroChance;
            enabled = false;
        }
    }
}
