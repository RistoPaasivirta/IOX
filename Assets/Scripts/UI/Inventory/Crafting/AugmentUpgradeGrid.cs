using UnityEngine;

public class AugmentUpgradeGrid : MonoBehaviour
{
    public void SetUpgradeOptions(GameObject[] upgrades)
    {
        AugmentUpgradeDisplay[] displays = GetComponentsInChildren<AugmentUpgradeDisplay>(true);

        for (int c = 0; c < displays.Length; c++)
        {
            if (c >= upgrades.Length)
            {
                displays[c].gameObject.SetActive(false);
                continue;
            }

            displays[c].gameObject.SetActive(true);
            displays[c].SetUpgradeItem(upgrades[c].GetComponent<InventoryGUIObject>());
        }
    }
}
