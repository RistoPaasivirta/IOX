using UnityEngine;
using UnityEngine.Events;

public class ThingController : MonoBehaviour
{
    public Vec2I gridPos { get; private set; } = Vec2I.illegalMin;
    //public bool InGrid { get; private set; } = false;

    //when something wants to target this thing, modify the targeting position by this amount
    [SerializeField] private Vector3 targetOffset = Vector3.zero;
    public Vector3 TargetOffset { get => targetOffset; }

    public UnityEvent<Vec2I> GridPosChanged = new UnityEventVec2I();
    public UnityEvent OnMouseEnter = new UnityEvent();
    public UnityEvent OnMouseLeave = new UnityEvent();

    /*public enum ThingTypes
    {
        Decor,
        Neutral,
        Item,
        Monster
    }

    [SerializeField] private ThingTypes thingType = ThingTypes.Decor;
    public ThingTypes ThingType { get => thingType; }*/

    private void Start()
    {
        gridPos = TheGrid.GridPosition(transform.position);

        //AddToGrid();

        //if (CannotMove)
            //enabled = false;

        GridPosChanged.Invoke(gridPos);
    }

    /*private void OnDestroy()
    {
        RemoveFromGrid();
    }*/

    private void Update()
    {
        Vec2I newPos = TheGrid.GridPosition(transform.position);
        if (newPos != gridPos)
        {
            //RemoveFromGrid();
            gridPos = newPos;
            //AddToGrid();

            GridPosChanged.Invoke(newPos);
        }
    }

    /*public void AddToGrid()
    {
        if (!TheGrid.Valid(gridPos))
        {
            Debug.LogError("ThingController \"" + gameObject.name + "\": AddToGrid: out of grid! (" + gridPos + ")");
            return;
        }

        HeatmapNode node = Heatmap.Nodes[gridPos.x, gridPos.y];
        if (node == null)
            return;

        switch (ThingType)
        {
            case ThingTypes.Decor:
                node.decorThings.InsertFront(this);
                break;

            case ThingTypes.Neutral:
                node.neutralThings.InsertFront(this);
                break;

            case ThingTypes.Item:
                node.itemThings.InsertFront(this);
                break;

            case ThingTypes.Monster:
                node.monsterThings.InsertFront(this);
                break;
        }

        InGrid = true;
    }

    public void RemoveFromGrid()
    {
        if (!InGrid)
            return;

        if (!TheGrid.Valid(gridPos))
        {
            Debug.LogError("ThingController \"" + gameObject.name + "\": RemoveFromGrid: out of grid! (" + gridPos + ")");
            return;
        }

        HeatmapNode node = Heatmap.Nodes[gridPos.x, gridPos.y];
        if (node == null)
            return;

        switch (ThingType)
        {
            case ThingTypes.Decor:
                node.decorThings.DestroyContainingNode(this);
                break;

            case ThingTypes.Neutral:
                node.neutralThings.DestroyContainingNode(this);
                break;

            case ThingTypes.Item:
                node.itemThings.DestroyContainingNode(this);
                break;

            case ThingTypes.Monster:
                node.monsterThings.DestroyContainingNode(this);
                break;
        }

        InGrid = false;
    }*/
}
