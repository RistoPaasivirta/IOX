using UnityEngine;

public class Difficulty : MonoBehaviour
{
    public static int CurrentDifficulty = (int)DifficultyLevel.Softcore;
    public static bool SharedStash = true;
    public static bool PermanentDeath = false;
    public static int PlayerDamage = 100;
    public static int EnemyDamage = 100;
}

public enum DifficultyLevel
{
    Custom = 0,
    Softcore = 1,
    Hardcore = 2,
    Eternium = 3
}
