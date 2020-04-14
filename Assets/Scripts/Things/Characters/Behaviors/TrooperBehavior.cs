using UnityEngine;

public class TrooperBehavior : Behavior
{
    [SerializeField] private bool UseOffhand = false;
    [SerializeField] private float AttackTime = 1f;
    [SerializeField] private float AttackHappenTime = .5f;
    [SerializeField] private float SecondaryAttackHappenTime = .2f;
    [SerializeField] private Vector2 DecisionInterval = new Vector2(.4f, .6f);
    [SerializeField] private float AttackTurnSpeedOverride = 5f;
    [SerializeField] private float MoveTurnSpeedOverride = 2f;
    [SerializeField] private float AttackMaxAngle = 10;
    [SerializeField] private float IdleChance = .1f;
    [SerializeField] private float RandomMoveChance = .1f;
    //[SerializeField] private float ClosestMoveChance = .4f;
    [SerializeField] private float EstimatedBulletSpeed = 40f;
    [SerializeField] private float PredictionMultiplier = 1f;
    [SerializeField] private float LookAtPlayerChance = .5f;
    [SerializeField] private float BurstChance = 0f;
    [SerializeField] private bool MoveWhileShooting = false;

    public float AggroChance = .6f; //need to be public for mechboss hack

    float decisionTimer;
    float attackTimer;
    bool attacked;
    bool secondaryAttacked;
    Vector2 MovementDecision;
    Quaternion DirectionDecision;
    bool LookAtTargetOverride;

    protected override void OnAwake()
    {
        base.OnAwake();
        character.OnInputRequest = Hesitate;
    }

    public override void Restart()
    {
        base.Restart();
        character.OnInputRequest = Hesitate;
    }

    private void Hesitate(ref MonsterCharacter.InputFrame frame)
    {
        if (target == null)
            return;

        if (character.CurrentWeapon != null &&
            character.CurrentWeapon.IsReady &&
            Random.value < AggroChance &&
            OnScreen &&
            RayToTarget(character.transform.position))
        //RayToTarget(character.CurrentWeapon.AttackRayStartPosition)) //fails on boss and other multi weapon monsters, also the weapon barrel might be inside wall
        {
            character.OnInputRequest = Attack;
            attackTimer = AttackTime;
            attacked = false;
            secondaryAttacked = false;
            return;
        }

        if (decisionTimer > 0f)
        {
            if (LookAtTargetOverride)
                frame.WantDirection = TowardsTarget;

            decisionTimer -= Time.deltaTime;
            return;
        }

        decisionTimer = Random.Range(DecisionInterval.x, DecisionInterval.y);

        LookAtTargetOverride = Random.value < LookAtPlayerChance;

        if (Random.value < IdleChance)
        {
            MovementDecision = Vector2.zero;
            DirectionDecision = character.LookDirection;
            character.OnInputRequest = Move;
            return;
        }

        if (Random.value < RandomMoveChance)
        {
            float a = Random.value * Mathf.PI * 2f;

            MovementDecision.x = Mathf.Cos(a);
            MovementDecision.y = Mathf.Sin(a);
            DirectionDecision = character.LookDirection;
            character.OnInputRequest = Move;
            return;
        }

        if ((MovementDecision = (Vector2)SmartMove) != Vector2.zero)
        {
            MovementDecision.Normalize();
            DirectionDecision = Quaternion.Euler(0, 0, AxMath.SafeAtan2(MovementDecision.y, MovementDecision.x) * Mathf.Rad2Deg);
            character.OnInputRequest = Move;
        }
    }

    private void Attack(ref MonsterCharacter.InputFrame frame)
    {
        if (target == null)
            return;

        Vector3 predictedPosition = TargetPredictedPosition(EstimatedBulletSpeed, PredictionMultiplier);

        character.TurnSpeed = AttackTurnSpeedOverride;
        frame.WantDirection = TowardsPosition(predictedPosition);

        attackTimer -= Time.deltaTime;

        if (!attacked &&
            attackTimer < AttackHappenTime &&
            OnScreen &&
            AngleToTarget(predictedPosition) < AttackMaxAngle)
        {
            attacked = true;
            character.CurrentWeapon.PointTowards(predictedPosition);
            frame.PrimaryAttackTrigger = true;
        }

        if (UseOffhand &&
            !secondaryAttacked &&
            attackTimer < SecondaryAttackHappenTime &&
            OnScreen &&
            AngleToTarget(predictedPosition) < AttackMaxAngle)
        {
            secondaryAttacked = true;
            character.CurrentOffhand.PointTowards(predictedPosition);
            frame.SecondaryAttackTrigger = true;
        }

        if (MoveWhileShooting)
            if ((MovementDecision = (Vector2)SmartMove) != Vector2.zero)
            {
                MovementDecision.Normalize();
                frame.up = MovementDecision.y;
                frame.right = MovementDecision.x;
            }

        if (attackTimer <= 0f)
        {
            if (Random.value < BurstChance)
            {
                if (OnScreen && RayToTarget(character.CurrentWeapon.AttackRayStartPosition))
                {
                    character.OnInputRequest = Attack;
                    attackTimer = AttackTime;
                    attacked = false;
                    secondaryAttacked = false;
                    return;
                }
            }
            else
                character.OnInputRequest = Hesitate;
        }
    }

    private void Move(ref MonsterCharacter.InputFrame frame)
    {
        if (target == null)
            return;

        character.TurnSpeed = MoveTurnSpeedOverride;
        frame.up = MovementDecision.y;
        frame.right = MovementDecision.x;

        if (LookAtTargetOverride)
            frame.WantDirection = TowardsTarget;
        else
            frame.WantDirection = DirectionDecision;

        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0f)
            character.OnInputRequest = Hesitate;
    }
}
