using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class TutorialsToggle : MonoBehaviour
{
    Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        toggle.onValueChanged.AddListener((b) => 
        {
            Options.Tutorials = b;
        });
    }

    private void OnEnable() =>
        toggle.SetIsOnWithoutNotify(Options.Tutorials);
}
