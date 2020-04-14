using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailEffect : MonoBehaviour
{
    [SerializeField] private float width = .1f;
    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private Color endColor = Color.clear;

    TrailRenderer trailRenderer;

    private void Start()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(startColor.a, 0.0f), new GradientAlphaKey(endColor.a, 1.0f) });

        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.colorGradient = gradient;
        trailRenderer.widthMultiplier = width;
    }
}
