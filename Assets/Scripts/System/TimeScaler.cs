using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    private void Awake() =>
        Messaging.System.SetTimeScale.AddListener((s) => { SetTimeScale(s); });

    private static void SetTimeScale(TimeScale scale)
    {
        if (scale != TimeScale.Paused)
        {
            SaveLoadSystem.GameNeedsSave = true;
            SaveLoadSystem.StashNeedsSave = true;
        }

        float t;
        switch (scale)
        {
            default:
            case TimeScale.Standard: t = 1f; break;
            case TimeScale.Slowmo: t = 0.25f; break;
            case TimeScale.SuperSlowmo: t = 0.05f; break;
            case TimeScale.Paused: t = 0f; break;
        }

        Time.timeScale = t;
        Time.fixedDeltaTime = 0.02f * t;
    }

    public static bool Paused { get => Time.timeScale == 0f; }
    public static bool Slowmo { get => Time.timeScale < 1f; }
    public static bool Unconventional { get => Time.timeScale != 1f; }
}

public enum TimeScale
{
    Standard,
    Slowmo,
    SuperSlowmo,
    Paused
}