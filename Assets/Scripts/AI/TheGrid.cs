using UnityEngine;

public static class TheGrid
{
    public static Vec2I min;
    public static Vec2I max;
    public static Vec2I size;

    public static bool Valid(Vec2I gridPos)
    {
        return gridPos.AllHigherOrEqual(Vec2I.zero) && gridPos.AllLower(size);
    }

    public static Vec2I GridPosition(Vector3 worldPos)
    {
        return Vec2I.Round(worldPos) - min;
    }

    public static Vector2 WorldPosition(int x, int y)
    {
        return WorldPosition(new Vec2I(x, y));
    }

    public static Vector2 WorldPosition(Vec2I gridPos)
    {
        return (Vector2)(gridPos + min);
    }
}
