using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GUIButton))]
public class PortraitSelectButton : MonoBehaviour
{
    [SerializeField] private Color ActiveColor = Color.cyan;

    GUIButton button;
    Color _normalColor;
    int index;

    private void Awake()
    {
        button = GetComponent<GUIButton>();
        _normalColor = button.NormalColor;

        index = transform.parent.GetSiblingIndex();

        button.onClick.AddListener(() => 
        {
            if (string.IsNullOrEmpty(PlayerInfo.CurrentLocal.RPGName)
                || PlayerInfo.CurrentLocal.RPGName == PlayerCustomization.PlayerNames[PlayerInfo.CurrentLocal.Portrait])
            {
                PlayerInfo.CurrentLocal.RPGName = PlayerCustomization.PlayerNames[index];
                Messaging.Player.RPGName.Invoke();
            }

            if (string.IsNullOrEmpty(PlayerInfo.CurrentLocal.RPGProfession) 
                ||PlayerInfo.CurrentLocal.RPGProfession == PlayerCustomization.PlayerProfessions[PlayerInfo.CurrentLocal.Portrait])
            {
                PlayerInfo.CurrentLocal.RPGProfession = PlayerCustomization.PlayerProfessions[index];
                Messaging.Player.RPGProfession.Invoke();
            }

            PlayerInfo.CurrentLocal.Portrait = index;
            Messaging.Player.Portrait.Invoke();
        });

        Messaging.Player.Portrait.AddListener(() =>
        {
            if (PlayerInfo.CurrentLocal.Portrait == index)
                button.NormalColor = ActiveColor;
            else
                button.NormalColor = _normalColor;

            GetComponent<Image>().color = button.NormalColor;
        });

        if (PlayerInfo.CurrentLocal.Portrait == index)
            button.NormalColor = ActiveColor;
    }
}
