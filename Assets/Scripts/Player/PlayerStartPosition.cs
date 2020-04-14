using UnityEngine;
using UnityEngine.Events;

public class PlayerStartPosition : MonoBehaviour
{
    [SerializeField] private string SpawnName = "Player";
    [SerializeField] private int EntryPoint = 0;
    [SerializeField] private float Delay = 2f;
    [SerializeField] private Vector3 CameraOffset = Vector3.zero;
    [SerializeField] private float CameraRotation = 0f;
    [SerializeField] private float CameraSpeed = 1f;
    [SerializeField] private Vector3 SpawnOffset = Vector3.zero;
    [SerializeField] private GameObject TeleportEffectPrefab = null;
    [SerializeField] private GameObject SpawnEffectPrefab = null;
    [SerializeField] private UnityEvent OnActivate = new UnityEvent();
    [SerializeField] private UnityEvent OnSpawn = new UnityEvent();

    bool respawning = false;
    float timer = 0f;

    private void Awake()
    {
        Messaging.System.LevelLoaded.AddListener((i) => 
        {
            if (i != EntryPoint)
                return;

            if (TeleportEffectPrefab != null)
                Instantiate(TeleportEffectPrefab, transform.position, transform.rotation, LevelLoader.TemporaryObjects);

            Messaging.CameraControl.Spectator.Invoke(true);
            Messaging.CameraControl.SpeedMultiplier.Invoke(CameraSpeed);
            Messaging.CameraControl.TargetPosition.Invoke(transform.position + CameraOffset);
            Messaging.CameraControl.TargetRotation.Invoke(CameraRotation);
            Messaging.CameraControl.Teleport.Invoke();
            Messaging.CameraControl.TargetPosition.Invoke(transform.position);
            Messaging.CameraControl.TargetRotation.Invoke(0);

            respawning = true;
            enabled = true;

            OnActivate.Invoke();
        });

        enabled = false;
    }

    private void Update()
    {
        if (respawning)
        {
            timer += Time.deltaTime;
            if (timer >= Delay)
            {
                if (!ThingDesignator.Designations.ContainsKey(SpawnName))
                {
                    Debug.LogError("PlayerStartPosition \"" + gameObject.name + "\" spawn name designation \"" + SpawnName + "\" not found in designator");
                    return;
                }

                GameObject g = Instantiate(ThingDesignator.Designations[SpawnName], transform.position + SpawnOffset, transform.rotation, LevelLoader.DynamicObjects);
                g.GetComponent<SaveGameObject>().SpawnName = SpawnName;

                Messaging.CameraControl.Spectator.Invoke(false);
                Messaging.CameraControl.SpeedMultiplier.Invoke(1f);

                if (SpawnEffectPrefab != null)
                    Instantiate(SpawnEffectPrefab, g.transform.position, transform.rotation, LevelLoader.TemporaryObjects);

                respawning = false;
                enabled = false;

                OnSpawn.Invoke();
            }
        }
    }
}
