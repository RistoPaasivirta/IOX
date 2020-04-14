using UnityEngine;
using UnityEngine.Events;

public class ToggleOnCharacterAnimation : MonoBehaviour
{
    [SerializeField] private MonsterCharacter.Animations DisableAnimation = MonsterCharacter.Animations.TwoHandedMeleeSwing;
    [SerializeField] private MonsterCharacter.Animations EnableAnimation = MonsterCharacter.Animations.TwoHandedMeleeIdle;
    [SerializeField] private UnityEvent OnEnable = new UnityEvent();
    [SerializeField] private UnityEvent OnDisable = new UnityEvent();

    MonsterCharacter character;

    private void Start()
    {
        character = GetComponentInParent<MonsterCharacter>();

        if (character != null)
            character.OnCallAnimation += Character_OnCallAnimation;
    }

    private void Character_OnCallAnimation(MonsterCharacter.Animations animation)
    {
        if (animation == DisableAnimation)
            OnDisable.Invoke();

        if (animation == EnableAnimation)
            OnEnable.Invoke();
    }

    private void OnDestroy() =>
        character.OnCallAnimation -= Character_OnCallAnimation;
}
