using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GUIButton))]
public class RecipeSelectButton : MonoBehaviour
{
    [SerializeField] private Image ItemIcon = null;
    [SerializeField] private Text ItemName = null;
    [SerializeField] private CostUIGrid CostImages = null;
    [SerializeField] private Outline Outline = null;
    [SerializeField] private GameObject noCostText = null;

    public GameObject LinkedRecipe;

    private void Start()
    {
        if (LinkedRecipe == null || ItemIcon == null || ItemName == null || CostImages == null || Outline == null || noCostText == null)
        {
            Debug.LogError("RecipeSelectButton: Start: not all references set");
            return;
        }

        InventoryGUIObject item = LinkedRecipe.GetComponent<InventoryGUIObject>();

        if (item == null)
        {
            Debug.LogError("RecipeSelectButton: Start: no <InventoryGUIObject> component found in recipe item");
            return;
        }

        GetComponent<GUIButton>().onClick.AddListener(() => 
        {
            Messaging.Crafting.SelectRecipe.Invoke(item);
        });

        ItemIcon.sprite = item.InventoryIcon;
        ItemIcon.color = Color.white;
        ItemName.text = item.ItemName;
        ItemName.color = item.MainColor;
        Outline.effectColor = item.SecondaryColor;

        noCostText.gameObject.SetActive(item.AssembleCost == CraftingCost.none);
        CostImages.SetCost(item.AssembleCost);
    }
}
