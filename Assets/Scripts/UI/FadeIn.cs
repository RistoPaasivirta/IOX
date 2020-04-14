using UnityEngine;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private float FadeTime = 2f;

    private void Awake()
    {
        if (FadeTime == 0)
        {
            Debug.Log("\"" + gameObject.name + "\" -> FadeIn: disabled to avoid division by zero (fadetime == 0 || slowdowntime == 0)");
            return;
        }

        Messaging.System.LevelLoaded.AddListener((i) => 
        {
            if (i < 0)
                return;

            float timer = FadeTime;

            UpdateCallHook u = gameObject.AddComponent<UpdateCallHook>();
            u.OnUpdateCall.AddListener(() =>
            {
                timer -= Time.deltaTime;
                float fade = timer / FadeTime;
                Messaging.GUI.Blackout.Invoke(Mathf.Clamp01(fade));
            });

            DestroyAfterTime d = gameObject.AddComponent<DestroyAfterTime>();
            d.LifeTime = FadeTime;
            d.UseUnscaledTime = false;
            d.OnDestroy.AddListener(() =>
            {
                Messaging.GUI.Blackout.Invoke(0f);
            });
        });
    }
}
