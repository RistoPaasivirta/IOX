using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to handle evaluation of the map, pathfinding and breath casting
/// </summary>
public static class AI
{
    public static bool CanSeeGridRay(Vec2I from, Vec2I to)
    {
        foreach (Vec2I step in AxMath.CartesianLine(from, to))
        {
            HeatmapNode node = Heatmap.GetNode(step);
            if (node == null) return false;
            if (node.HasWall) return false;
        }

        return true;
    }

    /// <summary>
    /// Pathfinding / breathcasting node step
    /// </summary>
    public class PathStep : IBinaryHeapItem<PathStep>
    {
        public HeatmapNode node;

        public int fullCost;
        public int travelCost;
        public int heuristic;
        public int HeapIndex { get; set; }
        public int CompareTo(PathStep target)
        {
            int compare = fullCost.CompareTo(target.fullCost);

            if (compare == 0)
                compare = heuristic.CompareTo(target.heuristic);

            //need to be flipped since binaryheap stores highest number first
            return -compare;
        }

        public PathStep(HeatmapNode Node, int TravelCost, int Heuristic)
        {
            node = Node;
            travelCost = TravelCost;
            heuristic = Heuristic;
            fullCost = travelCost + heuristic;
        }

        public PathStep(HeatmapNode Node, int Heuristic)
        {
            node = Node;
            heuristic = Heuristic;
            fullCost = heuristic;
        }
    }

    public static int Heuristic(Vec2I from, Vec2I to)
    {
        return Vec2I.Manhattan(from, to) * 40;
    }

    public static int TravelCostBreath(HeatmapNode from, HeatmapNode to)
    {
        if (to.SuperLane)
            return 10;
        else if (to.WideOpen)
            return 20;
        else if (to.HasWall)
            return 100;

        return 40;
    }

    public static int TravelCostAstar(HeatmapNode from, HeatmapNode to)
    {
        if (to.SuperLane)
            return 10;
        else if (to.WideOpen)
            return 20;
        else if (to.HasWall)
            return 1000;

        return 100;
    }

    public static bool CanTravel(HeatmapNode from, HeatmapNode to)
    {
        return true;
    }

    public static void FillBreath(HeatmapNode startNode, ref BreathArea breath)
    {
        int maxDistance = breath.maxDistance;
        int arraySize = breath.size;

        //start sanity check
        if (startNode == null) return;
        if (startNode.neighbors.Count == 0) return;

        breath.startPos = startNode.gridPos;
        breath.Invalidate();

        //init arrays
        bool[,] closedCheck = new bool[arraySize, arraySize];
        bool[,] openCheck = new bool[arraySize, arraySize];
        PathStep[,] openArray = new PathStep[arraySize, arraySize];

        //set start point
        BinaryHeap<PathStep> openList = new BinaryHeap<PathStep>(arraySize * arraySize);
        openList.Add(new PathStep(startNode, 0));
        openCheck[maxDistance, maxDistance] = true;

        //add start breath to area
        breath.exist[maxDistance, maxDistance] = true;
        breath.steps[maxDistance, maxDistance] = 0;
        breath.travelCost[maxDistance, maxDistance] = 0;
        breath.parent[maxDistance, maxDistance] = breath.startPos;

        while (openList.ItemCount > 0)
        {
            //get top of heap
            PathStep current = openList.RemoveFirst();
            int cx = current.node.gridPos.x - breath.startPos.x + maxDistance;
            int cy = current.node.gridPos.y - breath.startPos.y + maxDistance;
            int currentCost = breath.travelCost[cx, cy];
            int currentSteps = breath.steps[cx, cy];
            closedCheck[cx, cy] = true;

            if (currentSteps >= maxDistance)
                continue;

            //shuffle the neighbor nodes to give some noise 
            List<HeatmapNode> shuffled = new List<HeatmapNode>();
            foreach (HeatmapNode neighbor in current.node.neighbors)
                shuffled.Insert(Random.Range(0, shuffled.Count), neighbor);

            foreach (HeatmapNode neighbor in shuffled)
            {
                //calculate array position
                int nx = neighbor.gridPos.x - breath.startPos.x + maxDistance;
                int ny = neighbor.gridPos.y - breath.startPos.y + maxDistance;

                //cull disallowed
                if (Vec2I.Max(neighbor.gridPos, breath.startPos) > maxDistance) continue;
                if (closedCheck[nx, ny]) continue;
                if (openCheck[nx, ny]) continue;
                if (!CanTravel(current.node, neighbor)) continue;

                //calculate cost
                int travelCost = current.travelCost + TravelCostBreath(current.node, neighbor);

                //priority sorted by heap
                PathStep step = new PathStep(neighbor, travelCost, 0);
                openList.Add(step);
                openArray[nx, ny] = step;
                openCheck[nx, ny] = true;

                //add breath to area
                breath.exist[nx, ny] = true;
                breath.parent[nx, ny] = current.node.gridPos;
                breath.travelCost[nx, ny] = travelCost;
                breath.steps[nx, ny] = currentSteps + 1;
            }
        }
    }

