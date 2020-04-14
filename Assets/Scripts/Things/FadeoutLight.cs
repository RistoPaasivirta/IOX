using UnityEngine;

[RequireComponent(typeof(Light))]
public class FadeoutLight : MonoBehaviour
{
    Light _light;

    [SerializeField] private float Speed = .5f;

    private void Awake() =>
        _light = GetComponent<Light>();
 
    private void Update()
    {
        _light.intensity = Mathf.Lerp(_light.intensity, 0, Time.deltaTime * Speed);
        _light.range = Mathf.Lerp(_light.range, 0, Time.deltaTime * Speed);

        if (_light.intensity < 0.001f)
            _light.enabled = false;
    }
}
