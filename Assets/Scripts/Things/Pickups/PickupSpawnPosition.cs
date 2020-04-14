using UnityEngine;

public class PickupSpawnPosition : MonoBehaviour
{
    [SerializeField] private string SpawnName = "";
    [SerializeField] private int SpawnGroup = 0;
    
    private void Awake()
    {
        Messaging.System.SpawnLevelObjects.AddListener((i) =>
        {
            if (i != SpawnGroup)
                return;

            if (!ThingDesignator.Designations.ContainsKey(SpawnName))
            {
                Debug.LogError("PickupSpawnPosition \"" + gameObject.name + "\" at position " + gameObject.transform.position + " spawn name designation \"" + SpawnName + "\" not found in designator");
                return;
            }

            GameObject g = Instantiate(ThingDesignator.Designations[SpawnName], LevelLoader.DynamicObjects);
            g.transform.position = transform.position;
            g.GetComponent<SaveGameObject>().SpawnName = SpawnName;
        });
    }
}
