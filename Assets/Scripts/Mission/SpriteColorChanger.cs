using UnityEngine;

public class SpriteColorChanger : MonoBehaviour
{
    SpriteRenderer[] sprites = new SpriteRenderer[0];
    [SerializeField] private Color[] Colors = new Color[0];

    private void Awake() =>
        sprites = GetComponentsInChildren<SpriteRenderer>();

    public void SetColor(int color)
    {
        if (color < 0 || color >= Colors.Length)
            return;

        foreach (SpriteRenderer sr in sprites)
            sr.color = Colors[color];
    }
}
