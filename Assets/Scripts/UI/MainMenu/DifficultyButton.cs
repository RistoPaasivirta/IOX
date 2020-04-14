using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GUIButton))]
public class DifficultyButton : MonoBehaviour
{
    [SerializeField] private DifficultyLevel difficulty = DifficultyLevel.Softcore;
    [SerializeField] private Color ActiveColor = Color.cyan;

    GUIButton button;
    Color _normalColor;

    private void Awake()
    {
        button = GetComponent<GUIButton>();

        button.onClick.AddListener(() =>
        {
            Difficulty.CurrentDifficulty = (int)difficulty;
            Messaging.System.ChangeDifficulty.Invoke((int)difficulty);

            switch ((DifficultyLevel)Difficulty.CurrentDifficulty)
            {
                case DifficultyLevel.Custom:
                    Difficulty.CurrentDifficulty = 0;
                    Difficulty.SharedStash = Options.CustomSharedStash;
                    Difficulty.PermanentDeath = Options.CustomPermanentDeath;
                    Difficulty.EnemyDamage = Options.CustomEnemyDamage;
                    Difficulty.PlayerDamage = Options.CustomPlayerDamage;
                    break;

                case DifficultyLevel.Softcore:
                    Difficulty.CurrentDifficulty = 1;
                    Difficulty.SharedStash = true;
                    Difficulty.PermanentDeath = false;
                    Difficulty.EnemyDamage = 100;
                    Difficulty.PlayerDamage = 100;
                    break;

                case DifficultyLevel.Hardcore:
                    Difficulty.CurrentDifficulty = 2;
                    Difficulty.SharedStash = true;
                    Difficulty.PermanentDeath = true;
                    Difficulty.EnemyDamage = 100;
                    Difficulty.PlayerDamage = 100;
                    break;

                case DifficultyLevel.Eternium:
                    Difficulty.CurrentDifficulty = 3;
                    Difficulty.SharedStash = false;
                    Difficulty.PermanentDeath = true;
                    Difficulty.EnemyDamage = 100;
                    Difficulty.PlayerDamage = 100;
                    break;
            }
        });

        _normalColor = button.NormalColor;

        Messaging.System.ChangeDifficulty.AddListener((i) =>
        {
            if (i == (int)difficulty)
                button.NormalColor = ActiveColor;
            else
                button.NormalColor = _normalColor;

            GetComponent<Image>().color = button.NormalColor;
        });
    }


    private void OnEnable()
    {
        if (difficulty == (DifficultyLevel)Difficulty.CurrentDifficulty)
            button.NormalColor = ActiveColor;
        else
            button.NormalColor = _normalColor;

        GetComponent<Image>().color = button.NormalColor;
    }
}
