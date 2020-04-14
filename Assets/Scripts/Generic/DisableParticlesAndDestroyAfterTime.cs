using UnityEngine;
using UnityEngine.Events;

public class DisableParticlesAndDestroyAfterTime : MonoBehaviour
{
    public float LifeTime = 6f;
    public float DisableTime = 4f;
    public UnityEvent OnDisable = new UnityEvent();

    float lifeTimer = 0f;
    float disableTimer = 0f;

    private void Start()
    {
        lifeTimer = LifeTime;
        disableTimer = DisableTime;
    }

    public void ResetTimers()
    {
        lifeTimer = LifeTime;
        disableTimer = DisableTime;
    }

    public void End()
    {
        if (disableTimer > 0f)
        {
            foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
                p.Stop();

            OnDisable.Invoke();

            disableTimer = 0f;
        }

        float d = LifeTime - DisableTime;

        if (lifeTimer > d)
            lifeTimer = d;
    }

    private void Update()
    {
        if (lifeTimer > 0f)
            lifeTimer -= Time.deltaTime;
        else
            Destroy(gameObject);

        if (disableTimer > 0f)
        {
            disableTimer -= Time.deltaTime;

            if (disableTimer <= 0f)
            {
                foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
                    p.Stop();

                OnDisable.Invoke();
            }
        }
    }
}
