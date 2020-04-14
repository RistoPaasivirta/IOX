using UnityEngine;
using UnityEngine.UI;

public class AssignHotkeyPrompt : MonoBehaviour
{
    [SerializeField] private Text HotkeyText = null;

    Hotkey CurrentHotkeyEdit = Hotkey.Use;

    private void Awake()
    {
        if (HotkeyText == null)
        {
            Debug.LogError("AssignHotkeyPrompt: Awake: HotkeyText == null");
            return;
        }

        Messaging.GUI.AssignHotkey.AddListener((hotkey) => 
        {
            string hotkeyName = hotkey.ToString();
            string result = "";

            for (int i = 0; i < hotkeyName.Length; i++)
            {
                if (char.IsDigit(hotkeyName[i]))
                    result += " ";

                result += hotkeyName[i];
            }

            HotkeyText.text = result;
            CurrentHotkeyEdit = hotkey;

            gameObject.SetActive(true);
        });

        Messaging.GUI.CancelAssignHotkey.AddListener(() => 
        {
            gameObject.SetActive(false);
        });

        gameObject.SetActive(false);
    }

    private void OnGUI()
    {
        Event e = Event.current;

        if (e == null)
            return;

        if (e.isMouse && e.type == EventType.MouseUp)
        {
            HotkeyAssigment.Assigments[(int)CurrentHotkeyEdit] = new HotkeyAssigment(HotkeyAssigment.InputDeviceType.Mouse, e.button);
            Messaging.GUI.RefreshHotkeys.Invoke();
            gameObject.SetActive(false);
        }
        else if (e.isKey && e.type == EventType.KeyUp)
        {
            if (e.keyCode == KeyCode.None)
                return;

            if (e.keyCode == KeyCode.Escape)
            {
                gameObject.SetActive(false);
                return;
            }

            HotkeyAssigment.Assigments[(int)CurrentHotkeyEdit] = new HotkeyAssigment(e.keyCode);
            Messaging.GUI.RefreshHotkeys.Invoke();
            gameObject.SetActive(false);
        }
    }
}
