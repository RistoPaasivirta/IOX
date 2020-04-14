using UnityEngine;

[RequireComponent(typeof(CommonUsableObject))]
public class LootBoxes : MonoBehaviour
{
    [SerializeField] private bool Used = false;
    [SerializeField] private int Loot = 1;
    [SerializeField] private GameObject[] LootTable = new GameObject[0];

    CommonUsableObject useObject;

    private void Awake()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        useObject = GetComponent<CommonUsableObject>();

        useObject.OnUse.AddListener((character) => 
        {
            if (Used)
                return;
            Used = true;

            audioSource?.Play();

            if (LootTable.Length == 0)
                return;

            for (int i = 0; i < Loot; i++)
            {
                int r = Random.Range(0, LootTable.Length);
                InventoryGUIObject item = LootTable[r].GetComponent<InventoryGUIObject>();

                if (item == null)
                {
                    Debug.LogError("LootBoxes: OnUse: no <InventoryGUIObject> component found in loot index [" + r + "]");
                    return;
                }

                if (item is CraftingMatGUIObject)
                    Stash.CraftingMaterials += (item as CraftingMatGUIObject).craftingMats;
                else
                    Stash.AddItemToFirstEmpty(item, out int _);

                Messaging.GUI.LootMessage.Invoke(item, 0);
            }

            useObject.enabled = false;
        });
    }
}
