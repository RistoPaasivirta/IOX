using UnityEngine;

public class WeaponSwapTalent : Talent
{
    [SerializeField] private float Multiplier = 2f;

    public override void Activate(MonsterCharacter character) =>
        character.AnimationSpeedMultiplier = Multiplier;

    public override void DeActivate(MonsterCharacter character) =>
        character.AnimationSpeedMultiplier = 1f;
}
