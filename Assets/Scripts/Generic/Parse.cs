using System.Globalization;
using UnityEngine;

/// <summary>
/// Utility to parse values from strings
/// </summary>
public static class Parse
{
    /// <summary>
    /// Does not change target in case of error
    /// </summary>
    public static void SetDouble(string s, ref double target)
    {
        if (!double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out target))
            Messaging.GUI.ScreenMessage.Invoke("Parse: SetDouble: could not parse " + s + " into double!", Color.red);
    }

    /// <summary>
    /// Does not change target in case of error
    /// </summary>
    public static void SetFloat(string s, ref float target)
    {
        if (!float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out target))
            Messaging.GUI.ScreenMessage.Invoke("Parse: SetFloat: could not parse " + s + " into float!", Color.red);
    }

    /// <summary>
    /// Does not change target in case of error
    /// </summary>
    public static void SetInt(string s, ref int target)
    {
        if (!int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out target))
            Messaging.GUI.ScreenMessage.Invoke("Parse: SetInt: could not parse " + s + " into integer!", Color.red);
    }

    /// <summary>
    /// Does not change target in case of error
    /// </summary>
    public static void SetBool(string s, ref bool target)
    {
        if (!bool.TryParse(s, out target))
            Messaging.GUI.ScreenMessage.Invoke("Parse: SetBool: could not parse " + s + " into bool!", Color.red);
    }

    /// <summary>
    /// Returns 0 in case of error
    /// </summary>
    public static double Double (string s)
    {
        if (!double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            Messaging.GUI.ScreenMessage.Invoke("Parse: could not parse " + s + " into double!", Color.red);

        return value;
    }

    /// <summary>
    /// Returns 0 in case of error
    /// </summary>
    public static float Float (string s)
    {
        if (!float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out float value))
            Messaging.GUI.ScreenMessage.Invoke("Parse: could not parse " + s + " into float!", Color.red);

        return value;
    }

    /// <summary>
    /// Returns 0 in case of error
    /// </summary>
    public static int Int (string s)
    {
        if (!int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out int value))
            Messaging.GUI.ScreenMessage.Invoke("Parse: could not parse " + s + " into integer!", Color.red);

        return value;
    }

    /// <summary>
    /// Returns false in case of error
    /// </summary>
    public static bool Bool (string s)
    {
        if (!bool.TryParse(s, out bool value))
            Messaging.GUI.ScreenMessage.Invoke("Parse: could not parse " + s + " into bool!", Color.red);

        return value;
    }
}