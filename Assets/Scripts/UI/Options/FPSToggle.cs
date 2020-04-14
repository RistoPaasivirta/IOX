using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class FPSToggle : MonoBehaviour
{
    Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        toggle.onValueChanged.AddListener((b) => 
        {
            Messaging.GUI.ShowFPS.Invoke(b);

            Options.ShowFPS = b;
        });
    }

    private void OnEnable() =>
        toggle.SetIsOnWithoutNotify(Options.ShowFPS);
}
