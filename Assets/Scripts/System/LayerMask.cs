public static class LayerMask
{
    public const int Ground = (1 << 10);
    public const int Walls = (1 << 11);
    public const int Shootables = (1 << 12);
    public const int WallsAndShootables = (1 << 11) | (1 << 12);
    public const int Walkers = (1 << 13);
    public const int WalkersAndShootables = (1 << 13) | (1 << 12);
    public const int WallsWalkersAndShootables = (1 << 11) | (1 << 12) | (1 << 13);
    public const int Decor = (1 << 14);
}

public static class Layers
{
    public const int Ground = 10;
    public const int Walls = 11;
    public const int Shootables = 12;
    public const int Walkers = 13;
    public const int Decor = 14;
}