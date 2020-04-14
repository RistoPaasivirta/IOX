using System;
using System.Collections.Generic;

[Serializable]
public class Breath
{
    public Vec2I gridPos;
    public Vec2I parent;
    public int travelCost;
    public int steps;

    public Breath(Vec2I GridPos, Vec2I Parent, int TravelCost, int Steps)
    {
        gridPos = GridPos;
        parent = Parent;
        travelCost = TravelCost;
        steps = Steps;
    }
}

[Serializable]
public class BreathArea
{
    public Vec2I startPos;
    public int maxDistance { get; private set; }
    public int size { get; private set;}
    public bool[,] exist;
    public Vec2I[,] parent;
    public int[,] travelCost;
    public int[,] steps;

    public BreathArea(int MaxDistance)
    {
        maxDistance = MaxDistance;
        size = MaxDistance * 2 + 1;

        exist = new bool[size,size];
        parent = new Vec2I[size,size];
        travelCost = new int[size,size];
        steps = new int[size,size];
    }

    public bool Valid(Vec2I relativePos)
    {
        return (relativePos.AllHigherOrEqual(Vec2I.zero) && relativePos.AllLower(size));
    }

    public bool Valid(int x, int y)
    {
        return (x >= 0 && y >= 0 && x < size && y < size);
    }

    public Breath GetBreath(Vec2I position) { return GetBreath(position.x, position.y); }
    public Breath GetBreath(int posX, int posY)
    {
        int gridX = posX - startPos.x + maxDistance;
        int gridY = posY - startPos.y + maxDistance;

        if (!Valid(gridX, gridY))
            return null;

        if (!exist[gridX, gridY])
            return null;

        return new Breath(
            new Vec2I(posX, posY), 
            parent[gridX, gridY],
            travelCost[gridX, gridY],
            steps[gridX, gridY]);
    }

    public List<Breath> GetNearbyBreaths(Vec2I pos, int extend)
    {
        List<Breath> list = new List<Breath>();

        if (extend == 0)
        {
            Breath b = GetBreath(pos);
            if (b != null)
                list.Add(b);

            return list;
        }

        foreach(Vec2I n in Vec2I.Neighbors(pos + new Vec2I(maxDistance, maxDistance)))
            if (Valid(n))
                if (exist[n.x, n.y])
                    list.Add(new Breath(n, parent[n.x, n.y], travelCost[n.x, n.y], steps[n.x, n.y]));

        return list;
    }

    public SingleLinkedList<Breath> BreathList
    {
        get
        {
            SingleLinkedList<Breath> list = new SingleLinkedList<Breath>();

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    if (exist[x, y])
                        list.InsertFront(
                            new Breath(
                                new Vec2I(
                                    x - maxDistance + startPos.x,
                                    y - maxDistance + startPos.y),
                                    parent[x, y],
                                    travelCost[x, y],
                                    steps[x, y]));

            return list;
        }
    }

    public void Invalidate()
    {
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                exist[x, y] = false;
    }
}