using UnityEngine;
using UnityEngine.UI;

public class MissionSelectorInfo : MonoBehaviour
{
    [SerializeField] private Transform DescriptionHolder = null;
    [SerializeField] private Image LayoutImage = null;
    [SerializeField] private Text EstimatedTimeText = null;
    [SerializeField] private Image[] ModIcons = new Image[0];
    [SerializeField] private Text[] ModTexts = new Text[0];
    [SerializeField] private Sprite EmptyModSprite = null;

    GameObject lastDescription;

    private void Awake()
    {
        if (DescriptionHolder == null || LayoutImage == null || EstimatedTimeText == null)
        {
            Debug.LogError("MissionSelectorInfo: Awake: Not all references have been set");
            return;
        }

        Messaging.Mission.SetNextMission.AddListener((s) => 
        {
            if (lastDescription != null)
                Destroy(lastDescription);

            lastDescription = null;
            LayoutImage.sprite = null;
            LayoutImage.color = Color.clear;
            EstimatedTimeText.text = "";

            for (int i = 0; i < ModIcons.Length; i++)
                ModIcons[i].sprite = EmptyModSprite;

            for (int i = 0; i < ModTexts.Length; i++)
                ModTexts[i].text = "";

            if (string.IsNullOrEmpty(s))
                return;

            if (!MissionDesignator.Designations.ContainsKey(s))
                return;

            MissionDesignator.MissionDesignation d = MissionDesignator.Designations[s];

            if (d.DescriptionPrefab != null)
                lastDescription = Instantiate(d.DescriptionPrefab, DescriptionHolder);

            LayoutImage.sprite = d.Layout;
            LayoutImage.color = LayoutImage.sprite != null ? Color.white : Color.clear;
            EstimatedTimeText.text = d.EstimatedTime;

            /*for (int i = 0; i < d.mods.Length; i++)
            {
                ModIcons[i].sprite = d.mods[i].Icon;
                ModTexts[i].text = d.mods[i].Description;
            }*/
        });
    }
}
