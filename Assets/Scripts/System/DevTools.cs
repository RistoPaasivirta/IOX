using System.Collections.Generic;
using UnityEngine;

public class DevTools : MonoBehaviour
{
    public Material DevMaterial;

    static List<GameObject> Visualized = new List<GameObject>();

    private void Awake()
    {
        Messaging.DevTools.VisualizeHeatmap.AddListener(() => 
        {
            
            Vec2I campos = TheGrid.GridPosition(Camera.main.transform.position);

            for (int x = campos.x - (int)Options.ScreenDistance.x; x <= campos.x + (int)Options.ScreenDistance.x; x++)
                for (int y = campos.y - (int)Options.ScreenDistance.y; y <= campos.y + (int)Options.ScreenDistance.y; y++)
                {
                    HeatmapNode node = Heatmap.GetNode(new Vec2I(x, y));
                    if (node != null)
                    {
                        VisualizeNode(Heatmap.Nodes[x, y]);
                        VisualizeConnections(Heatmap.Nodes[x, y]);
                    }
                }
        });

        Messaging.DevTools.ClearVisualizations.AddListener(() => 
        {
            foreach (GameObject visualized in Visualized)
                Destroy(visualized);

            Visualized.Clear();
        });
    }

    //public GameObject accessory;

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
            if (accessory != null)
            {
                Messaging.GUI.ScreenMessage.Invoke("Item added to stash", Color.white);
                Stash.AddItemToFirstEmpty(accessory.GetComponent<InventoryGUIObject>(), out int _);
            }*/

        /*if (Input.GetKeyDown(KeyCode.F11))
        {
            Messaging.DevTools.ClearVisualizations.Invoke();
            Messaging.DevTools.VisualizeHeatmap.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            Messaging.DevTools.ClearVisualizations.Invoke();
        }
    }*/

