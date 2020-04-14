using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class CameraShakeSlider : MonoBehaviour
{
    Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();

        slider.onValueChanged.AddListener((value) => 
        {
            Options.CameraShakeStrength = value;
        });
    }

    private void OnEnable() =>
        slider.SetValueWithoutNotify(Options.CameraShakeStrength);
}