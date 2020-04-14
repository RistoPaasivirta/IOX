using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private string ParameterName = "Master";

    Slider slider;

    void Awake()
    {
        slider = GetComponent<Slider>();

        if (MixerDesignator.MainMixer == null)
        {
            Debug.LogError("VolumeSlider: Awake: MixerDesignator.MainMixer == null");
            return;
        }

        slider.onValueChanged.AddListener((value) => 
        {
            //it is important that the minimum value is 0.0001f to get -80db 
            //(zero would give erroneous result when using logarithm)

            MixerDesignator.MainMixer.SetFloat(ParameterName, Mathf.Log10(value) * 20f);
        });
    }

    private void OnEnable()
    {
        if (MixerDesignator.MainMixer == null)
        {
            Debug.LogError("VolumeSlider: OnEnable: MixerDesignator.MainMixer == null");
            return;
        }

        if (MixerDesignator.MainMixer.GetFloat(ParameterName, out float volume))
            slider.SetValueWithoutNotify((float)System.Math.Pow(10, (volume / 20f)));
        else
            Debug.LogError("VolumeSlider \"" + gameObject.name + "\": OnEnable: could not find parameter \"" + ParameterName + "\" in MainMixer \"" + MixerDesignator.MainMixer.name + "\"");
    }
}