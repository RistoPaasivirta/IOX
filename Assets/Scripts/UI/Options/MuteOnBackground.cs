using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class MuteOnBackground : MonoBehaviour
{
    Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        toggle.onValueChanged.AddListener((b) => 
        {
            Options.MuteOnBackground = b;
        });
    }

    private void OnEnable() =>
        toggle.SetIsOnWithoutNotify(Options.MuteOnBackground);
}