    public static void RoomBreath(ref Room room)
    {
        HeatmapNode startNode = Heatmap.GetNode(room.gridPos);

        //start sanity check
        if (startNode == null) return;
        if (startNode.neighbors.Count == 0) return;

        //init arrays
        bool[,] closedCheck = new bool[TheGrid.size.x, TheGrid.size.y];
        bool[,] openCheck = new bool[TheGrid.size.x, TheGrid.size.y];
        PathStep[,] openArray = new PathStep[TheGrid.size.x, TheGrid.size.y];

        //set start point
        BinaryHeap<PathStep> openList = new BinaryHeap<PathStep>(TheGrid.size.x * TheGrid.size.y);
        openList.Add(new PathStep(startNode, 0));
        openCheck[room.gridPos.x, room.gridPos.y] = true;

        //add start breath to room
        room.exist[room.gridPos.x, room.gridPos.y] = true;
        room.steps[room.gridPos.x, room.gridPos.y] = 0;
        room.travelCost[room.gridPos.x, room.gridPos.y] = 0;
        room.parent[room.gridPos.x, room.gridPos.y] = room.gridPos;

        while (openList.ItemCount > 0)
        {
            //get top of heap
            PathStep current = openList.RemoveFirst();
            int cx = current.node.gridPos.x;
            int cy = current.node.gridPos.y;
            int currentCost = room.travelCost[cx, cy];
            int currentSteps = room.steps[cx, cy];
            closedCheck[cx, cy] = true;

            //shuffle the neighbor nodes to give some noise 
            List<HeatmapNode> shuffled = new List<HeatmapNode>();
            foreach (HeatmapNode neighbor in current.node.neighbors)
                shuffled.Insert(Random.Range(0, shuffled.Count), neighbor);

            foreach (HeatmapNode neighbor in shuffled)
            {
                //calculate array position
                int nx = neighbor.gridPos.x;
                int ny = neighbor.gridPos.y;

                //cull disallowed
                if (closedCheck[nx, ny]) continue;
                if (openCheck[nx, ny]) continue;
                if (!CanTravel(current.node, neighbor)) continue;

                //calculate cost
                int travelCost = current.travelCost + TravelCostBreath(current.node, neighbor);

                //priority sorted by heap
                PathStep step = new PathStep(neighbor, travelCost, 0);
                openList.Add(step);
                openArray[nx, ny] = step;
                openCheck[nx, ny] = true;

                //add breath to room
                room.exist[nx, ny] = true;
                room.parent[nx, ny] = current.node.gridPos;
                room.travelCost[nx, ny] = travelCost;
                room.steps[nx, ny] = currentSteps + 1;
            }
        }
    }

    public static bool GetPath(Vec2I StartPos, Vec2I EndPos, int maxDistance, out Vec2I[] path)
    {
        if (StartPos == EndPos)
        {
            path = new Vec2I[1];
            path[0] = EndPos;
            return true;
        }

        path = null;

        if (Vec2I.Max(StartPos, EndPos) > maxDistance) return false;
        if (Heatmap.GetNode(EndPos) == null) return false;

        HeatmapNode startNode = Heatmap.GetNode(StartPos);
        if (startNode == null) return false;

        //init arrays
        int arraySize = maxDistance * 2 + 1;
        bool[,] closedCheck = new bool[arraySize, arraySize];
        bool[,] openCheck = new bool[arraySize, arraySize];
        Vec2I[,] parent = new Vec2I[arraySize, arraySize];
        PathStep[,] openArray = new PathStep[arraySize, arraySize];

        //set start point
        BinaryHeap<PathStep> openList = new BinaryHeap<PathStep>(arraySize * arraySize);
        openList.Add(new PathStep(startNode, Heuristic(StartPos, EndPos)));
        openCheck[maxDistance, maxDistance] = true;
        parent[maxDistance, maxDistance] = StartPos;

        bool found = false;
        while (openList.ItemCount > 0)
        {
            //get top of heap
            PathStep current = openList.RemoveFirst();
            int cx = current.node.gridPos.x - StartPos.x + maxDistance;
            int cy = current.node.gridPos.y - StartPos.y + maxDistance;
            closedCheck[cx, cy] = true;

            foreach (HeatmapNode neighbor in current.node.neighbors)
            {
                //calculate array position
                int nx = neighbor.gridPos.x - StartPos.x + maxDistance;
                int ny = neighbor.gridPos.y - StartPos.y + maxDistance;

                //cull disallowed
                if (Vec2I.Max(neighbor.gridPos, StartPos) > maxDistance) continue;
                if (closedCheck[nx, ny]) continue;
                if (!CanTravel(current.node, neighbor)) continue;

                //found target
                if (neighbor.gridPos == EndPos)
                {
                    parent[nx, ny] = current.node.gridPos;
                    found = true;
                    goto finalize;
                }

                //calculate cost
                int travelCost = current.travelCost + TravelCostAstar(current.node, neighbor);
                int heuristic = Heuristic(neighbor.gridPos, EndPos);
                int fullCost = travelCost + heuristic;

                //check if we can update parent to better 
                if (openCheck[nx, ny])
                    if (openArray[nx, ny].fullCost > fullCost)
                    {
                        openArray[nx, ny].travelCost = travelCost;
                        openArray[nx, ny].heuristic = heuristic;
                        openArray[nx, ny].fullCost = fullCost;
                        parent[nx, ny] = current.node.gridPos;
                        openList.UpdateItem(openArray[nx, ny]);
                        continue;
                    }
                    else
                        continue;

                //priority sorted by heap
                PathStep step = new PathStep(neighbor, travelCost, heuristic);
                openList.Add(step);
                openArray[nx, ny] = step;
                openCheck[nx, ny] = true;
                parent[nx, ny] = current.node.gridPos;
            }
        }

    finalize:
        if (found)
        {
            SingleLinkedList<Vec2I> list = new SingleLinkedList<Vec2I>();

            Vec2I current = EndPos;
            while (current != StartPos)
            {
                list.InsertFront(current);
                current = parent[current.x - StartPos.x + maxDistance, current.y - StartPos.y + maxDistance];
            }
            list.InsertFront(current); //adds the starting point to the path
            path = list.ToArray();
            return true;
        }

        return false;
    }

