using UnityEngine;

[RequireComponent(typeof(CommonUsableObject))]
public class ResupplyCrate : MonoBehaviour
{
    [SerializeField] private float Cooldown = 2;

    float timer;

    private void Awake()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        GetComponent<CommonUsableObject>().OnUse.AddListener((character) => 
        {
            if (timer > 0f)
                return;

            timer = Cooldown;

            audioSource?.Play();

            character.HitPoints = character.MaxHitPoints;
            character.ChargePoints = character.MaxChargePoints;
            for (int a = 0; a < character.ammunition.Length; a++)
                character.ammunition[a] = character.maxAmmo[a];
        });
    }

    private void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;
    }
}