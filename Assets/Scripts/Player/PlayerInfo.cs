using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerInfo")]
public class PlayerInfo : ScriptableObject
{
    private static PlayerInfo _currentLocal = null;
    public static PlayerInfo CurrentLocal
    {
        get
        {
            if (_currentLocal == null)
            {
                //Debug.Log("Created a new instance of PlayerInfo");
                _currentLocal = CreateInstance<PlayerInfo>();
                _currentLocal.CurrentMap = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                SaveLoadSystem.NewPlayerSlot();
            }

            return _currentLocal;
        }

        set
        {
            _currentLocal = value;
        }
    }

    public string RPGName = "_debug_";
    public string RPGProfession = "_debug_";
    public int Portrait = 0;
    public int Badge = 0;
    public int Level = 0;
    public Color[] FactionColors = new Color[1] { Color.red };

    public string CurrentMap = "";

    public Weapon[] PlayerWeapons = new Weapon[5];
    public Skill[] PlayerSkills = new Skill[5];
    public Accessory[] PlayerAccessories = new Accessory[3];
    public InventoryGUIObject CursorItem = null;
    public InventoryGUIObject DisassemblerItem = null;
    public InventoryGUIObject AssemblerItem = null;
    public InventoryGUIObject AugmentItem = null;

    public bool PlayerHasBeenInitialized = false;
    public int LastPlayerWeaponSwap = 0;
    public List<Talent> Talents = new List<Talent>();
    public int TalentPoints = 1;

    public static List<Lump> Serialize()
    {
        List<Lump> lumps = new List<Lump>
        {
            new Lump("NAME", Encoding.ASCII.GetBytes(CurrentLocal.RPGName)),
            new Lump("PROFESSION", Encoding.ASCII.GetBytes(CurrentLocal.RPGProfession)),
            new Lump("PORTRAIT", BitConverter.GetBytes(CurrentLocal.Portrait)),
            new Lump("BADGE", BitConverter.GetBytes(CurrentLocal.Badge)),
            new Lump("LEVEL", BitConverter.GetBytes(CurrentLocal.Level)),
            new Lump("DIFFICULTY", BitConverter.GetBytes((int)Difficulty.CurrentDifficulty)),
            new Lump("CURRENTMAP", Encoding.ASCII.GetBytes(CurrentLocal.CurrentMap)),

            new Lump("SHAREDSTASH", BitConverter.GetBytes(Difficulty.SharedStash)),
            new Lump("PERMADEATH", BitConverter.GetBytes(Difficulty.PermanentDeath)),
            new Lump("PDAMAGE", BitConverter.GetBytes(Difficulty.PlayerDamage)),
            new Lump("EDAMAGE", BitConverter.GetBytes(Difficulty.EnemyDamage)),
        };

        //faction colors
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);

            foreach (Color c in CurrentLocal.FactionColors)
            {
                bw.Write(c.r);
                bw.Write(c.g);
                bw.Write(c.b);
                bw.Write(c.a);
            }

            lumps.Add(new Lump("COLORS", stream.ToArray()));

