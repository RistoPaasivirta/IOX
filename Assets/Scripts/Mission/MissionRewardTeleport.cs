using UnityEngine;

[RequireComponent(typeof(LevelChangeTeleport))]
public class MissionRewardTeleport : MonoBehaviour
{
    [SerializeField] private bool MissionCompleted = true;
    [SerializeField] private int MaxPlayerLevelIncrease = 2;
    [SerializeField] private float Delay = 4f;
    [SerializeField] private int Loot = 1;
    [SerializeField] private GameObject[] LootTable = new GameObject[0];

    private void Awake()
    {
        GetComponent<LevelChangeTeleport>().OnPlayerTouch.AddListener(() => 
        {
            if (!MissionCompleted)
                return;

            if (PlayerInfo.CurrentLocal.Level < MaxPlayerLevelIncrease)
                PlayerInfo.CurrentLocal.Level++;

            for (int i = 0; i < Loot; i++)
            {
                int r = Random.Range(0, LootTable.Length);
                InventoryGUIObject item = LootTable[r].GetComponent<InventoryGUIObject>();

                if (item == null)
                {
                    Debug.LogError("MissionRewardTeleport: OnPlayerTouch: no <InventoryGUIObject> component found in loot index [" + r + "]");
                    return;
                }

                if (item is CraftingMatGUIObject)
                    Stash.CraftingMaterials += (item as CraftingMatGUIObject).craftingMats;
                else
                    Stash.AddItemToFirstEmpty(item, out int _);

                Messaging.GUI.LootMessage.Invoke(item, Delay);
            }
        });

        Messaging.Mission.MissionStatus.AddListener((b) => 
        {
            MissionCompleted = b;
        });
    }
}
