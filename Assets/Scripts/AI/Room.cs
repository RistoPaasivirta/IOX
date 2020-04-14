using UnityEngine;

public class Room : MonoBehaviour
{
    [HideInInspector]
    public Vec2I gridPos;
    [HideInInspector]
    public Vec2I size;
    [HideInInspector]
    public Vec2I[,] parent;
    [HideInInspector]
    public int[,] travelCost;
    [HideInInspector]
    public int[,] steps;
    [HideInInspector]
    public bool[,] exist;

    private void Awake()
    {
        _rooms.InsertFront(this);
    }

    private void Init()
    {
        gridPos = TheGrid.GridPosition(transform.position);
        size = TheGrid.size; //each room spans the entire level at the moment
        //later on we can optimize this so level designer can restrict the area

        if (!TheGrid.Valid(gridPos))
        {
            Debug.Log("Room \"" + gameObject.name + "\" out of grid! (" + gridPos + ")");
            enabled = false;
            return;
        }

        HeatmapNode h = Heatmap.GetNode(gridPos);
        if (h == null)
        {
            Debug.Log("Room \"" + gameObject.name + "\" does not reside in heatmap! (" + gridPos + ")");
            enabled = false;
            return;
        }

        parent = new Vec2I[size.x, size.y];
        travelCost = new int[size.x, size.y];
        steps = new int[size.x, size.y];
        exist = new bool[size.x, size.y];
    }

    private static SingleLinkedList<Room> _rooms = new SingleLinkedList<Room>();
    public static Room[] Rooms { get; private set; } = new Room[0];
    public static int[,] RoomIndex { get; private set; } = new int[0, 0];

    public static void DestroyNetwork()
    {
        Rooms = new Room[0];
    }

    public static void ReconstructNetwork()
    {
        Rooms = _rooms.ToArray();
        _rooms.Clear();

        for (int r = 0; r < Rooms.Length; r++)
        {
            Rooms[r].Init();
            AI.RoomBreath(ref Rooms[r]);
        }

        RoomIndex = new int[TheGrid.size.x, TheGrid.size.y];

        for (int y = 0; y < TheGrid.size.y; y++)
            for (int x = 0; x < TheGrid.size.x; x++)
            {
                RoomIndex[x, y] = -1;

                int closest = int.MaxValue;
                for (int r = 0; r < Rooms.Length; r++)
                    if (Rooms[r].steps[x, y] < closest)
                    {
                        RoomIndex[x, y] = r;
                        closest = Rooms[r].steps[x, y];
                    }

                if (closest == int.MaxValue)
                    Debug.LogError("Room: ConstructNetwork: closest == int.maxvalue at gridpos (" + x + "," + y + ")");
            }
    }

    public static int GetRoomIndex(Vec2I gridpos)
    {
        if (!TheGrid.Valid(gridpos))
            return -1;

        return RoomIndex[gridpos.x, gridpos.y];
    }
}