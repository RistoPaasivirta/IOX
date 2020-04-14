using UnityEngine;

public class DotStack : MonoBehaviour
{
    public DamageType damageType;
    public int damagePerApplication;
    public int totalDamage;

    private void Awake()
    {
        GetComponentInParent<MonsterCharacter>().OnDotStackApply.AddListener((frame) => 
        {
            if (frame.damageType != damageType)
                return;

            frame.amount += Mathf.Min(damagePerApplication, totalDamage);
            totalDamage -= damagePerApplication;

            if (totalDamage <= 0)
                Destroy(gameObject);
        });
    }
}
