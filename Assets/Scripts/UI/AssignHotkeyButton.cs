using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GUIButton))]
public class AssignHotkeyButton : MonoBehaviour
{
    [SerializeField] private Hotkey TargetHotkey = Hotkey.Attack1;
    [SerializeField] private Text LabelText = null;
    [SerializeField] private Text KeyText = null;

    private void Awake()
    {
        LabelText.text = TargetHotkey.ToString();

        GetComponent<GUIButton>().onClick.AddListener(() =>
        {
            Messaging.GUI.AssignHotkey.Invoke(TargetHotkey);
        });

        Messaging.GUI.RefreshHotkeys.AddListener(() => 
        {
            KeyText.text = HotkeyAssigment.Assigments[(int)TargetHotkey].ToString();
        });
    }

    private void OnEnable() =>
        KeyText.text = HotkeyAssigment.Assigments[(int)TargetHotkey].ToString();
    
    public void SetTargetHotkey(Hotkey hotkey)
    {
        TargetHotkey = hotkey;
        LabelText.text = HotkeyAssigment.HotkeyName(TargetHotkey);
        KeyText.text = HotkeyAssigment.Assigments[(int)TargetHotkey].ToString();
    }
}
