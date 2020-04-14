using UnityEngine;

public class FriendlyTrooperBehavior : Behavior
{
    [SerializeField] private string MissionTrigger = "";
    [SerializeField] private int MaxSteps = 10;
    [SerializeField] private bool UseOffhand = false;
    [SerializeField] private string TargetRoomName = "";
    [SerializeField] private float AggroChance = .6f;
    [SerializeField] private float IdleChance = .1f;
    [SerializeField] private float RandomMoveChance = .1f;
    //[SerializeField] private float ClosestMoveChance = .4f;
    [SerializeField] private float EstimatedBulletSpeed = 40f;
    [SerializeField] private float PredictionMultiplier = 1f;
    [SerializeField] private float LookAtTargetChance = .5f;
    [SerializeField] private float BurstChance = 0f;
    [SerializeField] private float AttackTime = 1f;
    [SerializeField] private float AttackHappenTime = .5f;
    [SerializeField] private float SecondaryAttackHappenTime = .2f;
    [SerializeField] private float AttackTurnSpeedOverride = 5f;
    [SerializeField] private float MoveTurnSpeedOverride = 2f;
    [SerializeField] private float AttackMaxAngle = 10f;
    [SerializeField] private string OnExitMessage = "";
    [SerializeField] private Sprite OnExitMessagePortrait = null;
    [SerializeField] private Vector2 DecisionInterval = new Vector2(.4f, .6f);
    [SerializeField] private float LookAtMoveDirChance = .5f;
    [SerializeField] private Vec2I PlayerDistance = new Vec2I(15, 20);
    [SerializeField] private GameObject TeleportEffect = null;

    Vec2I playerGridPos;
    Vec2I moveTarget = Vec2I.illegalMin;
    int targetRoom = -1;
    Vector2 MovementDecision;
    Quaternion DirectionDecision;
    bool LookAtTargetOverride;
    float decisionTimer;
    float attackTimer;
    bool attacked;
    bool secondaryAttacked;
    Vector2 dir = Vector2.zero;
    Vector2 lookDir = Vector2.zero;
    bool LookAtMoveDir = true;
    bool activated = false;
    static int MissionObjective = 0;

    protected override void OnAwake()
    {
        base.OnAwake();

        for (int r = 0; r < Room.Rooms.Length; r++)
            if (Room.Rooms[r].name == TargetRoomName)
            {
                targetRoom = r;
                break;
            }

        Messaging.Mission.MissionTrigger.AddListener((s) =>
        {
            if (s != MissionTrigger)
                return;

            activated = true;
        });

        /*for (int r = 0; r < Room.Rooms.Length; r++)
            if (Room.Rooms[r].ClosestNeighborTowards[targetRoom] != -1)
                Debug.Log("Room \"" + Room.Rooms[r].name + "\" closest neighbor towards targetroom is \"" + Room.Rooms[Room.Rooms[r].ClosestNeighborTowards[targetRoom]].name + "\"");*/

        character.OnInputRequest = StandAround;

        Messaging.Player.PlayerGridPosition.AddListener((p) =>
        {
            playerGridPos = p;
        });
    }

    public override void Restart()
    {
        base.Restart();

        for (int r = 0; r < Room.Rooms.Length; r++)
            if (Room.Rooms[r].name == TargetRoomName)
            {
                targetRoom = r;
                break;
            }

        character.OnInputRequest = MoveOut;
    }

    private void StandAround(ref MonsterCharacter.InputFrame frame)
    {
        if (target != null)
        {
            character.OnInputRequest = Hesitate;
            return;
        }

        Vec2I dif = playerGridPos - thing.gridPos;
        frame.WantDirection = Quaternion.Euler(0, 0, AxMath.SafeAtan2(dif.y, dif.x) * Mathf.Rad2Deg);

        if (decisionTimer > 0f)
        {
            decisionTimer -= Time.deltaTime;
            return;
        }

        decisionTimer = Random.Range(DecisionInterval.x, DecisionInterval.y);

        if (activated)
        {
            if (Vec2I.Manhattan(playerGridPos, thing.gridPos) <= PlayerDistance.x)
            {
                character.OnInputRequest = MoveOut;
                return;
            }

            if (targetRoom >= 0 || targetRoom < Room.Rooms.Length)
            {
                Room room = Room.Rooms[targetRoom];

                if (TheGrid.Valid(playerGridPos) && TheGrid.Valid(thing.gridPos))
                    if (room.steps[playerGridPos.x, playerGridPos.y] < room.steps[thing.gridPos.x, thing.gridPos.y])
                    {
                        character.OnInputRequest = MoveOut;
                        return;
                    }
            }
        }
    }
    
