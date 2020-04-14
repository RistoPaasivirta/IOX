using System;
using System.Collections.Generic;
using UnityEngine;

public class HandStateAnimation : MonoBehaviour
{
    MonsterCharacter character;
    AnimationState CurrentAnimation;

    [Serializable]
    public struct PartState
    {
        public GameObject TargetObject;
        public Vector3 LocalPosition;
        public Vector3 LocalRotation;
        public Vector3 LocalScale;
    }

    [Serializable]
    public class AnimationState
    {
        public MonsterCharacter.Animations Animation;
        public List<PartState> PartStates;

        public float AnimationPositionSpeed;
        public float AnimationRotationSpeed;
        public float AnimationScaleSpeed;
    }

    [SerializeField] private List<AnimationState> _animationStates = new List<AnimationState>();
    Dictionary<int, AnimationState> AnimationStates = new Dictionary<int, AnimationState>();

    private void Awake()
    {
        character = GetComponentInParent<MonsterCharacter>();
        character.OnDeath.AddListener(() => { enabled = false; });
        character.OnCallAnimation += Character_OnCallAnimation;

        //hash animations
        foreach (AnimationState state in _animationStates)
            AnimationStates.Add((int)state.Animation, state);
    }

    private void Character_OnCallAnimation(MonsterCharacter.Animations animation)
    {
        if (AnimationStates.ContainsKey((int)animation))
            CurrentAnimation = AnimationStates[(int)animation];
    }

    void Update()
    {
        if (CurrentAnimation == null)
            return;

        foreach (PartState ps in CurrentAnimation.PartStates)
        {
            Transform pt = ps.TargetObject.transform;
            pt.localPosition = Vector3.Lerp(pt.localPosition, ps.LocalPosition, Time.deltaTime * CurrentAnimation.AnimationPositionSpeed * character.AnimationSpeedMultiplier);
            pt.localRotation = Quaternion.Slerp(pt.localRotation, Quaternion.Euler(ps.LocalRotation), Time.deltaTime * CurrentAnimation.AnimationRotationSpeed * character.AnimationSpeedMultiplier);
            pt.localScale = Vector3.Lerp(pt.localScale, ps.LocalScale, Time.deltaTime * CurrentAnimation.AnimationScaleSpeed * character.AnimationSpeedMultiplier);
        }
    }
}
