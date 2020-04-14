using UnityEngine;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour
{
    [SerializeField] private GameObject ActionBarSlots = null;
    [SerializeField] private string cooldownChildName = "cooldown";
    [SerializeField] private string overlayChildName = "hover";
    [SerializeField] private string itemChildName = "item";
    [SerializeField] private Image LeftAbilityImage = null;
    [SerializeField] private Image RightAbilityImage = null;
    [SerializeField] private Sprite EmptySlotIcon = null;

    int currentActive = 0;

    private void Awake()
    {
        if (LeftAbilityImage != null)
            Messaging.GUI.LeftAbilityIcon.AddListener(SetLeftIcon);

        if (RightAbilityImage != null)
            Messaging.GUI.RightAbilityIcon.AddListener(SetRightIcon);

        if (ActionBarSlots != null)
        {
            Messaging.GUI.ActiveSlot.AddListener(SetActiveSlot);
            Messaging.GUI.SlotIcon.AddListener(SetSlotIcon);
            Messaging.Player.SkillCooldown.AddListener(SetSkillCooldown);
        }
    }

    void SetLeftIcon(Sprite s)
    {
        if (s == null)
            s = EmptySlotIcon;

        LeftAbilityImage.sprite = s;
    }

    void SetRightIcon(Sprite s)
    {
        if (s == null)
            s = EmptySlotIcon;

        RightAbilityImage.sprite = s;
    }

    void SetActiveSlot(int i)
    {
        if (currentActive >= 0 && currentActive < 5)
            ActionBarSlots.transform.GetChild(currentActive).Find(overlayChildName).gameObject.SetActive(false);

        currentActive = i;

        if (i >= 0 && i < 5)
            ActionBarSlots.transform.GetChild(i).Find(overlayChildName).gameObject.SetActive(true);
    }

    void SetSlotIcon(int i, Sprite s)
    {
        if (i < 0 || i >= ActionBarSlots.transform.childCount)
            return;

        if (s == null)
            s = EmptySlotIcon;

        ActionBarSlots.transform.GetChild(i).Find(itemChildName).GetComponent<Image>().sprite = s;
    }

    void SetSkillCooldown(int i, float cd, float rcd, bool b)
    {
        if (i < 0 || i >= 5)
            return;

        float f = 0;
        if (rcd > 0)
            f = cd / rcd;

        Transform t = ActionBarSlots.transform.GetChild(i + 6);

        if (b)
        {
            t.Find(overlayChildName).gameObject.SetActive(cd > 0);
            t.Find(cooldownChildName).gameObject.SetActive(false);
        }
        else
        {
            t.Find(overlayChildName).gameObject.SetActive(false);
            t.Find(cooldownChildName).gameObject.SetActive(true);
            t.Find(cooldownChildName).GetComponent<Image>().fillAmount = f;
        }
    }
}
