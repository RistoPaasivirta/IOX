using UnityEngine;

public class Flammable : MonoBehaviour, Damageable
{
    [SerializeField] private GameObject BurningEffectPrefab = null;
    GameObject burningEffect;

    void Damageable.Dot(DotFrame frame)
    {
        if (frame.damageType != DamageType.Fire)
            return;

        if (BurningEffectPrefab != null)
            if (burningEffect == null)
                burningEffect = Instantiate(BurningEffectPrefab, transform.position, Quaternion.identity, transform);
    }

    void Damageable.Damage(DamageFrame damageFrame) { }
    bool Damageable.Dead { get { return false; } }
    bool Damageable.Bleed { get { return false; } }
    float Damageable.Radius { get { return 1f; } }
    Factions Damageable.Faction { get { return Factions.AgainstAll; } }
    bool Damageable.HitSound(Vector3 position, AttackType attackType) { return false; }
    void Damageable.Impulse(Vector2 direction, float force) { }
    void Damageable.Kill() { }
}
