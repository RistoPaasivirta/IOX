using UnityEngine;

[RequireComponent(typeof(Light))]
public class LerpLight : MonoBehaviour 
{
    Light _light;

    [SerializeField] private Color TargetColor = Color.white;
    [SerializeField] private float TargetIntensity = 1f;
    [SerializeField] private float Speed = 4f;

    private void Awake() => 
        _light = GetComponent<Light>();

    void Update()
    {
        _light.color = Color.Lerp(_light.color, TargetColor, Time.deltaTime * Speed);
        _light.intensity = Mathf.Lerp(_light.intensity, TargetIntensity, Time.deltaTime * Speed);
    }
}
