using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Vec2I
{
    public int x;
    public int y;

    public Vec2I(int X, int Y)
    {
        x = X;
        y = Y;
    }

    public Vec2I RotateCW { get => new Vec2I(y, -x); }
    public Vec2I RotateCCW { get => new Vec2I(-y, x); }

    public float sqrMagnitude { get => x * x + y * y; } 
    public float magnitude { get => Mathf.Sqrt(x * x + y * y); } 

    public static float Distance(Vec2I a, Vec2I b) => 
        (a - b).magnitude; 

    public static float SqrDistance(Vec2I a, Vec2I b) => 
        (a - b).sqrMagnitude; 

    public static int Manhattan(Vec2I a, Vec2I b) =>
        Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

    public static int Max(Vec2I a, Vec2I b) =>
        Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));

    public static int Min(Vec2I a, Vec2I b) =>
        Mathf.Min(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));

    public static Vec2I zero { get => new Vec2I(0, 0); }
    public static Vec2I one { get => new Vec2I(1, 1); }
    public static Vec2I north { get => new Vec2I(0, 1); }
    public static Vec2I south { get => new Vec2I(0, -1); }
    public static Vec2I east { get => new Vec2I(1, 0); }
    public static Vec2I west { get => new Vec2I(-1, 0); } 
    public static Vec2I northeast { get => new Vec2I(1, 1); } 
    public static Vec2I southeast { get => new Vec2I(1, -1); } 
    public static Vec2I southwest { get => new Vec2I(-1, -1); } 
    public static Vec2I northwest { get => new Vec2I(-1, 1); } 
    public static Vec2I illegalMin { get => new Vec2I(int.MinValue, int.MinValue); } 
    public static Vec2I illegalMax { get => new Vec2I(int.MaxValue, int.MaxValue); } 

    //   Directions 
    //
    //     8  7  6
    //      \ | /
    //       \|/
    //    1---0---5
    //       /|\
    //      / | \
    //     2  3  4

    public static readonly Vec2I[] directions = new Vec2I[9]
    {
        zero,
        west,
        southwest,
        south,
        southeast,
        east,
        northeast,
        north,
        northwest
    };

    //    Neighbors 
    //
    //     7  6  5
    //      \ | /
    //       \|/
    //    0---X---4
    //       /|\
    //      / | \
    //     1  2  3

    public static IEnumerable<Vec2I> Square(Vec2I position)
    {
        yield return position;
        yield return position + north;
        yield return position + northeast;
        yield return position + east;
        yield return position + southeast;
        yield return position + south;
        yield return position + southwest;
        yield return position + west;
        yield return position + northwest;
    }

    public static IEnumerable<Vec2I> Neighbors(Vec2I position)
    {
        yield return position + north;
        yield return position + northeast;
        yield return position + east;
        yield return position + southeast;
        yield return position + south;
        yield return position + southwest;
        yield return position + west;
        yield return position + northwest;
    }

    public static explicit operator Vector2(Vec2I v) => new Vector2(v.x, v.y);
    public Vector3 ToVector3(float z) => new Vector3(x, y, z);

    public static Vec2I Floor(Vector2 v) => new Vec2I(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
    public static Vec2I Ceil(Vector2 v) =>  new Vec2I(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
    public static Vec2I Round(Vector2 v) => new Vec2I(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));

    public override string ToString() => x.ToString() + ", " + y.ToString();
    public override bool Equals(object obj) => obj is Vec2I && this == (Vec2I)obj;
    public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode();

    public static bool operator ==(Vec2I a, Vec2I b) => a.x == b.x && a.y == b.y;
    public static bool operator !=(Vec2I a, Vec2I b) => !(a == b);
    public static Vec2I operator +(Vec2I a, Vec2I b) => new Vec2I(a.x + b.x, a.y + b.y);
    public static Vec2I operator -(Vec2I a, Vec2I b) => new Vec2I(a.x - b.x, a.y - b.y);
    public static Vec2I operator *(Vec2I a, Vec2I b) => new Vec2I(a.x * b.x, a.y * b.y);
    public static Vec2I operator /(Vec2I a, Vec2I b) => new Vec2I(a.x / b.x, a.y / b.y);
    public static Vec2I operator *(Vec2I a, int s) => new Vec2I(a.x * s, a.y * s);
    public static Vec2I operator /(Vec2I a, int s) => new Vec2I(a.x / s, a.y / s);
    public static Vec2I operator -(Vec2I t) => new Vec2I(-t.x, -t.y);

    public bool AnyLower(Vec2I compareTo) => AnyLower(compareTo.x, compareTo.y); 
    public bool AnyLower(int X, int Y) => x < X || y < Y; 
    public bool AnyLower(int value) => x < value || y < value; 

    public bool AnyHigher(Vec2I compareTo) => AnyHigher(compareTo.x, compareTo.y); 
    public bool AnyHigher(int X, int Y) => x > X || y > Y; 
    public bool AnyHigher(int value) => x > value || y > value; 

    public bool AnyLowerOrEqual(Vec2I compareTo) => AnyLowerOrEqual(compareTo.x, compareTo.y); 
    public bool AnyLowerOrEqual(int X, int Y) => x <= X || y <= Y; 
    public bool AnyLowerOrEqual(int value) => x <= value || y <= value; 

    public bool AnyHigherOrEqual(Vec2I compareTo) => AnyHigherOrEqual(compareTo.x, compareTo.y); 
    public bool AnyHigherOrEqual(int X, int Y) => x >= X || y >= Y; 
    public bool AnyHigherOrEqual(int value) => x >= value || y >= value; 

    public bool AllLower(Vec2I compareTo) => AllLower(compareTo.x, compareTo.y); 
    public bool AllLower(int X, int Y) => x < X && y < Y; 
    public bool AllLower(int value) => x < value && y < value; 

    public bool AllHigher(Vec2I compareTo) => AllHigher(compareTo.x, compareTo.y); 
    public bool AllHigher(int X, int Y) => x > X && y > Y; 
    public bool AllHigher(int value) => x > value && y > value; 

    public bool AllLowerOrEqual(Vec2I compareTo) => AllLowerOrEqual(compareTo.x, compareTo.y); 
    public bool AllLowerOrEqual(int X, int Y) => x <= X && y <= Y; 
    public bool AllLowerOrEqual(int value) => x <= value && y <= value; 

    public bool AllHigherOrEqual(Vec2I compareTo) => AllHigherOrEqual(compareTo.x, compareTo.y);
    public bool AllHigherOrEqual(int X, int Y) => x >= X && y >= Y;
    public bool AllHigherOrEqual(int value) => x >= value && y >= value;

    public static Vec2I SelectMin(Vec2I a, Vec2I b) => new Vec2I(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
    public static Vec2I SelectMax(Vec2I a, Vec2I b) => new Vec2I(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
}