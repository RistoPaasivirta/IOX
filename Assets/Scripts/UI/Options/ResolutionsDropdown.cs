using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class ResolutionsDropdown : MonoBehaviour
{
    Resolution[] resolutions = new Resolution[0];

    //must be on start, in awake the Screen.Resolutions contain unsupported resolutions
    private void Start()
    {
        Dropdown dropdownMenu = GetComponent<Dropdown>();

        SingleLinkedList<Resolution> _resolutions = new SingleLinkedList<Resolution>();

        foreach (Resolution r in Screen.resolutions)
        {
            if (r.width < 1024 || r.height < 720)
                continue;

            _resolutions.InsertFront(r);
        }

        resolutions = _resolutions.ToArray();

        for (int i = 0; i < resolutions.Length; i++)
        {
            dropdownMenu.options.Add(new Dropdown.OptionData(resolutions[i].ToString()));

            if (ResEquals(Screen.currentResolution, resolutions[i]))
                dropdownMenu.value = i;
        }

        dropdownMenu.captionText.text = Screen.currentResolution.ToString();

        dropdownMenu.onValueChanged.AddListener((value) =>
        {
            Screen.SetResolution(resolutions[value].width, resolutions[value].height, Screen.fullScreenMode);
            Application.targetFrameRate = resolutions[value].refreshRate;
        });
    }

    private bool ResEquals(Resolution a, Resolution b) =>
        a.width == b.width && a.height == b.height && a.refreshRate == b.refreshRate;

    private string ResToString(Resolution res) =>
        res.width + " x " + res.height;
}