            bw.Close();
            stream.Close();
        }

        for (int i = 0; i < CurrentLocal.PlayerWeapons.Length; i++)
        {
            if (CurrentLocal.PlayerWeapons[i] == null)
            {
                lumps.Add(new Lump("null", new byte[0]));
                continue;
            }

            lumps.Add(CurrentLocal.PlayerWeapons[i].ToLump());
        }

        for (int i = 0; i < CurrentLocal.PlayerSkills.Length; i++)
        {
            if (CurrentLocal.PlayerSkills[i] == null)
            {
                lumps.Add(new Lump("null", new byte[0]));
                continue;
            }

            lumps.Add(CurrentLocal.PlayerSkills[i].ToLump());
        }

        for (int i = 0; i < CurrentLocal.PlayerAccessories.Length; i++)
        {
            if (CurrentLocal.PlayerAccessories[i] == null)
            {
                lumps.Add(new Lump("null", new byte[0]));
                continue;
            }

            lumps.Add(CurrentLocal.PlayerAccessories[i].ToLump());
        }

        if (CurrentLocal.CursorItem == null)
            lumps.Add(new Lump("null", new byte[0]));
        else
            lumps.Add(CurrentLocal.CursorItem.Serialize());

        lumps.Add(new Lump("LASTWEAPON", BitConverter.GetBytes(CurrentLocal.LastPlayerWeaponSwap)));

        //talents
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);

            bw.Write(CurrentLocal.TalentPoints);

            foreach (Talent talent in CurrentLocal.Talents)
                bw.Write(Wad.FixedLength(talent.ReferenceName));

            lumps.Add(new Lump("TALENTS", stream.ToArray()));

            bw.Close();
            stream.Close();
        }

        return lumps;
    }

    public static void Deserialize(List<Lump> lumps)
    {
        int i = 0;

        CurrentLocal.RPGName = Encoding.ASCII.GetString(lumps[i++].data);
        CurrentLocal.RPGProfession = Encoding.ASCII.GetString(lumps[i++].data);
        CurrentLocal.Portrait = BitConverter.ToInt32(lumps[i++].data, 0);
        CurrentLocal.Badge = BitConverter.ToInt32(lumps[i++].data, 0);
        CurrentLocal.Level = BitConverter.ToInt32(lumps[i++].data, 0);
        Difficulty.CurrentDifficulty = BitConverter.ToInt32(lumps[i++].data, 0);
        CurrentLocal.CurrentMap = Encoding.ASCII.GetString(lumps[i++].data);

        Difficulty.SharedStash = BitConverter.ToBoolean(lumps[i++].data, 0);
        Difficulty.PermanentDeath = BitConverter.ToBoolean(lumps[i++].data, 0);
        Difficulty.PlayerDamage = BitConverter.ToInt32(lumps[i++].data, 0);
        Difficulty.EnemyDamage = BitConverter.ToInt32(lumps[i++].data, 0);

        //faction colors
        {
            MemoryStream stream = new MemoryStream(lumps[i].data);
            BinaryReader br = new BinaryReader(stream);

            List<Color> colors = new List<Color>();
            for (int p = 0; p < lumps[i].data.Length / 16; p++)
                colors.Add(new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));

            CurrentLocal.FactionColors = colors.ToArray();

            br.Close();
            stream.Close();

            i++;
        }

        for (int w = 0; w < CurrentLocal.PlayerWeapons.Length; w++, i++)
        {
            if (lumps[i].lumpName == "null")
            {
                CurrentLocal.PlayerWeapons[w] = null;
                continue;
            }

            if (!ThingDesignator.Designations.ContainsKey(lumps[i].lumpName))
            {
                Debug.LogError("PlayerInfo: Deserialize: weaponslot[" + w + "] designation \"" + lumps[i].lumpName + "\" not found in designator");
                continue;
            }

            GameObject prefab = ThingDesignator.Designations[lumps[i].lumpName];
            Weapon weapon = prefab.GetComponent<Weapon>();

            if (weapon == null)
            {
                Debug.LogError("PlayerInfo: Deserialize: weaponslot[" + w + "] designation \"" + lumps[i].lumpName + "\" does not contain <Weapon> component");
                continue;
            }

            //(weapon as InventoryGUIObject).Deserialize(lumps[i].data);
            CurrentLocal.PlayerWeapons[w] = weapon;
        }

        for (int s = 0; s < CurrentLocal.PlayerSkills.Length; s++, i++)
        {
            if (lumps[i].lumpName == "null")
            {
                CurrentLocal.PlayerSkills[s] = null;
                continue;
            }

            if (!ThingDesignator.Designations.ContainsKey(lumps[i].lumpName))
            {
                Debug.LogError("PlayerInfo: Deserialize: skillslot[" + s + "] designation \"" + lumps[i].lumpName + "\" not found in designator");
                continue;
            }

            GameObject prefab = ThingDesignator.Designations[lumps[i].lumpName];
            Skill skill = prefab.GetComponent<Skill>();

            if (skill == null)
            {
                Debug.LogError("PlayerInfo: Deserialize: skillslot[" + s + "] designation \"" + lumps[i].lumpName + "\" does not contain <Skill> component");
                continue;
            }

            //(skill as InventoryGUIObject).Deserialize(lumps[i].data);
            CurrentLocal.PlayerSkills[s] = skill;
        }

        for (int a = 0; a < CurrentLocal.PlayerAccessories.Length; a++, i++)
        {
            if (lumps[i].lumpName == "null")
            {
                CurrentLocal.PlayerAccessories[a] = null;
                continue;
            }

            if (!ThingDesignator.Designations.ContainsKey(lumps[i].lumpName))
            {
                Debug.LogError("PlayerInfo: Deserialize: accessoryslot[" + a + "] designation \"" + lumps[i].lumpName + "\" not found in designator");
                continue;
            }

            GameObject prefab = ThingDesignator.Designations[lumps[i].lumpName];
            Accessory accessory = prefab.GetComponent<Accessory>();

            if (accessory == null)
            {
                Debug.LogError("PlayerInfo: Deserialize: accessoryslot[" + a + "] designation \"" + lumps[i].lumpName + "\" does not contain <Accessory> component");
                continue;
            }

            //(accessory as InventoryGUIObject).Deserialize(lumps[i].data);
            CurrentLocal.PlayerAccessories[a] = accessory;
        }

        if (lumps[i].lumpName == "null")
            CurrentLocal.CursorItem = null;
        else
        {

            if (!ThingDesignator.Designations.ContainsKey(lumps[i].lumpName))
            {
                Debug.LogError("PlayerInfo: Deserialize: cursoritem designation \"" + lumps[i].lumpName + "\" not found in designator");
                return;
            }

            GameObject prefab = ThingDesignator.Designations[lumps[i].lumpName];
            InventoryGUIObject g = prefab.GetComponent<InventoryGUIObject>();

            if (g == null)
            {
                Debug.LogError("PlayerInfo: Deserialize: cursoritem designation \"" + lumps[i].lumpName + "\" does not contain <InventoryGUIObject> component");
                return;
            }

            //g.Deserialize(lumps[i].data);
            CurrentLocal.CursorItem = g;
        }

        CurrentLocal.LastPlayerWeaponSwap = BitConverter.ToInt32(lumps[++i].data, 0);

        //talents
        {
            CurrentLocal.Talents.Clear();

            MemoryStream stream = new MemoryStream(lumps[++i].data);
            BinaryReader br = new BinaryReader(stream);

            CurrentLocal.TalentPoints = br.ReadInt32();

            for (int t = 0; t < (lumps[i].data.Length - 1) / 12; t++)
            {
                string referenceName = Wad.ByteString(br.ReadBytes(12));

                if (!ThingDesignator.Designations.ContainsKey(referenceName))
                {
                    Debug.LogError("PlayerInfo: Deserialize: talent designation \"" + referenceName + "\" not found in designator");
                    return;
                }

                GameObject prefab = ThingDesignator.Designations[referenceName];
                Talent talent = prefab.GetComponent<Talent>();

                if (talent == null)
                {
                    Debug.LogError("PlayerInfo: Deserialize: talent designation \"" + referenceName + "\" does not contain <Talent> component");
                    return;
                }

                CurrentLocal.Talents.Add(talent);
            }

            br.Close();
            stream.Close();

            i++;
        }
    }
}

