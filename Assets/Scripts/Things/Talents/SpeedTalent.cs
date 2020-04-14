using UnityEngine;

public class SpeedTalent : Talent
{
    [SerializeField] private float Multiplier = 1.2f;

    MonsterCharacter owner;

    public override void Activate(MonsterCharacter character)
    {
        owner = character;
        character.OnCombineMove += Character_OnCombineMove;
    }

    private void Character_OnCombineMove(ref Vector2 move)
    {
        if (owner.ChargePoints == owner.MaxChargePoints)
            move *= Multiplier;
    }

    public override void DeActivate(MonsterCharacter character)
    {
        character.OnCombineMove -= Character_OnCombineMove;
        owner = null;
    }
}
