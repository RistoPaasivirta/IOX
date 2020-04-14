using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GUIButton))]
public class BadgeSelectButton : MonoBehaviour
{
    [SerializeField] private Color ActiveColor = Color.cyan;
    GUIButton button;
    Color _normalColor;
    int index;

    private void Awake()
    {
        button = GetComponent<GUIButton>();
        index = transform.parent.GetSiblingIndex();

        button.onClick.AddListener(() => 
        {
            PlayerInfo.CurrentLocal.Badge = index;
            Messaging.Player.Badge.Invoke();
        });

        _normalColor = button.NormalColor;

        Messaging.Player.Badge.AddListener(() =>
        {
            if (PlayerInfo.CurrentLocal.Badge == index)
                button.NormalColor = ActiveColor;
            else
                button.NormalColor = _normalColor;

            GetComponent<Image>().color = button.NormalColor;
        });

        if (PlayerInfo.CurrentLocal.Badge == index)
            button.NormalColor = ActiveColor;
    }
}
