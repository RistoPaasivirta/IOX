using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TalentDescriptionText : MonoBehaviour
{
    Text text;

    private void Awake()
    {
        text = GetComponent<Text>();

        Messaging.Player.TalentText.AddListener((s) => 
        {
            text.text = s.Replace("\\n", "\n"); ;
        });
    }

    private void OnEnable() =>
        text.text = "";
}
