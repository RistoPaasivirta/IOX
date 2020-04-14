using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BurningDamageArea : MonoBehaviour
{
    [SerializeField] private float Cooldown = .5f;
    [SerializeField] private int BurnTotalDamage = 20000;
    [SerializeField] private int DamagePerBurn = 1000;

    float timer;

    private void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (timer > 0f)
            return;

        Damageable d = collision.GetComponent<Damageable>();
        if (d == null)
            return;

        timer = Cooldown;

        d.Dot(new DotFrame(BurnTotalDamage, DamagePerBurn, DamageType.Fire, null));
    }
}
