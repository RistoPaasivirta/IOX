using UnityEngine;

[RequireComponent(typeof(MonsterCharacter), typeof(ThingController))]
public class PlayerBotMove : MonoBehaviour
{
    MonsterCharacter character;
    ThingController thing;

    private void Awake()
    {
        character = GetComponent<MonsterCharacter>();
        thing = GetComponent<ThingController>();
        character.OnInputRequest = FillInputFrame;
    }

    private int destinationRoom = -1;
    float timer = 0f;
    [SerializeField] private float DecisionTime = .35f;

    private void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;
    }

    Vec2I dir = Vec2I.zero;

    private void FillInputFrame(ref MonsterCharacter.InputFrame frame)
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            destinationRoom = Random.Range(0, Room.Rooms.Length);
            Debug.Log("Moving towards room \"" + Room.Rooms[destinationRoom].gameObject.name + "\"");
        }

        /*int currentRoom = Room.GetRoomIndex(character.thingController.gridPos);

        if (currentRoom == destinationRoom)
            return;*/

        frame.up = dir.y;
        frame.right = dir.x;
        frame.WantDirection = Quaternion.Euler(0, 0, AxMath.SafeAtan2(dir.y, dir.x) * Mathf.Rad2Deg);

        if (timer > 0f)
            return;

        timer = DecisionTime;
        dir = Vec2I.zero;

        /*if (currentRoom == -1 || destinationRoom == -1)
            return;
        int nextRoom = Room.Rooms[currentRoom].ClosestNeighborTowards[destinationRoom];
        if (nextRoom == -1)
            return;

        Vec2I p = Vec2I.illegalMin;

        Vec2I p = Room.Parents[nextRoom][thing.gridPos.x, thing.gridPos.y];

        HeatmapNode h = Heatmap.GetNode(p);
        if (h != null)
        {
            int closest = Room.Distance[nextRoom][thing.gridPos.x, thing.gridPos.y];

            foreach (HeatmapNode n in h.neighbors)
            {
                int distance = Room.Distance[nextRoom][n.gridPos.x, n.gridPos.y];
                if (distance < closest)
                {
                    p = n.gridPos;
                    closest = distance;
                }
            }
        }

        if (p != Vec2I.illegalMin)
            dir = p - thing.gridPos;*/
    }
}
