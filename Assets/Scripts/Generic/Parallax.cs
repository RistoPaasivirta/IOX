using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Vector2 ParallaxAmount = new Vector2(.05f, .05f);
    [SerializeField] private Vector2 origo;
    [SerializeField] private Vector2 max = new Vector2(14,14);

    Vector2 offset;

    void Start() => 
        origo += (Vector2)transform.position;

    void Update()
    {
        Vector3 offset = ((Vector2)Camera.main.transform.position - origo) * ParallaxAmount;
        offset.x = Mathf.Clamp(offset.x, -max.x, max.x);
        offset.y = Mathf.Clamp(offset.y, -max.y, max.y);
        transform.position = new Vector3(origo.x + offset.x, origo.y + offset.y, transform.position.z);
    }
}
