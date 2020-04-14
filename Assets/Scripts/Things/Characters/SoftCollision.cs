using UnityEngine;

public class SoftCollision : MonoBehaviour
{
    [SerializeField] private float pushStrength = 20f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        Damageable d = collision.collider.GetComponent<Damageable>();
        if (d != null)
        {
            Vector2 dir = collision.transform.position - transform.position;
            dir.SafeNormalize();

            d.Impulse(dir, pushStrength);
        }
    }
}
