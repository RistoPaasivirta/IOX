using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIMapObjectives : MonoBehaviour
{
    //with GUI prefabs you need to keep the reference in the prefab or it will try to access old destroyed instance
    Text text;

    private void Awake()
    {
        text = GetComponent<Text>();

        Messaging.GUI.UIMapObjectives.AddListener((s) =>
        {
            text.text = s.Replace("\\n", "\n"); ;
        });
    }
}
