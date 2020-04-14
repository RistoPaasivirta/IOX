using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VersionDisplay : MonoBehaviour
{
    private void Start() =>
        GetComponent<Text>().text = "VERSION" + Environment.NewLine + Application.version;
}