    void VisualizeNode(HeatmapNode node)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Quad);
        cube.transform.localScale = new Vector3(1f, 1f, 1f);
        cube.transform.position = node.position - Vector3.forward;
        MeshRenderer mr = cube.GetComponent<MeshRenderer>();
        mr.material = DevMaterial;
        GameObject.Destroy(cube.GetComponent<MeshCollider>());

        MaterialPropertyBlock properties = new MaterialPropertyBlock();

        Color color;
        if (node.SuperLane) color = new Color(0, 1, 1, .15f);
        else if (node.WideOpen) color = new Color(0, 1, 0, .15f);
        else if (node.HasWall) color = new Color(1, 0, 0, .15f);
        else color = new Color(1, 1, 0, .15f);

        properties.SetColor("_Color", color);

        mr.SetPropertyBlock(properties);

        Visualized.Add(cube);
    }

    void VisualizeConnections(HeatmapNode node)
    {
        foreach (HeatmapNode neighbor in node.neighbors)
        {
            //lines
            GameObject line = new GameObject();
            LineRenderer lr = line.AddComponent<LineRenderer>();
            lr.widthMultiplier = .1f;
            lr.material = DevMaterial;
            {
                MaterialPropertyBlock properties = new MaterialPropertyBlock();
                properties.SetColor("_Color", neighbor.neighbors.Count > 7 ? Color.green : Color.yellow);
                lr.SetPropertyBlock(properties);
            }
            line.transform.position = node.position;
            Vector3[] positions = new Vector3[2]
            {
                node.position - Vector3.forward,
                neighbor.position - Vector3.forward
            };
            lr.SetPositions(positions);
            Visualized.Add(line);
        }
    }


    /*
    public static Material DevMaterial;
    public static GameObject DevToolsUI;

    static bool active;
    public static bool Active
    {
        get { return active; }
        set
        {
            active = value;

            if (!active)
                ClearVisualizations();

            Messaging.System.SetTimeScale.Invoke(0f);
            DevToolsUI.SetActive(active);
        }
    }

    static Vec2I lastMouseGrid;

    public enum ToolType
    {
        None,
        HeatmapTestTool,
        BreathTestTool,
        AStarTool
    }

    public static void Update()
    {
        switch (CurrentTool)
        {
            default:
            case ToolType.None:
                break;

            case ToolType.HeatmapTestTool:
                HeatmapTestTool();
                break;

            case ToolType.BreathTestTool:
                BreathTestTool();
                break;

            case ToolType.AStarTool:
                AStarTestTool();
                break;
        }
    }

    public static ToolType CurrentTool = ToolType.BreathTestTool;

    public static void HeatmapTestTool()
    {
        Mouse.UpdateWorldPosition();

        if (Mouse.InWorld)
        {
            if (Mouse.GridPosition != lastMouseGrid)
            {
                ClearVisualizations();

                HeatmapNode currentNode = Heatmap.GetNode(Mouse.GridPosition);
                if (currentNode != null)
                {
                    VisualizeNode(currentNode);
                    VisualizeConnections(currentNode);
                    VisualizeNeighbors(currentNode);
                }

                lastMouseGrid = Mouse.GridPosition;
            }
        }
    }

    public static void BreathTestTool()
    {
    }

    public static void AStarTestTool()
    {
        //OLD AND BROKEN
        MonsterCharacter PlayerCharacter = null;

        if (PlayerCharacter == null)
        {
            ClearVisualizations();
            return;
        }

        Mouse.UpdateWorldPosition();

        if (Mouse.InWorld)
        {
            if (Mouse.GridPosition != lastMouseGrid)
            {
                ClearVisualizations();

                Vec2I a = PlayerCharacter.thingController.gridPos;
                Vec2I b = Mouse.GridPosition;

                Vec2I[] path;

                if (AI.GetPath(a, b, Vec2I.Max(a, b), out path))
                {
                    AI.NaturalizePath(ref path, 10);
                    foreach (Vec2I step in path)
                        VisualizeNode(Heatmap.GetNode(step));
                }

                lastMouseGrid = Mouse.GridPosition;
            }
        }
    }

    public static Color green = new Color(0,1,0,.2f);
    public static Color yellow = new Color(1, 1, 0, .2f);

    public static void VisualizeNode(HeatmapNode node)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Quad);
        cube.transform.localScale = new Vector3(1f, 1f, 1f);
        cube.transform.position = node.position - Vector3.forward;
        MeshRenderer mr = cube.GetComponent<MeshRenderer>();
        mr.material = DevMaterial;
        GameObject.Destroy(cube.GetComponent<MeshCollider>());

        MaterialPropertyBlock properties = new MaterialPropertyBlock();
        
        Color color;
        if (node.SuperLane) color = new Color(0, 1, 1,.15f);
        else if (node.WideOpen) color = new Color(0, 1, 0,.15f);
        else if (node.HasWall) color = new Color(1, 0, 0,.15f);
        else color = new Color(1, 1, 0,.15f);

        properties.SetColor("_Color", color);

        mr.SetPropertyBlock(properties);

        Visualized.Add(cube);
    }

    public static void VisualizeConnections(HeatmapNode node)
    {
        foreach (HeatmapNode neighbor in node.neighbors)
        {
            //lines
            GameObject line = new GameObject();
            LineRenderer lr = line.AddComponent<LineRenderer>();
            lr.widthMultiplier = .1f;
            lr.material = DevMaterial;
            {
                MaterialPropertyBlock properties = new MaterialPropertyBlock();
                properties.SetColor("_Color", neighbor.neighbors.Count > 7 ? Color.green : Color.yellow);
                lr.SetPropertyBlock(properties);
            }
            line.transform.position = node.position;
            Vector3[] positions = new Vector3[2]
            {
                node.position - Vector3.forward,
                neighbor.position - Vector3.forward
            };
            lr.SetPositions(positions);
            Visualized.Add(line);
        }
    }

    public static void VisualizeNeighbors(HeatmapNode node)
    {
        foreach (HeatmapNode neighbor in node.neighbors)
        {
            //lines
            GameObject line = new GameObject();
            LineRenderer lr = line.AddComponent<LineRenderer>();
            lr.widthMultiplier = .1f;
            lr.material = DevMaterial;
            {
                MaterialPropertyBlock properties = new MaterialPropertyBlock();
                properties.SetColor("_Color", neighbor.neighbors.Count > 7 ? Color.green : Color.yellow);
                lr.SetPropertyBlock(properties);
            }
            line.transform.position = node.position;
            Vector3[] positions = new Vector3[2]
            {
                node.position - Vector3.forward,
                neighbor.position - Vector3.forward
            };
            lr.SetPositions(positions);
            Visualized.Add(line);

            //neighbor
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.transform.localScale = new Vector3(.5f, 1f, .5f);
            c.transform.position = neighbor.position;
            MeshRenderer mr = c.GetComponent<MeshRenderer>();
            {
                MaterialPropertyBlock properties = new MaterialPropertyBlock();
                properties.SetColor("_Color", neighbor.neighbors.Count > 7 ? Color.green : Color.yellow);
                mr.SetPropertyBlock(properties);
            }
            mr.material = DevMaterial;
            Visualized.Add(c);
        }
    }

    public static void VisualizeGridLine(Vec2I a, Vec2I b, Color color)
    {
        GameObject line = new GameObject();
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.widthMultiplier = .1f;
        lr.material = DevMaterial;
        {
            MaterialPropertyBlock properties = new MaterialPropertyBlock();
            properties.SetColor("_Color", color);
            lr.SetPropertyBlock(properties);
        }
        line.transform.position = TheGrid.WorldPosition(a);
        Vector3[] positions = new Vector3[2]
        {
            (Vector3)TheGrid.WorldPosition(a) - Vector3.forward,
            (Vector3)TheGrid.WorldPosition(b) - Vector3.forward
        };
        lr.SetPositions(positions);
        Visualized.Add(line);
    }

    public static void VisualizeAll()
    {
        for (int x = 0; x <= TheGrid.size.x; x++)
            for (int y = 0; y <= TheGrid.size.y; y++)
                if (Heatmap.Nodes[x,y] != null)
                {
                    VisualizeNode(Heatmap.Nodes[x,y]);
                    VisualizeConnections(Heatmap.Nodes[x,y]);
                }
    }

    public static List<GameObject> Visualized = new List<GameObject>();

    public static void ClearVisualizations()
    {
        foreach (GameObject visualized in Visualized)
            GameObject.Destroy(visualized);

        Visualized.Clear();
    }*/
}
