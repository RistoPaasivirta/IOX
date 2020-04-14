using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class ViewmodeDropdown : MonoBehaviour
{
    //must be on start, in awake the Screen.Resolutions contain unsupported resolutions
    private void Start()
    {
        Dropdown dropdownMenu = GetComponent<Dropdown>();

        dropdownMenu.value = (int)Screen.fullScreenMode;

        dropdownMenu.onValueChanged.AddListener((value) =>
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, (FullScreenMode)value);
        });
    }

    private bool ResEquals(Resolution a, Resolution b) =>
        a.width == b.width && a.height == b.height && a.refreshRate == b.refreshRate;

    private string ResToString(Resolution res) =>
        res.width + " x " + res.height;
}