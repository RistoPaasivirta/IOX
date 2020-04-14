using UnityEngine;

[RequireComponent(typeof(MonsterCharacter))]
public class MonsterAilmentEffects : MonoBehaviour
{
    [SerializeField] private GameObject BurningEffectPrefab = null;

    GameObject _burningEffect;

    private void Awake()
    {
        MonsterCharacter m = GetComponent<MonsterCharacter>();

        if (BurningEffectPrefab == null)
        {
            Debug.LogError("MonsterAilmentEffects: burning effect prefab reference not set on monster \"" + gameObject.name + "\"");
            return;
        }
        
        m.OnDeath.AddListener(() =>
        {
            if (_burningEffect != null)
            {
                _burningEffect.transform.SetParent(LevelLoader.TemporaryObjects);
                DisableParticlesAndDestroyAfterTime particleScript = _burningEffect.GetComponent<DisableParticlesAndDestroyAfterTime>();
                if (particleScript != null)
                    particleScript.End();
            }
        });

        m.OnDot.AddListener((frame) =>
        {
            //avoid division by zero
            if (frame.damagePerApplication == 0)
                return;

            if (frame.damageType == DamageType.Fire)
            {
                if (_burningEffect == null)
                    _burningEffect = Instantiate(BurningEffectPrefab, transform.position, Quaternion.identity, transform);

                DisableParticlesAndDestroyAfterTime particleScript = _burningEffect.GetComponent<DisableParticlesAndDestroyAfterTime>();
                if (particleScript != null)
                {
                    particleScript.DisableTime = (frame.totalDamage / frame.damagePerApplication) / 5;
                    particleScript.LifeTime = particleScript.DisableTime + 2f;
                    particleScript.ResetTimers();

                    particleScript.OnDisable.AddListener(() => { _burningEffect = null; });
                }
            }
        });
    }
}
