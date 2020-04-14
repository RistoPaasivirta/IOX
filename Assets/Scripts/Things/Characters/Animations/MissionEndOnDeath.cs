using UnityEngine;

[RequireComponent(typeof(MonsterCharacter))]
public class MissionEndOnDeath : MonoBehaviour
{
    [SerializeField] private float FadeoutTime = 5f;
    [SerializeField] private string TargetLevel = "Hideout";
    [SerializeField] private int EntryPoint = 1;
    [SerializeField] private int MaxPlayerLevelIncrease = 2;
    [SerializeField] private float Delay = 4f;
    [SerializeField] private int Loot = 1;
    [SerializeField] private GameObject[] LootTable = new GameObject[0];

    private void Awake()
    {
        GetComponent<MonsterCharacter>().OnDeath.AddListener(() => 
        {
            CreateLevelChange(FadeoutTime, TargetLevel, EntryPoint, transform.position);

            //loot
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

                Messaging.GUI.LootMessage.Invoke(item, FadeoutTime + Delay);
            }
        });
    }

    //UnityEvent.AddListener will tie the function to the monobehaviour object,
    //thus we need to create the lambdas outside of the instance or they will get
    //removed instantly (as the host class is destroyed on death)
    static void CreateLevelChange(float fadeoutTime, string targetLevel, int entryPoint, Vector3 camera)
    {
        Messaging.Player.Invulnerable.Invoke(true);
        Messaging.System.SetTimeScale.Invoke(TimeScale.Slowmo);

        Messaging.CameraControl.RemoveShake.Invoke();
        Messaging.CameraControl.Spectator.Invoke(true);
        Messaging.CameraControl.TargetPosition.Invoke(camera);
        //Messaging.CameraControl.TargetRotation.Invoke(Random.value > .5f ? 90 : -90);

        GameObject fadeout = new GameObject("fadeout levelchange");

        float timer = 0;

        UpdateCallHook u = fadeout.AddComponent<UpdateCallHook>();
        u.OnUpdateCall.AddListener(() =>
        {
            timer += Time.unscaledDeltaTime;
            float fade = timer / fadeoutTime;
            Messaging.GUI.Blackout.Invoke(Mathf.Clamp01(fade));
        });

        DestroyAfterTime d = fadeout.AddComponent<DestroyAfterTime>();
        d.LifeTime = fadeoutTime + .1f; //small addition to let screen turn to fully black
        d.UseUnscaledTime = true;
        d.OnDestroy.AddListener(() =>
        {
            Messaging.System.ChangeLevel.Invoke(targetLevel, entryPoint);
        });
    }
}
