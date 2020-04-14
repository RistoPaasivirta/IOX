using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI text box that has ten lines that scroll up
/// </summary>
public class LogBox : MonoBehaviour
{
    Text LogBoxText;
    readonly string[] logText = new string[10] { "", "", "", "", "", "", "", "", "", "" };
    float logTimer = 0f;
    [SerializeField] private float LogTime = 4f;

    private void Awake()
    {
        LogBoxText = GetComponent<Text>();
        Messaging.System.Log.AddListener((s) => { Log(s); });
    }

    //writes out the ten lines
    private void ShowLogText()
    {
        LogBoxText.text = "";

        for (int i = 0; i < 10; i++)
            LogBoxText.text += logText[i] + Environment.NewLine;
    }

    //scrolls one line up, clearing bottom one
    private void ScrollLog()
    {
        for (int i = 1; i < 10; i++)
            logText[i - 1] = String.Copy(logText[i]);

        logText[9] = "";

        ShowLogText();
    }

    //scrolls once and add string to message box
    private void Log(string text)
    {
        for (int i = 0; i < 10; i++)
            if (string.IsNullOrEmpty(logText[i]))
            {
                logText[i] = text;
                ShowLogText();
                logTimer = LogTime;
                return;
            }

        ScrollLog();

        logText[9] = text;
        ShowLogText();
        logTimer = LogTime;
    }

    private void Update()
    {
        logTimer -= Time.deltaTime;
        if (logTimer <= 0f)
        {
            ScrollLog();
            logTimer = LogTime;
        }
    }
}