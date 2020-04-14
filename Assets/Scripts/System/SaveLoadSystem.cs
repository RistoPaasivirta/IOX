using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveLoadSystem
{
    private static int _currentPlayerSlot = -1;
    public static int CurrentPlayerSlot
    {
        get
        {
            return _currentPlayerSlot;
        }

        private set
        {
            _currentPlayerSlot = value;
            Messaging.System.PlayerSlotChanged.Invoke();
        }
    }

    public static void SaveConfig()
    {
        Wad.WriteWad("config.wad", "CWAD", 1, Options.Serialize());
    }

    static bool NeedLoadConfig = true;

    public static void LoadConfig()
    {
        if (!NeedLoadConfig)
            return;

        //save default values
        if (!Wad.WadExists("config.wad"))
            SaveConfig();

        Wad.ReadWad("config.wad", out string wadtype, out int version, out List<Lump> data);

        Options.Deserialize(data);

        NeedLoadConfig = false;
    }

    static bool NeedLoadInfos = true;

    public static void LoadPlayerInfos()
    {
        if (!NeedLoadInfos)
            return;

        PlayerInfoHolder.LoadedHolders.Clear();

        int i = 0;
        while (Wad.WadExists("Games/" + i + "/player.wad"))
        {
            Wad.ReadWad("Games/" + i + "/player.wad", out string wadtype, out int version, out List<Lump> data);

            PlayerInfoHolder p = new PlayerInfoHolder();
            p.Deserialize(data);
            PlayerInfoHolder.LoadedHolders.Add(p);

            i++;
        }

        NeedLoadInfos = false;
    }

    public static void DeletePlayerInfo()
    {
        string path(int i)
        {
            return Path.Combine(Wad.BaseFolder, "Games/" + i);
        }

        if (!Directory.Exists(path(CurrentPlayerSlot)))
            return;

        Directory.Delete(path(CurrentPlayerSlot), true);

        //truncate remaining numbers so there are no gaps in numbers
        while (Directory.Exists(path(++CurrentPlayerSlot)))
            Directory.Move(path(CurrentPlayerSlot), path(CurrentPlayerSlot - 1));

        //refresh
        NeedLoadInfos = true;
        LoadPlayerInfos();
        CurrentPlayerSlot = -1;
    }

    public static bool GameNeedsSave = true;
    public static bool StashNeedsSave = true;

    public static void SaveStash()
    {
        if (!StashNeedsSave || CurrentPlayerSlot == -1)
            return;

        if (PlayerInfo.CurrentLocal.CursorItem != null)
        {
            Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.CursorItem, out int _);
            PlayerInfo.CurrentLocal.CursorItem = null;
        }

        if (PlayerInfo.CurrentLocal.DisassemblerItem != null)
        {
            Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.DisassemblerItem, out int _);
            PlayerInfo.CurrentLocal.DisassemblerItem = null;
        }

        if (PlayerInfo.CurrentLocal.AssemblerItem != null)
        {
            Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.AssemblerItem, out int _);
            PlayerInfo.CurrentLocal.AssemblerItem = null;
        }

        if (PlayerInfo.CurrentLocal.AugmentItem != null)
        {
            Stash.AddItemToFirstEmpty(PlayerInfo.CurrentLocal.AugmentItem, out int _);
            PlayerInfo.CurrentLocal.AugmentItem = null;
        }

        if (Difficulty.SharedStash)
            Wad.WriteWad("stash.wad", "IWAD", 1, Stash.Serialize());
        else
            Wad.WriteWad("Games/" + CurrentPlayerSlot + "/stash.wad", "IWAD", 1, Stash.Serialize());

        StashNeedsSave = false;
    }

    public static void SaveGame()
    {
        if (!GameNeedsSave || CurrentPlayerSlot == -1)
            return;

        Wad.WriteWad("Games/" + CurrentPlayerSlot + "/player.wad", "PWAD", 1, PlayerInfo.Serialize());

        if (!LevelLoader.LevelLoaded || !PlayerInfo.CurrentLocal.PlayerHasBeenInitialized)
        {
            Wad.DeleteWad("Games/" + CurrentPlayerSlot + "/mission.wad");
            return;
        }

        List<SaveGameObject> monsters = new List<SaveGameObject>();
        List<SaveGameObject> projectiles = new List<SaveGameObject>();
        List<SaveGameObject> skills = new List<SaveGameObject>();
        List<SaveGameObject> levelobjects = new List<SaveGameObject>();
        List<SaveGameObject> itempickups = new List<SaveGameObject>();

        foreach (Transform t in LevelLoader.DynamicObjects)
        {
            SaveGameObject s = t.GetComponent<SaveGameObject>();
            if (s == null)
            {
                Debug.Log("SaveLoadSystem: SaveGame: non-savegame object \"" + t.name + "\" found in LevelLoader.DynamicObjects");
                continue;
            }

            switch (s.ObjectType)
            {
                case SaveObjectType.Monster:
                    s.SaveIndex = monsters.Count;
                    monsters.Add(s);
                    break;
                case SaveObjectType.Projectile:
                    s.SaveIndex = projectiles.Count;
                    projectiles.Add(s);
                    break;
                case SaveObjectType.Skill:
                    s.SaveIndex = skills.Count;
                    skills.Add(s);
                    break;
                case SaveObjectType.LevelObject:
                    s.SaveIndex = levelobjects.Count;
                    levelobjects.Add(s);
                    break;
                case SaveObjectType.Pickup:
                    s.SaveIndex = itempickups.Count;
                    levelobjects.Add(s);
                    break;
            }
        }

        List<Lump> lumps = new List<Lump>();
        {
            lumps.Add(new Lump("MONSTERS", new byte[0]));
            foreach (SaveGameObject m in monsters) lumps.Add(new Lump(m.SpawnName, m.Serialize()));

            lumps.Add(new Lump("PROJECTILES", new byte[0]));
            foreach (SaveGameObject p in projectiles) lumps.Add(new Lump(p.SpawnName, p.Serialize()));

            lumps.Add(new Lump("SKILLS", new byte[0]));
            foreach (SaveGameObject s in skills) lumps.Add(new Lump(s.SpawnName, s.Serialize()));

            lumps.Add(new Lump("LEVELOBJECTS", new byte[0]));
            foreach (SaveGameObject l in levelobjects) lumps.Add(new Lump(l.SpawnName, l.Serialize()));

            lumps.Add(new Lump("PICKUPS", new byte[0]));
            foreach (SaveGameObject i in itempickups) lumps.Add(new Lump(i.SpawnName, i.Serialize()));
        }

        Wad.WriteWad("Games/" + CurrentPlayerSlot + "/mission.wad", "MWAD", 1, lumps);

        GameNeedsSave = false;
        NeedLoadInfos = true;
    }

    public static void NewPlayerSlot()
    {
        LoadPlayerInfos();
        CurrentPlayerSlot = PlayerInfoHolder.LoadedHolders.Count;

        GameNeedsSave = true;
        SaveGame();
        LoadStash();

        NeedLoadInfos = true;
    }

    public static bool SelectPlayerSlot(int slot)
    {
        LoadPlayerInfos();

        if (slot < 0 || slot >= PlayerInfoHolder.LoadedHolders.Count)
            return false;

        CurrentPlayerSlot = slot;
        return true;
    }

    public static void LoadStash()
    {
        List<Lump> data = new List<Lump>();

        if (Difficulty.SharedStash && Wad.WadExists("stash.wad"))
            Wad.ReadWad("stash.wad", out string wadType, out int version, out data);
        else if (!Difficulty.SharedStash && Wad.WadExists("Games/" + CurrentPlayerSlot + "/stash.wad"))
            Wad.ReadWad("Games/" + CurrentPlayerSlot + "/stash.wad", out string wadType, out int version, out data);

        Stash.Deserialize(data);

        StashNeedsSave = false;
    }

    public static void LoadGame()
    {
        //playerinfo
        {
            Wad.ReadWad("Games/" + CurrentPlayerSlot + "/player.wad", out string wadtype, out int version, out List<Lump> data);

            PlayerInfo.CurrentLocal = ScriptableObject.CreateInstance<PlayerInfo>();
            PlayerInfo.Deserialize(data);

            Messaging.Player.RPGName.Invoke();
            Messaging.Player.RPGProfession.Invoke();
            Messaging.Player.Portrait.Invoke();
            Messaging.Player.Badge.Invoke();
            Messaging.Player.FactionColors.Invoke();
        }

        GameNeedsSave = false;

        //no full deserialization yet, missions have to be restarted

        //player that saved in hideout will have faster start (no regeneration)
        if (PlayerInfo.CurrentLocal.CurrentMap == "Hideout")
            if (Wad.WadExists("Games/" + CurrentPlayerSlot + "/mission.wad"))
            {
                Messaging.System.ChangeLevel.Invoke(PlayerInfo.CurrentLocal.CurrentMap, 2);
                return;
            }

        Messaging.System.ChangeLevel.Invoke(PlayerInfo.CurrentLocal.CurrentMap, 0);
        return;

        //currentmission
        /*{
            Wad.ReadWad("Games/" + CurrentPlayerSlot + "/mission.wad", out string wadtype, out int version, out loadedMission);

            Messaging.System.ChangeLevel.Invoke(PlayerInfo.CurrentLocal.CurrentMap, -1);

            Messaging.GUI.PauseCurtain.Invoke();
            Messaging.GUI.OpenWindow.Invoke("Ingame Menu");
            Messaging.System.SetTimeScale.Invoke(0f);
            Messaging.GUI.ChangeCursor.Invoke(0);
        }*/
    }

    private static List<Lump> loadedMission = new List<Lump>();

    private static void DeserializeSaveGame()
    {
        indexedMonsters.Clear();

        List <Lump> lumps = Lump.Cull(loadedMission,
            new string[5]
            {
                "MONSTERS",
                "PROJECTILES",
                "SKILLS",
                "LEVELOBJECTS",
                "PICKUPS",
            });

        foreach (Lump l in lumps)
        {
            string spawnName = l.lumpName;

            if (!ThingDesignator.Designations.ContainsKey(spawnName))
            {
                Debug.LogError("SaveLoadSystem: LoadGame: spawn name designation \"" + spawnName + "\" not found in designator");
                return;
            }

            GameObject g = UnityEngine.GameObject.Instantiate(ThingDesignator.Designations[spawnName], LevelLoader.DynamicObjects);
            SaveGameObject s = g.GetComponent<SaveGameObject>();
            s.SpawnName = spawnName;
            s.Deserialize(l.data);
        }

        foreach (Transform t in LevelLoader.DynamicObjects)
        {
            SaveGameObject s = t.GetComponent<SaveGameObject>();
            if (s == null)
            {
                Debug.Log("SaveLoadSystem: LoadGame: non-savegame object found in LevelLoader.DynamicObjects");
                return;
            }
            s.AfterCreated();
        }
    }

    public static List<MonsterCharacter> indexedMonsters = new List<MonsterCharacter>();
    public static MonsterCharacter getIndexedMonster(int i)
    {
        if (i < 0 || i >= indexedMonsters.Count)
            return null;

        return indexedMonsters[i];
    }
}