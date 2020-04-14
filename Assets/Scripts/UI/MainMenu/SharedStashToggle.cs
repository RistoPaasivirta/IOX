using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SharedStashToggle : MonoBehaviour
{
    Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        toggle.onValueChanged.AddListener((b) => 
        {
            Options.CustomSharedStash = b;

            if (Difficulty.CurrentDifficulty == (int)DifficultyLevel.Custom)
                Difficulty.SharedStash = b;
        });
    }

    private void OnEnable() =>
        toggle.SetIsOnWithoutNotify(Options.CustomSharedStash);
}
