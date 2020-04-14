using UnityEngine;
using UnityEngine.Events;

public class GenericTimer : MonoBehaviour
{
    public UnityEvent OnTimer = new UnityEvent();
    public float LifeTime = 1;
    public bool UseUnscaledTime = false;
    public bool AutoRestart = false;

    float timer = 0f;

    void Update()
    {
        timer += UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (timer >= LifeTime)
        {
            timer -= LifeTime;

            OnTimer.Invoke();

            if (!AutoRestart)
                enabled = false;
        }
    }
}
