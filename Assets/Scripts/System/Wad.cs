using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class Wad
{
    //public static readonly string BaseFolder = Application.persistentDataPath;
    public static readonly string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/IOX";

    public static bool WadExists(string file)
    {
        string path = Path.Combine(BaseFolder, file);
        return File.Exists(path);
    }

    public static bool ReadWad(string file, out string wadType, out int version, out List<Lump> lumps)
    {
        lumps = new List<Lump>();
        wadType = "";
        version = -1;

        string path = Path.Combine(BaseFolder, file);
        if (!File.Exists(path))
        {
            Debug.LogError("Wad: ReadWad: File \"" + file + "\" does not exist");
            return false;
        }

        FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        BinaryReader reader = new BinaryReader(stream, Encoding.ASCII);

        if (file.Length < 4)
        {
            reader.Close();
            stream.Close();
            Debug.LogError("Wad: ReadWad: WAD length < 4");
            return false;
        }

        try
        {
            stream.Seek(0, SeekOrigin.Begin);

            wadType = Encoding.ASCII.GetString(reader.ReadBytes(4));
            version = reader.ReadInt32();
            int numlumps = reader.ReadInt32();
            int lumpsoffset = reader.ReadInt32();

            stream.Seek(lumpsoffset, SeekOrigin.Begin);

            for (int i = 0; i < numlumps; i++)
            {
                int offset = reader.ReadInt32();
                int length = reader.ReadInt32();
                string name = ByteString(reader.ReadBytes(12));

                lumps.Add(new Lump(offset, length, name));
            }

            //load the whole wad into memory
            long bytes = 0;
            foreach (Lump l in lumps)
            {
                l.data = new byte[l.length];
                stream.Seek(l.offset, SeekOrigin.Begin);
                stream.Read(l.data, 0, l.length);
                bytes += l.length;
            }

            //Debug.Log("Loaded WAD \"" + file + "\" (" + bytes + " bytes in lumps)");
        }
        catch (Exception e)
        {
            Debug.LogError("Wad: ReadWad: Reader exception");
            Debug.LogError(e);

            reader.Close();
            stream.Close();
            return false;
        }

        reader.Close();
        stream.Close();
        return true;
    }

    public static byte[] FixedLength(string input)
    {
        if (input.Length > 12)
            return Encoding.ASCII.GetBytes(input.Substring(0, 12));
        else
            return Encoding.ASCII.GetBytes(input.PadRight(12, ' '));
    }

    public static string ByteString(byte[] bytes)
    {
        return Encoding.ASCII.GetString(bytes).TrimEnd(' ');
    }

    public static bool WriteWad(string file, string wadType, int version, List<Lump> lumps)
    {
        string path = Path.Combine(BaseFolder, file);
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        FileStream stream = File.Create(path);
        BinaryWriter writer = new BinaryWriter(stream, Encoding.ASCII);

        try
        {
            writer.Write(wadType.ToCharArray());
            writer.Write(version);
            writer.Write(lumps.Count);
            writer.Write(16);
            int offset = 16 + lumps.Count * 20;
            int bytes = 0;

            foreach (Lump l in lumps)
            {
                writer.Write(offset);
                writer.Write(l.data.Length);
                writer.Write(FixedLength(l.lumpName));
                offset += l.data.Length;
                bytes += l.data.Length;
            }

            foreach (Lump l in lumps)
                writer.Write(l.data);

            //Debug.Log("Wrote WAD \"" + file + "\" (" + bytes + " bytes in lumps)");
        }
        catch (Exception e)
        {
            Debug.LogError("Wad: WriteWad: Writer exception");
            Debug.LogError(e);

            writer.Close();
            stream.Close();
            return false;
        }

        writer.Close();
        stream.Close();
        return true;
    }

    public static bool DeleteWad(string file)
    {
        string path = Path.Combine(BaseFolder, file);

        if (File.Exists(file))
        {
            File.Delete(file);
            return true;
        }

        return false;
    }
}