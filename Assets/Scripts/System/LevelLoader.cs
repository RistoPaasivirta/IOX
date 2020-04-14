using UnityEngine;

public static class LevelLoader
{
    public static Transform TemporaryObjects { get; private set; } = null;
    public static Transform DynamicObjects { get; private set; } = null;
    public static bool LevelLoaded = false;
    public static int entryPoint = 0;

    public static void InitLevel()
    {
        DynamicObjects = new GameObject("DYNAMIC OBJECTS").transform;
        TemporaryObjects = new GameObject("TEMPORARY OBJECTS").transform;

        Messaging.System.LevelLoaded.Invoke(entryPoint);
        Messaging.System.SpawnLevelObjects.Invoke(entryPoint);
        Messaging.Player.FactionColors.Invoke();
        Messaging.GUI.ChangeCursor.Invoke(1);
        Messaging.Mission.SetNextMission.Invoke("");

        PlayerInfo.CurrentLocal.CurrentMap = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        LevelLoaded = true;
        Messaging.System.SetTimeScale.Invoke(TimeScale.Standard);
    }

    public static void DestroyLevel()
    {
        DynamicObjects = null;
        TemporaryObjects = null;
        LevelLoaded = false;
        Room.DestroyNetwork();
        PlayerInfo.CurrentLocal = null;
    }
}
