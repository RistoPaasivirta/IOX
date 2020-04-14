using UnityEngine;

[RequireComponent(typeof(Behavior), typeof(MonsterCharacter), typeof(ThingController))]
public class BlipMovement : MonoBehaviour
{
    [SerializeField] private float BlipTime = .5f;
    [SerializeField] private float BlipWaitChance = .25f;
    [SerializeField] private float BlipRandomChance = .3f;
    [SerializeField] private int NearSteps = 10; 

    MonsterCharacter character;
    ThingController thing;
    Behavior behavior;
    bool Active = false;
    int LastKnownPlayerRoom = -1;
    float blipTimer;
    public bool Leashed = false;
    public float LeashRange = 32f;
    public Vector3 homePosition;
    public bool HuntPlayer = true;
    public int targetRoom = -1;
    Vec2I dir = Vec2I.zero;

    private void Awake()
    {
        behavior = GetComponent<Behavior>();
        character = GetComponent<MonsterCharacter>();
        thing = GetComponent<ThingController>();

        behavior.OnLoseTarget.AddListener(() => { TakeOver(); });
        behavior.OnGainTarget.AddListener(() => { behavior.Restart(); Active = false; });

        Messaging.Player.PlayerEnterRoom.AddListener((i) => { LastKnownPlayerRoom = i; }); 
    }

    public void TakeOver()
    {
        if (!Active)
        {
            character.OnInputRequest = BlipMove;
            Active = true;
        }
    }

    private void BlipMove(ref MonsterCharacter.InputFrame frame)
    {
        frame.up = dir.y;
        frame.right = dir.x;
        frame.WantDirection = Quaternion.Euler(0, 0, AxMath.SafeAtan2(dir.y, dir.x) * Mathf.Rad2Deg);

        if (blipTimer > 0)
        {
            blipTimer -= Time.deltaTime;
            return;
        }

        blipTimer = BlipTime;
        dir = Vec2I.zero;

        if (Random.value < BlipWaitChance)
            return;

        if (Leashed)
            if ((homePosition - transform.position).sqrMagnitude >= LeashRange * LeashRange)
            {
                Vector2 distance = transform.position - Camera.main.transform.position;
                if (Mathf.Max(distance.x) > Options.AlertDistance.x || Mathf.Max(distance.y) > Options.AlertDistance.y)
                    Destroy(gameObject);
            }  

        if (Random.value < BlipRandomChance)
        {
            dir = Vec2I.directions[Random.Range(1, Vec2I.directions.Length)];
            return;
        }

        if (HuntPlayer)
            if (LastKnownPlayerRoom != -1)
                targetRoom = LastKnownPlayerRoom;

        if (targetRoom >= 0 && targetRoom < Room.Rooms.Length)
        {
            //simplistic behavior
            //dir = Room.Rooms[targetRoom].parent[thing.gridPos.x, thing.gridPos.y] - thing.gridPos;
            //return;

            //to avoid monsters piling up on single point
            if (Vec2I.Manhattan(Room.Rooms[targetRoom].gridPos, thing.gridPos) < NearSteps)
                return;

            if (!AI.RoomPath(thing.gridPos, Room.Rooms[targetRoom], MaxSteps, out Vec2I[] path))
                return;

            if (path.Length < 2)
                return;

            if (path.Length == 2)
                dir = path[1] - thing.gridPos;

            if (path.Length >= 3)
                dir = AI.NaturalStep(path, MaxSteps) - thing.gridPos;
        }
    }

    public int MaxSteps = 6;
}
