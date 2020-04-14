using UnityEngine;

[System.Serializable]
public struct Vec3I
{
    public int x;
    public int y;
    public int z;

    public Vec3I(int X, int Y, int Z)
    {
        x = X;
        y = Y;
        z = Z;
    }

    public float sqrMagnitude { get { return x * x + y * y + z * z; } }
    public float magnitude { get { return Mathf.Sqrt(x * x + y * y + z * z); } }
    public static float Distance (Vec3I a, Vec3I b) { return (a - b).magnitude; }
    public static float SqrDistance(Vec3I a, Vec3I b) { return (a - b).sqrMagnitude; }

    public static int Manhattan(Vec3I a, Vec3I b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }

    public static int Max(Vec3I a, Vec3I b)
    {
        return Mathf.Max(Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y)), Mathf.Abs(a.z - b.z));
    }

    public static int Min(Vec3I a, Vec3I b)
    {
        return Mathf.Min(Mathf.Min(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y)), Mathf.Abs(a.z - b.z));
    }

    public static Vec3I zero { get { return new Vec3I(0, 0, 0); } }
    public static Vec3I one { get { return new Vec3I(1, 1, 1); } }
    public static Vec3I north { get { return new Vec3I(0, 0, 1); } }
    public static Vec3I south { get { return new Vec3I(0, 0, -1); } }
    public static Vec3I east { get { return new Vec3I(1, 0, 0); } }
    public static Vec3I west { get { return new Vec3I(-1, 0, 0); } }
    public static Vec3I northeast { get { return new Vec3I(1, 0, 1); } }
    public static Vec3I southeast { get { return new Vec3I(1, 0, -1); } }
    public static Vec3I southwest { get { return new Vec3I(-1, 0, -1); } }
    public static Vec3I northwest { get { return new Vec3I(-1, 0, 1); } }
    public static Vec3I up { get { return new Vec3I(0, 1, 0); } }
    public static Vec3I down { get { return new Vec3I(0, 0, -1); } }
    public static Vec3I illegalMin { get { return new Vec3I(int.MinValue, int.MinValue, int.MinValue); } }
    public static Vec3I illegalMax { get { return new Vec3I(int.MaxValue, int.MaxValue, int.MaxValue); } }

    //   Directions 
    //
    //     8  7  6
    //      \ | /
    //       \|/
    //    1---0---5
    //       /|\
    //      / | \
    //     2  3  4

    public static readonly Vec3I[] directions = new Vec3I[9]
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

    public static explicit operator Vector3(Vec3I v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    public static Vec3I Floor(Vector3 v)
    {
        return new Vec3I(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
    }

    public static Vec3I Ceil(Vector3 v)
    {
        return new Vec3I(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));
    }

    public static Vec3I Round(Vector3 v)
    {
        return new Vec3I(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
    }

    public override string ToString()
    {
        return x.ToString() + ", " + y.ToString() + ", " + z.ToString();
    }

    public override bool Equals(object obj)
    {
        return obj is Vec3I && this == (Vec3I)obj;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
    }

    public static bool operator ==(Vec3I a, Vec3I b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }

    public static bool operator !=(Vec3I a, Vec3I b)
    {
        return !(a == b);
    }

    public static Vec3I operator +(Vec3I a, Vec3I b)
    {
        return new Vec3I(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Vec3I operator -(Vec3I a, Vec3I b)
    {
        return new Vec3I(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Vec3I operator *(Vec3I a, Vec3I b)
    {
        return new Vec3I(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vec3I operator /(Vec3I a, Vec3I b)
    {
        return new Vec3I(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    public static Vec3I operator *(Vec3I a, int s)
    {
        return new Vec3I(a.x * s, a.y * s, a.z * s);
    }

    public static Vec3I operator /(Vec3I a, int s)
    {
        return new Vec3I(a.x / s, a.y / s, a.z / s);
    }

    public static Vec3I operator -(Vec3I t)
    {
        return new Vec3I(-t.x, -t.y, -t.z);
    }

    public bool AnyLower(Vec3I compareTo) { return AnyLower(compareTo.x, compareTo.y, compareTo.z); }
    public bool AnyLower(int X, int Y, int Z) { return x < X || y < Y || z < Z; }
    public bool AnyLower(int value) { return x < value || y < value || z < value; }

    public bool AnyHigher(Vec3I compareTo) { return AnyHigher(compareTo.x, compareTo.y, compareTo.z); }
    public bool AnyHigher(int X, int Y, int Z) { return x > X || y > Y || z > Z; }
    public bool AnyHigher(int value) { return x > value || y > value || z > value; }

    public bool AnyLowerOrEqual(Vec3I compareTo) { return AnyLowerOrEqual(compareTo.x, compareTo.y, compareTo.z); }
    public bool AnyLowerOrEqual(int X, int Y, int Z) { return x <= X || y <= Y || z <= Z; }
    public bool AnyLowerOrEqual(int value) { return x <= value || y <= value || z <= value; }

    public bool AnyHigherOrEqual(Vec3I compareTo) { return AnyHigherOrEqual(compareTo.x, compareTo.y, compareTo.z); }
    public bool AnyHigherOrEqual(int X, int Y, int Z) { return x >= X || y >= Y || z >= Z; }
    public bool AnyHigherOrEqual(int value) { return x >= value || y >= value || z >= value; }

    public bool AllLower(Vec3I compareTo) { return AllLower(compareTo.x, compareTo.y, compareTo.z); }
    public bool AllLower(int X, int Y, int Z) { return x < X && y < Y && z < Z; }
    public bool AllLower(int value) { return (x < value && y < value && z < value); }

    public bool AllHigher(Vec3I compareTo) { return AllHigher(compareTo.x, compareTo.y, compareTo.z); }
    public bool AllHigher(int X, int Y, int Z) { return x > X && y > Y && z > Z; }
    public bool AllHigher(int value) { return x > value && y > value && z > value; }

    public bool AllLowerOrEqual(Vec3I compareTo) { return AllLowerOrEqual(compareTo.x, compareTo.y, compareTo.z); }
    public bool AllLowerOrEqual(int X, int Y, int Z) { return x <= X && y <= Y && z <= Z; }
    public bool ALlLowerOrEqual(int value) { return x <= value && y <= value && z <= value; }

    public bool AllHigherOrEqual(Vec3I compareTo) { return AllHigherOrEqual(compareTo.x, compareTo.y, compareTo.z); }
    public bool AllHigherOrEqual(int X, int Y, int Z) { return x >= X && y >= Y && z >= Z; }
    public bool AllHigherOrEqual(int value) { return x >= value && y >= value && z >= value; }

    public static Vec3I SelectMin(Vec3I a, Vec3I b)
    {
        return new Vec3I(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
    }

    public static Vec3I SelectMax(Vec3I a, Vec3I b)
    {
        return new Vec3I(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
    }
}