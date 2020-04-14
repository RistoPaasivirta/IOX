using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    Vector3 velocity;
    [SerializeField] private float acceleration = 4f;
    [SerializeField] private float dampen = .96f;

	private void Update ()
    {
        float v = acceleration * Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) velocity.x -= v;
        if (Input.GetKey(KeyCode.E)) velocity.x += v;
        if (Input.GetKey(KeyCode.Comma)) velocity.z += v;
        if (Input.GetKey(KeyCode.O)) velocity.z -= v;

        transform.position += velocity * Time.deltaTime;
    }

    private void FixedUpdate() => 
        velocity *= dampen;
}
