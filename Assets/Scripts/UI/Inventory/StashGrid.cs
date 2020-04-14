using UnityEngine;

public class StashGrid : MonoBehaviour
{
    [SerializeField] private string ListenWindowName = "Inventory";
    [SerializeField] private GameObject SlotPrefab = null;

    private void Awake()
    {
        if (SlotPrefab == null)
        {
            Debug.LogError("StashGrid: Awake: not all references set");
            return;
        }

        Messaging.GUI.OpenWindow.AddListener((s) => 
        {
            if (s != ListenWindowName)
                return;

            foreach (Transform t in transform)
                Destroy(t.gameObject);

            //to update child count (stash slots reference sibling index)
            transform.DetachChildren();

            for (int i = 0; i < Stash.StashItems.Count; i++)
                Instantiate(SlotPrefab, transform);
        });
    }
}
