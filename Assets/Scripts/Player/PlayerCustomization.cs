using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerCustomization")]
public class PlayerCustomization : ReferenceData
{
    public static Sprite[] PlayerPortraits { get; private set; } = new Sprite[0];
    public static Sprite[] PlayerBadges { get; private set; } = new Sprite[0];
    public static Color[] FactionColors { get; private set; } = new Color[0];
    public static string[] PlayerNames { get; private set; } = new string[0];
    public static string[] PlayerProfessions { get; private set; } = new string[0];

    [SerializeField] private Sprite[] playerPortraits = new Sprite[0];
    [SerializeField] private Sprite[] playerBadges = new Sprite[0];
    [SerializeField] private Color[] factionColors = new Color[0];
    [SerializeField] private string[] playerNames = new string[0];
    [SerializeField] private string[] playerProfessions = new string[0];

    public override void Activate()
    {
        PlayerPortraits = playerPortraits;
        PlayerBadges = playerBadges;
        FactionColors = factionColors;
        PlayerNames = playerNames;
        PlayerProfessions = playerProfessions;
    }
}
