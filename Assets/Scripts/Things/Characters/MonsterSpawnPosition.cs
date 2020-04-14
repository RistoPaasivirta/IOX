using UnityEngine;

public class MonsterSpawnPosition : MonoBehaviour
{
    [SerializeField] private string SpawnName = "";
    [SerializeField] private int SpawnGroup = 0;
    [SerializeField] private string TargetRoom = "";
    [SerializeField] private bool StartAwake = false;
    [SerializeField] private bool Respawn = false;
    [SerializeField] private float RespawnRange = 32f;
    [SerializeField] private float LeashRange = 48f;
    [SerializeField] private bool HuntPlayer = true;

    bool Spawned = false;

    private void Awake()
    {
        if (!ThingDesignator.Designations.ContainsKey(SpawnName))
        {
            Debug.LogError("MonsterSpawnPosition \"" + gameObject.name + "\" at position " + gameObject.transform.position + "\" spawn name designation \"" + SpawnName + "\" not found in designator");
            return;
        }

        if (Respawn)
        {
            Messaging.Player.PlayerGridPosition.AddListener((g) =>
            {
                float distanceSqr = (TheGrid.GridPosition(transform.position) - g).sqrMagnitude;

                if (distanceSqr >= LeashRange * LeashRange)
                    Spawned = false;

                if (!Spawned)
                    if (distanceSqr <= RespawnRange * RespawnRange)
                    {
                        Spawn();
                        Spawned = true;
                    }
            });
        }
        else
        {
            Messaging.System.SpawnLevelObjects.AddListener((i) =>
            {
                if (i != SpawnGroup)
                    return;

                Spawn();
            });
        }
    }

    private void Spawn()
    {
        GameObject g = Instantiate(ThingDesignator.Designations[SpawnName], LevelLoader.DynamicObjects);
        g.transform.position = transform.position;
        g.GetComponent<SaveGameObject>().SpawnName = SpawnName;

        MonsterCharacter m = g.GetComponent<MonsterCharacter>();
        if (m != null)
            m.LookDirection = transform.rotation;

        //Blip
        BlipMovement blip = g.GetComponent<BlipMovement>();
        if (blip != null)
        {
            blip.Leashed = Respawn;
            blip.LeashRange = LeashRange;
            blip.homePosition = transform.position;

            for (int r = 0; r < Room.Rooms.Length; r++)
                if (Room.Rooms[r].name == TargetRoom)
                {
                    blip.targetRoom = r;
                    break;
                }

            blip.HuntPlayer = HuntPlayer;

            if (StartAwake)
                blip.TakeOver();
        }
    }
}
