using UnityEngine;

public class WeaponRecoilAnimation : MonoBehaviour, WeaponPrefabEffect
{
    [SerializeField] private Vector3 RecoilMovement = new Vector3(-.4f, 0, 0);
    [SerializeField] private float Speed = 4;

    float recoilAmount;
    Vector3 originalPosition;
    Vector3 recoilPosition;

    private void Awake()
    {
        originalPosition = transform.localPosition;
        recoilPosition = originalPosition + RecoilMovement;
    }

    private void Update()
    {
        recoilAmount = Mathf.Lerp(recoilAmount, 0, Time.deltaTime * Speed);
        transform.localPosition = Vector3.Lerp(originalPosition, recoilPosition, recoilAmount);
    }

    public void Init(Weapon weapon, MonsterCharacter character)
    {
        weapon.OnFire.AddListener(() => { recoilAmount = 1f; });
        character.OnDeath.AddListener(() => { Destroy(this); });
    }
}
