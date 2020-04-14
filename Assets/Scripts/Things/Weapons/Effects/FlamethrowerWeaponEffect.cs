using UnityEngine;

public class FlamethrowerWeaponEffect : MonoBehaviour, WeaponPrefabEffect
{
    [SerializeField] private float LightIntensity = 0;
    [SerializeField] private float LightSpeed = 4;
    [SerializeField] private float WaitTime = .2f;

    Light muzzleLight;
    AudioSource[] audioSources = new AudioSource[0];
    ParticleSystem[] particleSystems = new ParticleSystem[0];
    float amount = 0f;
    float waitTimer;

    private void Awake()
    {
        muzzleLight = GetComponentInChildren<Light>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        audioSources = GetComponentsInChildren<AudioSource>();

        if (muzzleLight != null)
            muzzleLight.intensity = 0f;

        foreach (AudioSource a in audioSources)
            a.Stop();

        foreach (ParticleSystem p in particleSystems)
            p.Stop();
    }

    private void Update()
    {
        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;

            amount = Mathf.Lerp(amount, 1, Time.deltaTime * LightSpeed);

            if (muzzleLight != null)
                muzzleLight.intensity = amount * LightIntensity;
        }
        else
        {
            amount = Mathf.Lerp(amount, 0, Time.deltaTime * LightSpeed);

            if (muzzleLight != null)
                muzzleLight.intensity = amount * LightIntensity;

            foreach (ParticleSystem p in particleSystems)
                p.Stop();

            foreach (AudioSource a in audioSources)
                a.Stop();
        }
    }

    public void Init(Weapon weapon, MonsterCharacter _)
    {
        weapon.OnFire.AddListener(() => 
        {
            waitTimer = WaitTime;

            foreach (ParticleSystem p in particleSystems)
                //if (p.isStopped) //particle system is not considered stopped if it still has particles alive, even after emission is stopped
                    p.Play();

            foreach (AudioSource a in audioSources)
                if (!a.isPlaying)
                    a.Play();
        });
    }
}
