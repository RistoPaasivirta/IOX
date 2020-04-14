using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TimeDisplay : MonoBehaviour
{
    Text t;

    private void Awake() =>
        t = GetComponent<Text>();

    private void Update() =>
        t.text = DateTime.Now.ToString("HH:mm:ss" + Environment.NewLine + "dd/MM/yyyy");
}
