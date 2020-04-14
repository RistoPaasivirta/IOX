using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class AssemblerItemStats : MonoBehaviour
{
    private void Awake()
    {
        Text text = GetComponent<Text>();

        Messaging.Crafting.SelectRecipe.AddListener((item) => 
        {
            if (item == null)
            {
                text.text = "";
                return;
            }

            text.text = item.GetShortStats;
        });
    }
}