    bool PlayerTooFar
    {
        get
        {

            if (Vec2I.Manhattan(playerGridPos, thing.gridPos) < PlayerDistance.y)
                return false;

            if (targetRoom >= 0 || targetRoom < Room.Rooms.Length)
            {
                Room room = Room.Rooms[targetRoom];

                if (TheGrid.Valid(playerGridPos) && TheGrid.Valid(thing.gridPos))
                    if (room.steps[playerGridPos.x, playerGridPos.y] > room.steps[thing.gridPos.x, thing.gridPos.y])
                        return true;
            }

            return false;
        }
    }

    private void MoveOut(ref MonsterCharacter.InputFrame frame)
    {
        if (target != null)
        {
            character.OnInputRequest = Hesitate;
            return;
        }

        if (PlayerTooFar)
            character.OnInputRequest = StandAround;

        if (moveTarget != Vec2I.illegalMin)
        {
            Vector2 dif = TheGrid.WorldPosition(moveTarget) - (Vector2)transform.position;

            if (dif != Vector2.zero)
                dir = dif.normalized;

            frame.up = dir.y;
            frame.right = dir.x;

            if (LookAtMoveDir)
                lookDir = dir;

            frame.WantDirection = Quaternion.Euler(0, 0, AxMath.SafeAtan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg);
        }

        if (decisionTimer > 0f)
        {
            decisionTimer -= Time.deltaTime;
            return;
        }

        decisionTimer = Random.Range(DecisionInterval.x, DecisionInterval.y);

        LookAtMoveDir = Random.value < LookAtMoveDirChance;

        if (targetRoom < 0 || targetRoom >= Room.Rooms.Length)
            return;

        Room room = Room.Rooms[targetRoom];

        if (Vec2I.Max(room.gridPos, thing.gridPos) < 3)
        {
            if (MissionObjective == 0)
            {
                Messaging.GUI.CommsMessage.Invoke(0, OnExitMessagePortrait, OnExitMessage);
                MissionObjective++;
                Messaging.Mission.MissionStatus.Invoke(true);
            }

            Instantiate(TeleportEffect, transform.position, Quaternion.identity, LevelLoader.TemporaryObjects);

            Destroy(gameObject);
        }

        if (!AI.RoomPath(thing.gridPos, room, MaxSteps, out Vec2I[] path))
            return;

        if (path.Length < 2)
            return;

        if (path.Length == 2)
            moveTarget = path[1];

        if (path.Length >= 3)
            moveTarget = AI.NaturalStep(path, MaxSteps);
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, TheGrid.WorldPosition(moveTarget));
    }*/

    private void Hesitate(ref MonsterCharacter.InputFrame frame)
    {
        if (target == null)
        {
            if (activated)
                character.OnInputRequest = MoveOut;
            else
                character.OnInputRequest = StandAround;

            return;
        }

        if (character.CurrentWeapon != null &&
            character.CurrentWeapon.IsReady &&
            Random.value < AggroChance &&
            OnScreen &&
            RayToTarget(character.CurrentWeapon.AttackRayStartPosition))
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

        LookAtTargetOverride = Random.value < LookAtTargetChance;

        if (activated)
        {
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
    }

    private void Attack(ref MonsterCharacter.InputFrame frame)
    {
        if (target == null)
        {
            if (activated)
                character.OnInputRequest = MoveOut;
            else
                character.OnInputRequest = StandAround;

            return;
        }

        frame.up = MovementDecision.y;
        frame.right = MovementDecision.x;

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
        {
            if (activated)
                character.OnInputRequest = MoveOut;
            else
                character.OnInputRequest = StandAround;

            return;
        }

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