//used to show player info to GUI
public class PlayerInfoHolder
{
    public static List<PlayerInfoHolder> LoadedHolders = new List<PlayerInfoHolder>();

    public string RPGName;
    public string RPGProfession;
    public int Portrait;
    public int Badge;
    public int Level;
    public int Difficulty;
    public string CurrentMap;
    public Color FactionColor;

    public void Deserialize(List<Lump> lumps)
    {
        int i = 0;

        RPGName = Encoding.ASCII.GetString(lumps[i++].data);
        RPGProfession = Encoding.ASCII.GetString(lumps[i++].data);
        Portrait = BitConverter.ToInt32(lumps[i++].data, 0);
        Badge = BitConverter.ToInt32(lumps[i++].data, 0);
        Level = BitConverter.ToInt32(lumps[i++].data, 0);
        Difficulty = BitConverter.ToInt32(lumps[i++].data, 0);
        CurrentMap = Encoding.ASCII.GetString(lumps[i++].data);

        //SharedStash
        i++;
        //PermanentDeath
        i++;
        //PlayerDamage
        i++;
        //EnemyDamage
        i++;

        //faction color
        {
            MemoryStream stream = new MemoryStream(lumps[i].data);
            BinaryReader br = new BinaryReader(stream);

            FactionColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

            br.Close();
            stream.Close();
        }
    }
}