using System;
using UnityEngine;
using UnityEngine.Events;

public static class Messaging
{
    public static class System
    {        
        public static UnityEvent<int> LevelLoaded = new UnityEventInteger(); //called after the level is loaded
        public static UnityEvent<int> SpawnLevelObjects = new UnityEventInteger(); //level loader calls this with value zero when mission is first created and with -1 if game was loaded from save
        public static UnityEvent<string> Log = new UnityEventString();
        public static UnityEvent<int> ChangeDifficulty = new UnityEventInteger();
        public static UnityEvent PlayerSlotChanged = new UnityEvent();

        [Serializable]
        public class UnityEventLevelLoad : UnityEvent<string, int> { }
        public static UnityEvent<string, int> ChangeLevel = new UnityEventLevelLoad();

        [Serializable]
        public class UnityEventTimeScale : UnityEvent<TimeScale> { }
        public static UnityEvent<TimeScale> SetTimeScale = new UnityEventTimeScale();
    }

    public static class Player
    {
        public static UnityEvent FactionColors = new UnityEvent();
        public static UnityEvent Portrait = new UnityEvent();
        public static UnityEvent Badge = new UnityEvent();
        public static UnityEvent RPGName = new UnityEvent();
        public static UnityEvent RPGProfession = new UnityEvent();
        [Serializable]
        public class UnityEventHpCharge : UnityEvent<int, int> { }
        public static UnityEvent<int, int> Health = new UnityEventHpCharge();
        public static UnityEvent<int, int> Charge = new UnityEventHpCharge();
        public static UnityEvent<int> SoftDamage = new UnityEventInteger();
        public static UnityEvent<Vector2> Position = new UnityEventVector2();
        public static UnityEvent<Vector2> Velocity = new UnityEventVector2();
        public static UnityEvent Death = new UnityEvent();
        [Serializable]
        public class UnityEventAmmo : UnityEvent<int, int, int> { }
        public static UnityEvent<int,int,int> Ammo = new UnityEventAmmo();
        [Serializable]
        public class UnityEventSkillCooldown: UnityEvent<int, float, float, bool> { }
        public static UnityEvent<int, float, float, bool> SkillCooldown = new UnityEventSkillCooldown();
        public static UnityEvent<Vec2I> PlayerGridPosition = new UnityEventVec2I();
        public static UnityEvent EquipPlayerItems = new UnityEvent();
        public static UnityEvent ForceHolster = new UnityEvent();
        public static UnityEvent<int> DeActivateSkill = new UnityEventInteger();
        public static UnityEvent<int> ActivateAccessory = new UnityEventInteger();
        public static UnityEvent<int> DeActivateAccessory = new UnityEventInteger();
        [Serializable]
        public class UnityEventTalent : UnityEvent<Talent> { }
        public static UnityEvent<Talent> AddTalent = new UnityEventTalent();
        public static UnityEvent<Talent> RemoveTalent = new UnityEventTalent();
        public static UnityEvent<int> RefreshStash = new UnityEventInteger();
        public static UnityEvent<int> TalentPoints = new UnityEventInteger();
        public static UnityEvent<string> TalentText = new UnityEventString();
        public static UnityEvent LoadoutRefresh = new UnityEvent();
        public static Func<PlayerStats> GetLocalPlayerStats = () => { return null; };
        public static UnityEvent<bool> Invulnerable = new UnityEventBool();
        public static UnityEvent<int> PlayerEnterRoom = new UnityEventInteger();
    }

    public static class CameraControl
    {
        public static UnityEvent<Vector3> TargetPosition = new UnityEventVector3();
        public static UnityEvent<float> TargetRotation = new UnityEventFloat();
        public static UnityEvent<bool> Spectator = new UnityEventBool();
        public static UnityEvent<float> Shake = new UnityEventFloat();
        public static UnityEvent RemoveShake = new UnityEvent();
        public static UnityEvent Teleport = new UnityEvent();
        public static UnityEvent<float> SpeedMultiplier = new UnityEventFloat();
    }

