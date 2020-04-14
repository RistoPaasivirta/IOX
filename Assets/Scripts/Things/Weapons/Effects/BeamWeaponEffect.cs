using UnityEngine;

public class BeamWeaponEffect : MonoBehaviour, WeaponPrefabEffect
{
    [SerializeField] private ProgressControlV3D progressControl = null;
    [SerializeField] private float WaitTime = .1f;

    float waitTimer;

    private void Awake() =>
        progressControl.always = false;

    public void Init(Weapon weapon, MonsterCharacter _)
    {
        BeamWeapon beam = (BeamWeapon)weapon;

        if (beam != null)
            beam.OnFire.AddListener(() => 
            {
                waitTimer = WaitTime;
                progressControl.always = true;
                float distance = beam.beamLength + beam.BeamDistanceMod;
                if (distance < 1f)
                    distance = 1f;
                progressControl.maxLength = distance;

                transform.GetChild(0).gameObject.SetActive(true);
            });

        progressControl.always = false;
    }

    private void Update()
    {
        if (waitTimer > 0f)
            waitTimer -= Time.deltaTime;
        else
            progressControl.always = false;
    }
}
