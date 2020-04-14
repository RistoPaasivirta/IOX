using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class EnemyDamageInputField : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<InputField>().onEndEdit.AddListener((s) => 
        {
            if (int.TryParse(s, out int enemyDamage))
            {
                Options.CustomEnemyDamage = enemyDamage;

                if (Difficulty.CurrentDifficulty == (int)DifficultyLevel.Custom)
                    Difficulty.EnemyDamage = enemyDamage;
            }
        });
    }
}
