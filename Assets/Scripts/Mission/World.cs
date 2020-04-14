using UnityEngine;
using UnityEngine.Events;

public class World : MonoBehaviour
{
    [SerializeField] private UnityEvent AfterInit = new UnityEvent();

    private void Awake()
    {
        Messaging.System.LevelLoaded.AddListener((i) => 
        {
            Vector2 size = transform.localScale;
            Vector2 offset = transform.position;

            TheGrid.min = Vec2I.Floor(offset - size / 2);
            TheGrid.max = Vec2I.Ceil(offset + size / 2);
            TheGrid.size = TheGrid.max - TheGrid.min;

            Heatmap.Recalculate();
            Room.ReconstructNetwork();

            AfterInit.Invoke();

            //report room information
            /*foreach (Room r in Room.Rooms)
            {
                string d = "Room \"" + r.gameObject.name + "\" has " + r.Neighbors.Count + " neigbors: \n";
                r.Neighbors.Perform((n) => { d += "\"" + Room.Rooms[n.Data].gameObject.name + "\" "; });
                Debug.Log(d);

                Debug.Log("ROOM \"" + r.gameObject.name + "\" neighbors:");
                r.Neighbors.Perform((n) => 
                {
                    Debug.Log("neighbor \"" + Room.Rooms[n.Data].gameObject.name + "\" distance: "+ r.DistanceToNeighbor[n.Data]);
                });
            }*/
        });
    }

    private void Start()
    {
        LevelLoader.InitLevel();
    }

    //visualize current room and tell which room you are in
    /*private void Start()
    {
        int lastRoom = -1;
        PlayerCharacter.Local.thingController.GridPosChanged.AddListener((v) =>
        {
            int r = Room.GetRoomIndex(v);
            if (r == -1)
                return;
            
            if (r != lastRoom)
            {
                Debug.Log("Current room: " + Room.Rooms[r].gameObject.name);
                lastRoom = r;

                DevTools.ClearVisualizations();
                for (int x = 0; x <= TheGrid.size.x; x++)
                    for (int y = 0; y <= TheGrid.size.y; y++)
                        if (Room.Parents[r][x, y] != Vec2I.illegalMin)
                        {
                            Color c = Color.blue;
                            HeatmapNode node = Heatmap.Nodes[x, y];
                            if (node != null)
                            {
                                if (node.WideOpen)
                                    c = Color.green;
                                if (node.SuperLane)
                                    c = Color.cyan;
                            }

                            DevTools.VisualizeGridLine(new Vec2I(x, y), Room.Parents[r][x, y], c);
                        }
            }
        });
    }*/

    /*private void Update()
    {
        //request a path from a random room to another
        if (Input.GetKeyDown(KeyCode.R))
        {
            int from = Random.Range(0, Room.Rooms.Length);
            int to = Random.Range(0, Room.Rooms.Length);
            string p = "Path from room " + Room.Rooms[from].gameObject.name + " to " + Room.Rooms[to].gameObject.name + "\n";
            foreach (int s in Room.PathToRoom(from, to))
                p += "=> [\"" + Room.Rooms[s].gameObject.name + "\"] ";
            Debug.Log(p);
        }
    }*/
}
