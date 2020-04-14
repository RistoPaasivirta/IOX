using UnityEngine;

public class HotkeyAssigment
{
    public bool Pressed
    {
        get
        {
            switch (inputDevice)
            {
                case InputDeviceType.Keyboard:
                    return Input.GetKeyDown(assignedKey);

                case InputDeviceType.Mouse:
                    return Input.GetMouseButtonDown(mouseButton);

                default:
                    return false;
            }
        }
    }

    public bool Continuous
    {
        get
        {
            switch (inputDevice)
            {
                case InputDeviceType.Keyboard:
                    return Input.GetKey(assignedKey);

                case InputDeviceType.Mouse:
                    return Input.GetMouseButton(mouseButton);

                default:
                    return false;
            }
        }
    }

    public enum InputDeviceType : int
    {
        Mouse = 0,
        Keyboard = 1
    }

    public InputDeviceType inputDevice = InputDeviceType.Keyboard;
    public KeyCode assignedKey = KeyCode.None;
    public int mouseButton = 0;

    public HotkeyAssigment(InputDeviceType InputDevice, int MouseButton, KeyCode AssignedKey = KeyCode.None)
    {
        inputDevice = InputDevice;
        mouseButton = MouseButton;
        assignedKey = AssignedKey;
    }

    public HotkeyAssigment(KeyCode AssignedKey)
    {
        inputDevice = InputDeviceType.Keyboard;
        assignedKey = AssignedKey;
    }

    public HotkeyAssigment(int InputDevice, int MouseButton, int AssignedKey)
    {
        inputDevice = (InputDeviceType)InputDevice;
        mouseButton = MouseButton;
        assignedKey = (KeyCode)AssignedKey;
    }

    public override string ToString()
    {
        switch (inputDevice)
        {
            default:
            case InputDeviceType.Keyboard:
            {
                //remove the word "Alpha" from numeric keys
                if ((int)assignedKey >= 48 && (int)assignedKey <= 57)
                    return assignedKey.ToString().Substring(5);
                else
                    return assignedKey.ToString();
            }

            case InputDeviceType.Mouse:
                return "Mouse " + mouseButton;
        }
    }

    public static bool InventoryKey
    {
        get
        {
            return Input.GetKey(KeyCode.LeftShift) ||
                Input.GetKey(KeyCode.RightShift) ||
                Input.GetKey(KeyCode.LeftControl) ||
                Input.GetKey(KeyCode.RightControl);
        }
    }

    public static KeyCode ScreenshotKey = KeyCode.F12;

    //default QWERTY hotkeys
    /*public static HotkeyAssigment[] Assigments = new HotkeyAssigment[18]
    {
            new HotkeyAssigment(InputDeviceType.Mouse, 0),
            new HotkeyAssigment(InputDeviceType.Mouse, 1),
            new HotkeyAssigment(KeyCode.W),
            new HotkeyAssigment(KeyCode.S),
            new HotkeyAssigment(KeyCode.A),
            new HotkeyAssigment(KeyCode.D),
            new HotkeyAssigment(KeyCode.F),
            new HotkeyAssigment(KeyCode.Tab),
            new HotkeyAssigment(KeyCode.Alpha1),
            new HotkeyAssigment(KeyCode.Alpha2),
            new HotkeyAssigment(KeyCode.Alpha3),
            new HotkeyAssigment(KeyCode.Alpha4),
            new HotkeyAssigment(KeyCode.Alpha5),
            new HotkeyAssigment(KeyCode.Space),
            new HotkeyAssigment(KeyCode.Q),
            new HotkeyAssigment(KeyCode.E),
            new HotkeyAssigment(KeyCode.R),
            new HotkeyAssigment(KeyCode.C)
    };*/

    //default DVORAK hotkeys
    public static HotkeyAssigment[] Assigments = new HotkeyAssigment[18]
    {
            new HotkeyAssigment(InputDeviceType.Mouse, 0),
            new HotkeyAssigment(InputDeviceType.Mouse, 1),
            new HotkeyAssigment(KeyCode.Comma),
            new HotkeyAssigment(KeyCode.O),
            new HotkeyAssigment(KeyCode.A),
            new HotkeyAssigment(KeyCode.E),
            new HotkeyAssigment(KeyCode.U),
            new HotkeyAssigment(KeyCode.Tab),
            new HotkeyAssigment(KeyCode.Alpha1),
            new HotkeyAssigment(KeyCode.Alpha2),
            new HotkeyAssigment(KeyCode.Alpha3),
            new HotkeyAssigment(KeyCode.Alpha4),
            new HotkeyAssigment(KeyCode.Alpha5),
            new HotkeyAssigment(KeyCode.Space),
            new HotkeyAssigment(KeyCode.Quote),
            new HotkeyAssigment(KeyCode.Period),
            new HotkeyAssigment(KeyCode.P),
            new HotkeyAssigment(KeyCode.J)
    };

    public static string HotkeyName(Hotkey hotkey)
    {
        switch (hotkey)
        {
            default: return "UNKNOWN HOTKEY";

            case Hotkey.Attack1: return "Attack  1";
            case Hotkey.Attack2: return "Attack 2";
            case Hotkey.Up: return "Up";
            case Hotkey.Down: return "Down";
            case Hotkey.Left: return "Left";
            case Hotkey.Right: return "Right";
            case Hotkey.Use: return "Use";
            case Hotkey.Map: return "Map";
            case Hotkey.Weapon1: return "Weapon  1";
            case Hotkey.Weapon2: return "Weapon 2";
            case Hotkey.Weapon3: return "Weapon 3";
            case Hotkey.Weapon4: return "Weapon 4";
            case Hotkey.Weapon5: return "Weapon 5";
            case Hotkey.Skill1: return "Skill  1";
            case Hotkey.Skill2: return "Skill 2";
            case Hotkey.Skill3: return "Skill 3";
            case Hotkey.Skill4: return "Skill 4";
            case Hotkey.Skill5: return "Skill 5";
        }
    }
}
