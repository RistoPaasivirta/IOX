using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class AssembleItemButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<GUIButton>().onClick.AddListener(() => 
        {
            Messaging.Crafting.AssembleSelectedRecipe.Invoke();
        });
    }
}
