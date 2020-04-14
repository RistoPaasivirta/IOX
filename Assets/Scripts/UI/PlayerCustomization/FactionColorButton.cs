using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GUIButton))]
public class FactionColorButton : MonoBehaviour
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
            SingleLinkedList<Color> colors = new SingleLinkedList<Color>();

            for (int i = 0; i < PlayerCustomization.FactionColors.Length; i++)
                if (i == index)
                    colors.InsertFront(PlayerCustomization.FactionColors[i]);
                else
                    colors.InsertBack(PlayerCustomization.FactionColors[i]);

            PlayerInfo.CurrentLocal.FactionColors = colors.ToArray();
            Messaging.Player.FactionColors.Invoke();
        });

        Messaging.Player.FactionColors.AddListener(() => 
        {
            if (PlayerInfo.CurrentLocal.FactionColors.Length > 0)
            {
                if (PlayerInfo.CurrentLocal.FactionColors[0] == PlayerCustomization.FactionColors[index])
                    button.NormalColor = ActiveColor;
                else
                    button.NormalColor = _normalColor;

                GetComponent<Image>().color = button.NormalColor;
            }
        });

        if (PlayerInfo.CurrentLocal.FactionColors[0] == PlayerCustomization.FactionColors[index])
            button.NormalColor = ActiveColor;
    }
}
