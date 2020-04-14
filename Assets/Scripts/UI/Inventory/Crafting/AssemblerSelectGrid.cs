using UnityEngine;

public class AssemblerSelectGrid : MonoBehaviour
{
    [SerializeField] private GameObject ButtonPrefab = null;
    [SerializeField] private GameObject[] Recipes = new GameObject[0];

    private void Awake()
    {
        if (ButtonPrefab == null)
        {
            Debug.LogError("AssemblerSelectGrid: Awake: ButtonPrefab == null");
            return;
        }

        foreach (GameObject recipe in Recipes)
        {
            RecipeSelectButton r = Instantiate(ButtonPrefab, transform).GetComponent<RecipeSelectButton>();

            if (r == null)
            {
                Debug.LogError("AssemblerSelectGrid: Awake: no component <RecipeSelectButton> found in ButtonPrefab");
                return;
            }

            r.LinkedRecipe = recipe;
        }
    }
}
