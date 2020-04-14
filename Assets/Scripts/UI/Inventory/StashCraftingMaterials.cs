using UnityEngine;
using UnityEngine.UI;

public class StashCraftingMaterials : MonoBehaviour
{
    [SerializeField] private Text UtilityPartsNumber = null;
    [SerializeField] private Text ArmorPartsNumber = null;
    [SerializeField] private Text WeaponPartsNumber = null;
    [SerializeField] private Text GoldNumber = null;

    private void Awake()
    {
        if (UtilityPartsNumber == null || 
            ArmorPartsNumber == null ||
            WeaponPartsNumber == null ||
            GoldNumber == null)
        {
            Debug.LogError("StashCraftingMaterials: Awake: not all references set");
            return;
        }

        Messaging.Crafting.CraftingMaterialsUpdated.AddListener(() => { UpdateNumbers(); });
    }

    private void OnEnable() =>
        UpdateNumbers();

    private void UpdateNumbers()
    {
        UtilityPartsNumber.text = Stash.CraftingMaterials.u.ToString();
        ArmorPartsNumber.text = Stash.CraftingMaterials.a.ToString();
        WeaponPartsNumber.text = Stash.CraftingMaterials.w.ToString();
        GoldNumber.text = Stash.CraftingMaterials.g.ToString();
    }
}
