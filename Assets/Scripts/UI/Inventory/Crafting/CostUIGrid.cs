using UnityEngine;
using UnityEngine.UI;

public class CostUIGrid : MonoBehaviour
{
    [SerializeField] private GameObject utilityCost = null;
    [SerializeField] private GameObject armorCost = null;
    [SerializeField] private GameObject weaponCost = null;
    [SerializeField] private GameObject goldCost = null;
    [SerializeField] private Text utilityCostNumber = null;
    [SerializeField] private Text armorCostNumber = null;
    [SerializeField] private Text weaponCostNumber = null;
    [SerializeField] private Text goldCostNumber = null;

    public void SetCost(CraftingCost cost)
    {
        utilityCost.SetActive(cost.u != 0);
        utilityCostNumber.text = cost.u + " X";

        armorCost.SetActive(cost.a != 0);
        armorCostNumber.text = cost.a + " X";

        weaponCost.SetActive(cost.w != 0);
        weaponCostNumber.text = cost.w + " X";

        goldCost.SetActive(cost.g != 0);
        goldCostNumber.text = cost.g + " X";
    }
}
