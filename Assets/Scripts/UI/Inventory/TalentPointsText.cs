using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TalentPointsText : MonoBehaviour
{
    Text text;

    private void Awake()
    {
        text = GetComponent<Text>();

        Messaging.Player.TalentPoints.AddListener((_) => { UpdateText(); });

        UpdateText();
    }

    private void OnEnable() =>
        UpdateText();

    private void UpdateText() =>
        text.text = "Talent points remaining: " + PlayerInfo.CurrentLocal.TalentPoints;
}
