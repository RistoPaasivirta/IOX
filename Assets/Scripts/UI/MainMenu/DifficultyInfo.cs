using UnityEngine;

public class DifficultyInfo : MonoBehaviour
{
    [SerializeField] private DifficultyLevel difficulty = DifficultyLevel.Softcore;

    private void Awake()
    {
        Messaging.System.ChangeDifficulty.AddListener((i) => 
        {
            gameObject.SetActive(i == (int)difficulty);
        });
    }

    private void OnEnable() =>
        gameObject.SetActive(difficulty == (DifficultyLevel)Difficulty.CurrentDifficulty);
}
