using UnityEngine;

public class AlienBehavior : Behavior
{
    [SerializeField] private int StopMovingStepCount = 2;
    [SerializeField] private int MaxAttackConsiderationStepCount = 4;
    [SerializeField] private float DualSwingDelay = .6f;
    [SerializeField] private float IdleChance = .01f;
    [SerializeField] private Vector2 IdleTime = new Vector2(.4f, .8f);
    [SerializeField] private float RunSpeed = 60f;
    [SerializeField] private float RunRange = 20f;
    [SerializeField] private float RunTurnSpeed = 2f;
    [SerializeField] private float StopRange = 2f;
    [SerializeField] private float RandomMoveChance = .01f;
    [SerializeField] private float ChargeTime = 1f;
    [SerializeField] private float ChargeCooldown = 2f;
    [SerializeField] private int MinChargePointsForSkill = 400000;
    [SerializeField] private float MaxChargeAngle = 20f;

    float dualSwingTimer = 0f;
    float hesitateTimer = 0;
    float walkSpeed;
    float walkTurnSpeed;
    float chargeTimer;
    float chargeCooldownTimer;

    protected override void OnAwake()
    {
        base.OnAwake();

        character.OnInputRequest = OnInputRequest;

        walkSpeed = character.walkSpeed;
        walkTurnSpeed = character.TurnSpeed;
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

        frame.WantDirection = TowardsTarget;

        if (character.ChargePoints >= MinChargePointsForSkill)
            if (character.Skills.Length > 0)
                if (character.Skills[0] != null)
                    if (character.SkillCooldowns[0] <= 0f)
                        if (CanSeeTargetGrid)
                            character.Skills[0].Activate(character, 0);

        if (dualSwingTimer > 0f)
            dualSwingTimer -= Time.deltaTime;

        int steps = Vec2I.Max(thing.gridPos, target.thingController.gridPos);

        if (steps <= MaxAttackConsiderationStepCount)
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

        frame.LocalTransform = false;

        if (chargeTimer > 0f)
        {
            if (AngleToTarget(target.transform.position) > MaxChargeAngle)
                chargeTimer = 0f;

            frame.LocalTransform = true;
            chargeTimer -= Time.deltaTime;
            frame.up = 1f;
            character.walkSpeed = RunSpeed;
            character.TurnSpeed = RunTurnSpeed;
            return;
        }

        if (chargeCooldownTimer > 0f)
            chargeCooldownTimer -= Time.deltaTime;

        character.walkSpeed = walkSpeed;
        character.TurnSpeed = walkTurnSpeed;

        float distanceSqr = (target.transform.position - transform.position).sqrMagnitude;

        if (steps > StopMovingStepCount)
            if (chargeCooldownTimer <= 0f)
                if (distanceSqr >= StopRange * StopRange && distanceSqr <= RunRange * RunRange)
                    if (AngleToTarget(target.transform.position) <= MaxChargeAngle)
                        if (CanSeeTargetGrid)
                        {
                            chargeTimer = ChargeTime;
                            chargeCooldownTimer = ChargeCooldown;
                            return;
                        }

        if (hesitateTimer > 0f)
        {
            hesitateTimer -= Time.deltaTime;
            return;
        }

        if (Random.value < IdleChance)
        {
            frame.up = 0;
            frame.right = 0;
            hesitateTimer = Random.Range(IdleTime.x, IdleTime.y);
            return;
        }

        if (Random.value < RandomMoveChance)
        {
            float a = Random.value * Mathf.PI * 2f;
            frame.right = Mathf.Cos(a);
            frame.up = Mathf.Sin(a);
            return;
        }

        //path find to target
        {
            Vec2I smart = SmartMove;

            frame.up = smart.y;
            frame.right = smart.x;
        }
    }
}
