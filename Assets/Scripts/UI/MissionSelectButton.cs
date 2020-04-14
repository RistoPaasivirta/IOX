using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GUIButton))]
public class MissionSelectButton : MonoBehaviour
{
    [SerializeField] private string MissionLevelName = "";
    [SerializeField] private Color ActiveColor = Color.cyan;
    GUIButton button;
    Color _normalColor;

    private void Awake()
    {
        button = GetComponent<GUIButton>();

        button.onClick.AddListener(() =>
        {
            Messaging.Mission.SetNextMission.Invoke(MissionLevelName);
        });

        _normalColor = button.NormalColor;

        Messaging.Mission.SetNextMission.AddListener((s) =>
        {
            if (s == MissionLevelName)
                button.NormalColor = ActiveColor;
            else
                button.NormalColor = _normalColor;

            GetComponent<Image>().color = button.NormalColor;
        });
    }
}
