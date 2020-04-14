using System.Collections.Generic;
using UnityEngine;

public static class Heatmap
{
    public static HeatmapNode[,] Nodes { get; private set; }
    public static List<HeatmapNode> AllNodes { get; private set; } = new List<HeatmapNode>();

    public static HeatmapNode GetNode(Vec2I gridPos)
    { 
        if (!TheGrid.Valid(gridPos))
            return null;

        return Nodes[gridPos.x,gridPos.y];
    }

    public static HeatmapNode GetNearbyNode(Vec2I gridPos)
    { 
        foreach(Vec2I near in Vec2I.Neighbors(gridPos))
        {
            if (!TheGrid.Valid(near))
                continue;

            if (Nodes[near.x, near.y] != null)
                return Nodes[near.x, near.y];
        }

        return null;
    }

    static readonly Vector2 GroundCheckSize = new Vector2(.1f, .1f);
    static readonly Vector2 WallCheckSize = new Vector2(1f, 1f);

    public static void Recalculate()
    {
        Nodes = new HeatmapNode[TheGrid.size.x + 1, TheGrid.size.y + 1];
        AllNodes.Clear();

        //create nodes
        for (int x = 0; x <= TheGrid.size.x; x++)
            for (int y = 0; y <= TheGrid.size.y; y++)
            {
                //Collider2D groundCollider = Physics2D.OverlapPoint(worldPos, LayerMask.Ground); //misses between quads
                Collider2D groundCollider = Physics2D.OverlapBox(TheGrid.WorldPosition(x, y), GroundCheckSize, 0, LayerMask.Ground);
                if (groundCollider != null)
                {
                    HeatmapNode node = new HeatmapNode(x, y);
                    Nodes[x, y] = node;
                    AllNodes.Add(node);
                }
            }

        // evaluate neighbors \\
        List<HeatmapNode> hasWalls = new List<HeatmapNode>();
        List<HeatmapNode> noWalls = new List<HeatmapNode>();
        List<HeatmapNode> wideOpen = new List<HeatmapNode>();

        //walls
        foreach (HeatmapNode node in AllNodes)
        { 
            Collider2D wall = Physics2D.OverlapBox(TheGrid.WorldPosition(node.gridPos), WallCheckSize, 0, LayerMask.WallsAndShootables);
            if (wall != null)
                node.HasWall = true;

            if (node.HasWall)
                hasWalls.Add(node);
            else
                noWalls.Add(node);

            foreach (Vec2I neighbor in Vec2I.Neighbors(node.gridPos))
            {
                HeatmapNode target = GetNode(neighbor);
                if (target != null)
                    node.neighbors.Add(target);
            }
        }

        //wideopen
        foreach (HeatmapNode node in noWalls)
        {
            if (node.neighbors.Count < 8)
                continue;

            foreach (HeatmapNode neighbor in node.neighbors)
                if (neighbor.HasWall)
                    goto next;

            node.WideOpen = true;
            wideOpen.Add(node);

            //priority sorting (send wideopen nodes in front of the neighbor list)
            foreach (HeatmapNode neighbor in node.neighbors)
                if (neighbor.neighbors.Remove(node))
                    neighbor.neighbors.Insert(0, node);
        next:;
        }

        //superlane
        foreach (HeatmapNode node in wideOpen)
        {
            foreach (HeatmapNode neighbor in node.neighbors)
                if (!neighbor.WideOpen)
                    goto next;

            node.SuperLane = true;

            //priority sorting (send superlane nodes in front of the neighbor list)
            foreach (HeatmapNode neighbor in node.neighbors)
                if (neighbor.neighbors.Remove(node))
                    neighbor.neighbors.Insert(0, node);
        next:;
        }

        //walls priority sorting (send nodes with walls to back of the neighbor list)
        foreach (HeatmapNode node in hasWalls)
            foreach (Vec2I neighbor in Vec2I.Neighbors(node.gridPos))
            {
                HeatmapNode target = GetNode(neighbor);
                if (target != null)
                {
                    if (target.HasWall)
                        target.neighbors.Remove(node);
                    else
                    {
                        if (target.neighbors.Remove(node))
                            target.neighbors.Insert(target.neighbors.Count, node);
                    }
                }
            }
    }
}

public class HeatmapNode
{
    public Vec2I gridPos;
    public Vector3 position { get { return TheGrid.WorldPosition(gridPos); } }
    public List<HeatmapNode> neighbors = new List<HeatmapNode>();

    public bool HasWall;
    public bool WideOpen;
    public bool SuperLane;

    public HeatmapNode(Vec2I GridPos)
    {
        gridPos = GridPos;
    }

    public HeatmapNode(int x, int y)
    {
        gridPos = new Vec2I(x, y);
    }
}