using UnityEngine;
using UnityEngine.Events;

public class DestroyAfterTime : MonoBehaviour 
{
    public UnityEvent OnDestroy = new UnityEvent();
    public float LifeTime = 1;
    public bool UseUnscaledTime = false;

    float timer = 0f;

    void Update()
    {
        timer += UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (timer >= LifeTime)
        {
            OnDestroy.Invoke();
            Destroy(gameObject);
        }
    }
}
