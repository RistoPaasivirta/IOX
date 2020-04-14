using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class VSyncToggle : MonoBehaviour
{
    Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        toggle.onValueChanged.AddListener((b) => 
        {
            QualitySettings.vSyncCount = b ? 1 : 0;
        });
    }

    private void OnEnable() =>
        toggle.SetIsOnWithoutNotify(QualitySettings.vSyncCount == 1);
}
