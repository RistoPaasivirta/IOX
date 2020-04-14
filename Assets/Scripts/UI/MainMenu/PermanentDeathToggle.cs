using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class PermanentDeathToggle : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Toggle>().onValueChanged.AddListener((b) => 
        {
            Options.CustomPermanentDeath = b;

            if (Difficulty.CurrentDifficulty == (int)DifficultyLevel.Custom)
                Difficulty.PermanentDeath = b;
        });
    }
}
