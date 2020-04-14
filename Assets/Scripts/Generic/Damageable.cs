using UnityEngine;

public enum AttackType
{
    All,
    Melee,
    Bullet,
    Explosion,
    DoT,
}

public enum DamageType
{
    Physical,
    Fire,
    Cold,
    Electrical,
    Toxin,
    Chaos
}

public interface Damageable
{
    bool Dead { get; }
    bool Bleed { get; }
    void Damage(DamageFrame damageFrame);
    void Dot(DotFrame burnFrame);
    void Kill();
    void Impulse(Vector2 direction, float force);
    bool HitSound(Vector3 position, AttackType attackType);
    float Radius { get; }
    Factions Faction { get; }
}

public class DamageFrame
{
    public int amount;
    public AttackType attackType { get; private set; }
    public DamageType damageType { get; private set; }
    public MonsterCharacter attacker { get; private set; }
    public Vector2 hitPosition { get; private set; }

    public DamageFrame() { }
    public DamageFrame(int Damage, AttackType AttackType, DamageType DamageType, MonsterCharacter Attacker, Vector2 HitPosition)
    {
        amount = Damage;
        damageType = DamageType;
        attackType = AttackType;
        attacker = Attacker;
        hitPosition = HitPosition;
    }
}

public class DotFrame
{
    public int totalDamage;
    public DamageType damageType { get; private set; }
    public int damagePerApplication { get; private set; }
    public MonsterCharacter attacker { get; private set; }

    public DotFrame() { }
    public DotFrame(int TotalDamage, int DamagePerApplication, DamageType DamageType, MonsterCharacter Attacker)
    {
        totalDamage = TotalDamage;
        damagePerApplication = DamagePerApplication;
        damageType = DamageType;
        attacker = Attacker;
    }
}