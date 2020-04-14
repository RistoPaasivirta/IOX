using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UIMapName : MonoBehaviour
{
    //with GUI prefabs you need to keep the reference in the prefab or it will try to access old destroyed instance
    Text text;
        
    private void Awake()
    {
        text = GetComponent<Text>();

        Messaging.GUI.UIMapName.AddListener((s) =>
        {
            text.text = s.Replace("\\n", "\n"); ;
        });
    }
}
