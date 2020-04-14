using UnityEngine;
using UnityEngine.UI;

public class AssemblePreviewItem : MonoBehaviour
{
    [SerializeField] private Image ItemIcon = null;
    [SerializeField] private Text ItemName = null;
    [SerializeField] private CostUIGrid Cost = null;
    [SerializeField] private Outline Outline = null;
    [SerializeField] private GameObject NoCostText = null;

    private void Awake()
    {
        if (ItemIcon == null || ItemName == null || Cost == null || Outline == null || NoCostText == null)
        {
            Debug.LogError("AssemblePreviewItem: Awake: not all references set");
            return;
        }

        Messaging.Crafting.SelectRecipe.AddListener((recipe) => 
        {
            if (recipe == null)
            {
                ItemIcon.sprite = null;
                ItemIcon.color = Color.black;
                ItemName.text = "";
                NoCostText.gameObject.SetActive(false);
                Cost.SetCost(CraftingCost.none);
            }
            else
            {
                ItemIcon.sprite = recipe.InventoryIcon;
                ItemIcon.color = Color.white;
                ItemName.text = recipe.ItemName;
                ItemName.color = recipe.MainColor;
                Outline.effectColor = recipe.SecondaryColor;
                NoCostText.gameObject.SetActive(recipe.AssembleCost == CraftingCost.none);
                Cost.SetCost(recipe.AssembleCost);
            }
        });
    }
}
