using UnityEngine;

public class RoomWalkTestBehavior : Behavior
{
    [SerializeField] private string TargetRoomName = "";
    [SerializeField] private string MissionTrigger = "";
    [SerializeField] private float StandAroundTurnSpeed = 6f;
    [SerializeField] private float MoveOutTurnSpeed = 1f;
    [SerializeField] private int MaxSteps = 10;
    [SerializeField] private Vector2 DecisionInterval = new Vector2(.4f, .6f);
    [SerializeField] private float LookAtMoveDirChance = .5f;

    int targetRoom = -1;
    float decisionTimer;
    Vector2 dir = Vector2.zero;
    Vector2 lookDir = Vector2.zero;
    bool LookAtMoveDir = true;
    Vec2I moveTarget;
    MonsterCharacter lookatTarget;

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

            character.OnInputRequest = MoveOut;
            character.TurnSpeed = MoveOutTurnSpeed;
        });

        /*for (int r = 0; r < Room.Rooms.Length; r++)
            if (Room.Rooms[r].ClosestNeighborTowards[targetRoom] != -1)
                Debug.Log("Room \"" + Room.Rooms[r].name + "\" closest neighbor towards targetroom is \"" + Room.Rooms[Room.Rooms[r].ClosestNeighborTowards[targetRoom]].name + "\"");*/

        character.OnInputRequest = StandAround;
        character.TurnSpeed = StandAroundTurnSpeed;
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
        character.TurnSpeed = MoveOutTurnSpeed;
    }

    private void StandAround(ref MonsterCharacter.InputFrame frame)
    {
        if (lookatTarget != null)
        {
            Vector2 dif = lookatTarget.transform.position - transform.position;
            frame.WantDirection = Quaternion.Euler(0, 0, AxMath.SafeAtan2(dif.y, dif.x) * Mathf.Rad2Deg);
        }

        if (decisionTimer > 0f)
        {
            decisionTimer -= Time.deltaTime;
            return;
        }

        decisionTimer = Random.Range(DecisionInterval.x, DecisionInterval.y);

        lookatTarget = null;

        //look at player
        Vector2 distance = transform.position - Camera.main.transform.position;
        if (Mathf.Max(distance.x) > Options.AlertDistance.x || Mathf.Max(distance.y) > Options.AlertDistance.y)
            return;

        Collider2D[] hits = Physics2D.OverlapAreaAll(
            (Vector2)transform.position + Options.AlertDistance,
            (Vector2)transform.position - Options.AlertDistance,
            LayerMask.Walkers);

        foreach (Collider2D hit in hits)
        {
            MonsterCharacter monster = hit.GetComponent<MonsterCharacter>();

            if (monster == null)
                continue;

            if (monster.GetComponent<PlayerControls>() != null)
                lookatTarget = monster;
        }
    }
    
    private void MoveOut(ref MonsterCharacter.InputFrame frame)
    {
        Vector2 dif = TheGrid.WorldPosition(moveTarget) - (Vector2)transform.position;

        if (dif != Vector2.zero)
            dir = dif.normalized;

        frame.up = dir.y;
        frame.right = dir.x;

        if (LookAtMoveDir)
            lookDir = dir;

        frame.WantDirection = Quaternion.Euler(0, 0, AxMath.SafeAtan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg);

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

        if (!AI.RoomPath(thing.gridPos, room, MaxSteps, out Vec2I[] path))
            return;

        if (path.Length < 2)
            return;

        if (path.Length == 2)
            moveTarget = path[1];

        if (path.Length >= 3)
            moveTarget = AI.NaturalStep(path, MaxSteps);

        moveTarget += Vec2I.directions[Random.Range(0, 9)];
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, TheGrid.WorldPosition(moveTarget));
    }*/
}
