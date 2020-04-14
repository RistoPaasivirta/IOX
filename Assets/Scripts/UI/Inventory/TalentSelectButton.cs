using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GUIButton))]
public class TalentSelectButton : MonoBehaviour
{
    [SerializeField] private Talent LinkedTalent = null;

    GUIButton button;

    private void Refresh()
    {
        if (PlayerInfo.CurrentLocal.Talents.Contains(LinkedTalent))
            button.NormalColor = button.EnterColor = Color.white;
        else
            button.NormalColor = button.EnterColor = Color.clear;

        GetComponent<Image>().color = button.NormalColor;
    }

    private void OnEnable() =>
        Refresh();

    private void Awake()
    {
        button = GetComponent<GUIButton>();

        if (LinkedTalent == null)
        {
            Debug.LogError("TalentSelectButton: LinkedTalent == null");
            return;
        }

        button.onClick.AddListener(() =>
        {
            if (PlayerInfo.CurrentLocal.Talents.Contains(LinkedTalent))
                Messaging.Player.RemoveTalent.Invoke(LinkedTalent);
            else
                Messaging.Player.AddTalent.Invoke(LinkedTalent);

            Messaging.Player.LoadoutRefresh.Invoke();
            Refresh();
        });

        button.OnCursorEnter.AddListener(() =>
        {
            Messaging.Player.TalentText.Invoke(LinkedTalent.Description);
        });

        button.OnCursorExit.AddListener(() =>
        {
            Messaging.Player.TalentText.Invoke("");
        });
    }
}
