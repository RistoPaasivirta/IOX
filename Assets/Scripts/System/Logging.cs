/*using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Class that stores logged messages and writes them to hard drive in a burst
/// </summary>
public class Logging : MonoBehaviour
{
    public static bool LogToUnity = false;
    private static readonly List<string> buffer = new List<string>();
    public static string LogFileName { get; private set; }

    public static void Log (string message)
    {
        if (string.IsNullOrEmpty(LogFileName))
            return;

        buffer.Add(DateTime.Now.ToString("yyyyMMddHHmmss") + ": " + message);

        if (LogToUnity)
            Debug.Log(message);
    }

    public static void Init(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            Disable();
            return;
        }

        LogFileName = fileName;

        GameObject obj = new GameObject("Logging") { hideFlags = HideFlags.HideAndDontSave };
        DontDestroyOnLoad(obj);

        if (!Directory.Exists(Application.persistentDataPath + "/Logs"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Logs");

        buffer.Clear();
    }

    public static void Disable()
    {
        LogFileName = "";
    }

    //write out the buffered messages during intervals
    int ticks = 0;
    private void FixedUpdate()
    {
        ticks++;
        if (ticks < Options.LoggingInterval)
            return;
        ticks = 0;

        if (buffer.Count == 0)
            return;

        string buf = "";
        foreach (string b in buffer) buf += b + Environment.NewLine;
        buffer.Clear();

        File.AppendAllText(Application.persistentDataPath + "/Logs/" + LogFileName, buf);
    }
}*/