    public static bool RoomPath(Vec2I StartPos, Room room, int MaxSteps, out Vec2I[] path)
    {
        path = new Vec2I[0];

        if (!TheGrid.Valid(StartPos)) return false;
        if (!room.exist[StartPos.x, StartPos.y]) return false;

        SingleLinkedList<Vec2I> steps = new SingleLinkedList<Vec2I>();

        Vec2I current = new Vec2I(StartPos.x, StartPos.y);

        if (MaxSteps < 1)
        {
            steps.InsertBack(current);
            steps.InsertBack(room.parent[current.x, current.y]);
            path = steps.ToArray();
            return true;
        }

        for (int i = 0; i < MaxSteps; i++)
        {
            steps.InsertBack(current);
            current = room.parent[current.x, current.y];

            if (room.parent[current.x, current.y] == current)
            {
                steps.InsertBack(current);
                break;
            }
        }

        path = steps.ToArray();
        return true;
    }

    public static void NaturalizePath(ref Vec2I[] path, int maxSteps)
    {
        if (path.Length < 3) return;
        if (maxSteps < 2) return;

        SingleLinkedList<Vec2I> naturalized = new SingleLinkedList<Vec2I>();
        naturalized.InsertBack(path[0]);

        int s = 0;
        int e = 2;

        Vec2I[] lastLine = new Vec2I[0];

        while (e < path.Length)
        {
            int steps = 0;

        again:
            Vec2I[] line = AxMath.CartesianLine(path[s], path[e]);

            for (int i = 0; i < line.Length; i++)
            {
                HeatmapNode n = Heatmap.GetNode(line[i]);
                if (n == null) goto failed;
                if (!n.WideOpen) goto failed;
            }

            if (steps <= maxSteps)
            {
                if (e == path.Length - 1)
                {
                    naturalized.InsertBack(path[e]);
                    path = naturalized.ToArray();
                    return;
                }

                lastLine = line;
                steps++;
                e++;

                goto again;
            }

        failed:
            if (e > s + 2)
            {
                naturalized.InsertBack(lastLine[lastLine.Length - 1]);

                s = e;
                e = s + 2;
                continue;
            }

            naturalized.InsertBack(path[++s]);
            e = s + 2;
        }

        while (s < path.Length)
            naturalized.InsertBack(path[s++]);

        path = naturalized.ToArray();
    }

    public static Vec2I NaturalStep(Vec2I[] path, int maxSteps)
    {
        if (path.Length == 0) return Vec2I.zero;
        if (path.Length < 3) return path[1];
        if (maxSteps < 2) return path[1];

        Vec2I[] lastLine = new Vec2I[0];

        int steps = 0;
        int e = 2;

    again:
        Vec2I[] line = AxMath.CartesianLine(path[0], path[e]);

        for (int i = 0; i < line.Length; i++)
        {
            HeatmapNode n = Heatmap.GetNode(line[i]);
            if (n == null) goto failed;
            if (!n.WideOpen) goto failed;
        }

        if (steps <= maxSteps)
        {
            if (e == path.Length - 1)
                return path[e];

            lastLine = line;
            steps++;
            e++;

            goto again;
        }

    failed:
        if (e > 2)
            return lastLine[lastLine.Length - 1];

        return path[1];
    }
}