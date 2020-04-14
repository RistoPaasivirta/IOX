[System.Serializable]
public struct CraftingCost
{
    public int u;
    public int a;
    public int w;
    public int g;

    public CraftingCost(int utility, int armor, int weapon, int gold)
    {
        u = utility;
        a = armor;
        w = weapon;
        g = gold;
    }

    public static CraftingCost none = new CraftingCost(0, 0, 0, 0);

    public static bool operator ==(CraftingCost a, CraftingCost b)
    {
        return a.u == b.u && a.a == b.a && a.w == b.w && a.g == b.g;
    }

    public static bool operator !=(CraftingCost a, CraftingCost b)
    {
        return !(a == b);
    }

    public static bool operator >(CraftingCost a, CraftingCost b)
    {
        return a.u > b.u && a.a > b.a && a.w > b.w && a.g > b.g;
    }

    public static bool operator <(CraftingCost a, CraftingCost b)
    {
        return a.u < b.u || a.a < b.a || a.w < b.w || a.g < b.g;
    }

    public static CraftingCost operator -(CraftingCost a, CraftingCost b)
    {
        return new CraftingCost(a.u - b.u, a.a - b.a, a.w - b.w, a.g - b.g);
    }

    public static CraftingCost operator +(CraftingCost a, CraftingCost b)
    {
        return new CraftingCost(a.u + b.u, a.a + b.a, a.w + b.w, a.g + b.g);
    }

    public override string ToString()
    {
        return u.ToString() + ", " + a.ToString() + ", " + w.ToString() + ", " + g.ToString();
    }

    public override bool Equals(object obj)
    {
        return obj is CraftingCost && this == (CraftingCost)obj;
    }

    public override int GetHashCode()
    {
        return u.GetHashCode() ^ a.GetHashCode() ^ w.GetHashCode() ^ g.GetHashCode();
    }
}
