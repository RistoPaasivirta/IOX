using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class PlayerDamageInputField : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<InputField>().onEndEdit.AddListener((s) => 
        {
            if (int.TryParse(s, out int playerDamage))
            {
                Options.CustomPlayerDamage = playerDamage;

                if (Difficulty.CurrentDifficulty == (int)DifficultyLevel.Custom)
                    Difficulty.PlayerDamage = playerDamage;
            }
        });
    }
}
