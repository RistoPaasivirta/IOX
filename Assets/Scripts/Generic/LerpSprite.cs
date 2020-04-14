using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LerpSprite : MonoBehaviour 
{
    SpriteRenderer _sprite;

    [SerializeField] private Color TargetColor = Color.white;
    [SerializeField] private float Speed = 4f;

    private void Awake() => 
        _sprite = GetComponent<SpriteRenderer>();

    private void Update() => 
        _sprite.color = Color.Lerp(_sprite.color, TargetColor, Time.deltaTime * Speed);
}
