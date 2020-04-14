using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CircleCollider2D))]
public class LevelChangeTeleport : MonoBehaviour
{
    [SerializeField] private bool StartActive = true;
    [SerializeField] private float FadeoutTime = .75f;
    [SerializeField] private int EntryPoint = 0;
    [SerializeField] private GameObject TeleportEffect = null;
    [SerializeField] private string ForceTargetLevel = "";
    public UnityEvent OnPlayerTouch = new UnityEvent();
    public bool Active { get; set; }

    string TargetLevel;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(ForceTargetLevel))
            TargetLevel = ForceTargetLevel;
        else
            Messaging.Mission.SetNextMission.AddListener((s) => { TargetLevel = s; });
    }

    private void Start() =>
        Active = StartActive;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Active) return;
        MonsterCharacter m = collision.GetComponent<MonsterCharacter>();
        if (m == null) return;
        if (m.GetComponent<PlayerControls>() == null) return;

        OnPlayerTouch.Invoke();

        //stop the player and make him invulnerable
        m.physicsBody.velocity = Vector2.zero;
        m.impulseVector = Vector2.zero;
        m.Invulnerable = true;
        m.enabled = false;

        //to avoid division by zero just change level instantly
        if (FadeoutTime == 0)
        {
            Messaging.GUI.Blackout.Invoke(0f);
            Messaging.System.ChangeLevel.Invoke(TargetLevel, EntryPoint);
            return;
        }

        GameObject fadeout = new GameObject("fadeout levelchange");

        float timer = 0;

        UpdateCallHook u = fadeout.AddComponent<UpdateCallHook>();
        u.OnUpdateCall.AddListener(() =>
        {
            timer += Time.unscaledDeltaTime;
            float fade = timer / FadeoutTime;
            Messaging.GUI.Blackout.Invoke(Mathf.Clamp01(fade));
        });

        DestroyAfterTime d = fadeout.AddComponent<DestroyAfterTime>();
        d.LifeTime = FadeoutTime + .1f; //small addition to let screen turn to fully black
        d.UseUnscaledTime = true;
        d.OnDestroy.AddListener(() =>
        {
            /*Messaging.GUI.LoadingWindow.Invoke(() => 
            {
                Messaging.System.ChangeLevel.Invoke(TargetLevel, EntryPoint);
            });*/

            Messaging.System.ChangeLevel.Invoke(TargetLevel, EntryPoint);
        });

        if (TeleportEffect != null)
        {
            Instantiate(TeleportEffect, collision.transform.position, Quaternion.identity, LevelLoader.TemporaryObjects);
            Destroy(collision.gameObject);
        }
    }
}
