using System.Collections.Generic;

public class Lump
{
    public int offset;
    public int length;
    public string lumpName;
    public byte[] data;

    public Lump(int Offset, int Length, string Name)
    {
        offset = Offset;
        length = Length;
        lumpName = Name;
    }

    public Lump(string Name, byte[] Data)
    {
        lumpName = Name;
        data = Data;
    }

    public static List<Lump> Cull(List<Lump> lumps, string[] names)
    {
        List<Lump> delete = new List<Lump>();

        foreach(Lump l in lumps)
            foreach(string s in names)
                if (l.lumpName == s)
                {
                    delete.Add(l);
                    break;
                }

        foreach (Lump d in delete)
            lumps.Remove(d);

        return lumps;
    }
}
