using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Stash
{
    const int ExtraEmptySlots = 50;
    public static List<InventoryGUIObject> StashItems = new List<InventoryGUIObject>();
    public static CraftingCost CraftingMaterials = new CraftingCost(0, 0, 0, 0);

    public enum StashMode
    {
        Disabled,
        Loadout,
        Stash,
        Assembler,
        Disassembler,
        Augment
    }

    public static StashMode LeftStashMode = StashMode.Disabled;
    public static StashMode RightStashMode = StashMode.Disabled;

    public static void AddItemToFirstEmpty(InventoryGUIObject item, out int slot)
    {
        for (int i = 0; i < StashItems.Count; i++)
            if (StashItems[i] == null)
            {
                StashItems.RemoveAt(i);
                StashItems.Insert(i, item);
                slot = i;
                return;
            }

        slot = StashItems.Count;
        StashItems.Add(item);
    }

    public static List<Lump> Serialize()
    {
        List<Lump> lumps = new List<Lump>();

        //mats
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);

            bw.Write(CraftingMaterials.u);
            bw.Write(CraftingMaterials.a);
            bw.Write(CraftingMaterials.w);
            bw.Write(CraftingMaterials.g);

            lumps.Add(new Lump("CRAFTMATS", stream.ToArray()));

            bw.Close();
            stream.Close();
        }

        //cull trailing slots
        int e = StashItems.Count - 1;
        for (; e >= 0; e--)
            if (StashItems[e] != null)
                break;

        for (int i = 0; i <= e; i++)
            if (StashItems[i] == null)
                lumps.Add(new Lump("null", new byte[0]));
            else
                lumps.Add(StashItems[i].Serialize());

        return lumps;
    }

    public static void Deserialize(List<Lump> lumps)
    {
        StashItems.Clear();

        CraftingMaterials = new CraftingCost(0, 0, 0, 0);

        if (lumps.Count > 0)
            if (lumps[0].lumpName == "CRAFTMATS")
            {
                MemoryStream stream = new MemoryStream(lumps[0].data);
                BinaryReader br = new BinaryReader(stream);

                CraftingMaterials.u = br.ReadInt32();
                CraftingMaterials.a = br.ReadInt32();
                CraftingMaterials.w = br.ReadInt32();
                CraftingMaterials.g = br.ReadInt32();

                br.Close();
                stream.Close();
            }

        for (int i = 1; i < lumps.Count; i++)
        {
            Lump l = lumps[i];

            if (l.lumpName == "null")
                StashItems.Add(null);
            else
            {
                if (!ThingDesignator.Designations.ContainsKey(l.lumpName))
                {
                    Debug.LogError("Stash: Deserialize: designation \"" + l.lumpName + "\" not found in designator");
                    return;
                }

                GameObject prefab = ThingDesignator.Designations[l.lumpName];
                InventoryGUIObject g = prefab.GetComponent<InventoryGUIObject>();

                if (g == null)
                {
                    Debug.LogError("Stash: Deserialize: designation \"" + l.lumpName + "\" does not contain <InventoryGUIObject> component");
                    return;
                }

                //g.Deserialize(l.data);
                StashItems.Add(g);
            }
        }

        for (int i = 0; i < ExtraEmptySlots; i++)
            StashItems.Add(null);
    }
}