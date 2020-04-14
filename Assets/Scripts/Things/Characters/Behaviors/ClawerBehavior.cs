using UnityEngine;

public class ClawerBehavior : Behavior
{
    [SerializeField] private int StopMovingStepCount = 2;
    [SerializeField] private int MaxAttackConsiderationStepCount = 4;
    [SerializeField] private float DualSwingDelay = .6f;

    float dualSwingTimer = 0f;

    protected override void OnAwake()
    {
        base.OnAwake();

        character.OnInputRequest = OnInputRequest;
    }

    public override void Restart()
    {
        base.Restart();

        character.OnInputRequest = OnInputRequest;
    }

    private void OnInputRequest(ref MonsterCharacter.InputFrame frame)
    {
        if (target == null)
            return;

        if (dualSwingTimer > 0f)
            dualSwingTimer -= Time.deltaTime;

        frame.LocalTransform = true;

        int steps = Vec2I.Max(thing.gridPos, target.thingController.gridPos);

        if (steps <= MaxAttackConsiderationStepCount)
        {
            frame.WantDirection = TowardsTarget;

            if (character.CurrentWeapon.IsReady && character.CurrentOffhand.IsReady)
            {
                if (Random.value > .5f)
                    frame.PrimaryAttackContinuous = true;
                else
                    frame.SecondaryAttackContinuous = true;

                dualSwingTimer = DualSwingDelay;
            }
            else if (dualSwingTimer <= 0f)
            {
                frame.PrimaryAttackContinuous = true;
                frame.SecondaryAttackContinuous = true;
            }

            frame.up = 1f;
            return;
        }
                
        if (CanSeeTargetGrid)
        {
            frame.WantDirection = TowardsTarget;
            if (steps > StopMovingStepCount)
                frame.up = 1f;

            return;
        }

        Vec2I smart = SmartMove;
        if (SmartMove != Vec2I.zero)
        {
            frame.WantDirection = Quaternion.Euler(0, 0, AxMath.SafeAtan2(smart.y, smart.x) * Mathf.Rad2Deg);
            if (steps > StopMovingStepCount)
                frame.up = 1;
        }
    }
}
