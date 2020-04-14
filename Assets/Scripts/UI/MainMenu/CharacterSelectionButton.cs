using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GUIButton))]
public class CharacterSelectionButton : MonoBehaviour
{
    [SerializeField] private Image Portrait = null;
    [SerializeField] private Image Badge = null;
    [SerializeField] private Text NameText = null;
    [SerializeField] private Text ClassText = null;
    [SerializeField] private Text Difficulty = null;
    [SerializeField] private Image FactionColor = null;
    [SerializeField] private Text CurrentMap = null;

    private void Awake()
    {
        if (Portrait == null || Badge == null || NameText == null || ClassText == null || Difficulty == null || FactionColor == null || CurrentMap == null)
        {
            Debug.LogError("CharacterSelectionButton: OnAwake: Not all references have been set");
            return;
        }
    }

    public void LoadPlayerInfo(int slot)
    {
        if (slot < 0 || slot >= PlayerInfoHolder.LoadedHolders.Count)
        {
            Debug.LogError("CharacterSelectionButton: LoadPlayerInfo: slot [" + slot + "] outside the bounds of PlayerInfoHolder.LoadedHolders");
            return;
        }

        PlayerInfoHolder p = PlayerInfoHolder.LoadedHolders[slot];

        if (p.Portrait >= 0 && p.Portrait < PlayerCustomization.PlayerPortraits.Length)
            Portrait.sprite = PlayerCustomization.PlayerPortraits[p.Portrait];
        if (p.Badge >= 0 && p.Badge < PlayerCustomization.PlayerBadges.Length)
            Badge.sprite = PlayerCustomization.PlayerBadges[p.Badge];

        NameText.text = p.RPGName;
        ClassText.text = p.RPGProfession;
        Difficulty.text = "Level " + p.Level + ", " + ((DifficultyLevel)p.Difficulty).ToString() + " difficulty";
        FactionColor.color = p.FactionColor;

        string mapname = "";
        if (MissionDesignator.Designations.ContainsKey(p.CurrentMap))
            mapname = MissionDesignator.Designations[p.CurrentMap].DisplayName;
        CurrentMap.text = "Currently in: " + mapname;

        GetComponent<GUIButton>().onClick.AddListener(() =>
        {
            SaveLoadSystem.SelectPlayerSlot(slot);
        });
    }
}