    public static class GUI
    {
        [Serializable]
        public class UnityEventSetSlotIcon : UnityEvent<int, Sprite> { }
        public static UnityEvent<int, Sprite> SlotIcon = new UnityEventSetSlotIcon();
        public static UnityEvent<int> ActiveSlot = new UnityEventInteger();
        public static UnityEvent<Sprite> LeftAbilityIcon = new UnityEventSprite();
        public static UnityEvent<Sprite> RightAbilityIcon = new UnityEventSprite();
        public static UnityEvent<float> Blackout = new UnityEventFloat();
        public static UnityEvent<float> Painflash = new UnityEventFloat();
        public static UnityEvent<string> OpenWindow = new UnityEventString();
        public static UnityEvent<string> CloseWindow = new UnityEventString();
        public static UnityEvent PauseCurtain = new UnityEvent();
        public static UnityEvent CloseWindows = new UnityEvent();
        public static UnityEvent<int> ChangeCursor = new UnityEventInteger();
        public static UnityEvent<bool> ShowFPS = new UnityEventBool();
        public static UnityEvent<string> OpenCanvas = new UnityEventString();
        [Serializable]
        public class UnityEventDamageIndicator : UnityEvent<Vector3, string, DamageType> { }
        public static UnityEvent<Vector3, string, DamageType> DamageIndicator = new UnityEventDamageIndicator();
        [Serializable]
        public class UnityEventPopupText : UnityEvent<Vector3, string, Color, Color> { }
        public static UnityEvent<Vector3, string, Color, Color> RisingText = new UnityEventPopupText();
        public static Func<Vector3, string, Color, FloatingTexts.FloatingText> FloatingText = (p, t, c) => { return null; };
        public static UnityEvent ClearDynamicGUI = new UnityEvent();
        [Serializable]
        public class UnityEventScreenMessage : UnityEvent<string, Color> { }
        public static UnityEvent<string, Color> ScreenMessage = new UnityEventScreenMessage();
        [Serializable]
        public class UnityEventCommsMessage : UnityEvent<int, Sprite, string> { }
        public static UnityEvent<int, Sprite, string> CommsMessage = new UnityEventCommsMessage();
        [Serializable]
        public class UnityEventHotkey : UnityEvent<Hotkey> { }
        public static UnityEvent<Hotkey> AssignHotkey = new UnityEventHotkey();
        public static UnityEvent CancelAssignHotkey = new UnityEvent();
        public static UnityEvent RefreshHotkeys = new UnityEvent();
        public static UnityEvent RefreshPlayerSlots = new UnityEvent();
        public static UnityEvent<string> HoverBox = new UnityEventString();
        [Serializable]
        public class UnityEventLootMessage : UnityEvent<InventoryGUIObject, float> { }
        public static UnityEvent<InventoryGUIObject, float> LootMessage = new UnityEventLootMessage();
        [Serializable]
        public class UnityEventUIMap : UnityEvent<Sprite, Vec2I, Vec2I> { }
        public static UnityEvent<Sprite, Vec2I, Vec2I> UIMapSprite = new UnityEventUIMap();
        public static UnityEventString UIMapName = new UnityEventString();
        public static UnityEventString UIMapObjectives = new UnityEventString();
    }

    public static class DevTools
    {
        public static UnityEvent VisualizeHeatmap = new UnityEvent();
        public static UnityEvent ClearVisualizations = new UnityEvent();
        [Serializable]
        public class UnityEventDirectionArrow : UnityEvent<Vector3, int, Color> { }
        [Serializable]
        public class UnityEventLine : UnityEvent<Vector3, Vector3, Color> { }
        public static UnityEvent<Vector3, int, Color> CreateDirectionArrow = new UnityEventDirectionArrow();
        public static UnityEvent<Vector3, Vector3, Color> UpdateLine = new UnityEventLine();
        public static UnityEvent<int> VisualizeRoom = new UnityEventInteger();
    }

    public static class Mission
    {
        public static UnityEvent<string> MissionTrigger = new UnityEventString();
        public static UnityEvent<string> SetNextMission = new UnityEventString();
        public static UnityEvent<int> MissionObjective = new UnityEventInteger();
        public static UnityEvent BeginMission = new UnityEvent(); //this is called when player selects a mission and opens up a teleport
        public static UnityEvent<bool> MissionStatus = new UnityEventBool();
        public static UnityEvent<float> BossHealth = new UnityEventFloat();
    }

    public static class Crafting
    {
        public static UnityEvent<InventoryGUIObject> SelectRecipe = new UnityEventInventoryItem();
        public static UnityEvent AssembleSelectedRecipe = new UnityEvent();
        public static UnityEvent CraftingMaterialsUpdated = new UnityEvent();
        public static UnityEvent RefreshAssembler = new UnityEvent();
    }
}
