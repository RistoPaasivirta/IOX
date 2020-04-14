using UnityEngine;

public class AssignHotkeyGrid : MonoBehaviour
{
    [SerializeField] private GameObject HotkeyButtonPrefab = null;

    private void Awake()
    {
        for (int i = 0; i < HotkeyAssigment.Assigments.Length; i++)
            Instantiate(HotkeyButtonPrefab, transform).GetComponentInChildren<AssignHotkeyButton>().SetTargetHotkey((Hotkey)i);
    }
}
