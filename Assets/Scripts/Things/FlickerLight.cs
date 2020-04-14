using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickerLight : MonoBehaviour
{
    [SerializeField] private Vector2 flickSpeed = new Vector2(3,4);
    [SerializeField] private Vector2 flickIntensity = new Vector2(2,3);
    [SerializeField] private Vector2 flickRange = new Vector2(10, 15);

    Light _light;
    float lerpPhase;
    float originalIntensity;
    float originalRange;
    float currentSpeed;

    private void Awake()
    {
        _light = GetComponent<Light>();
        originalIntensity = _light.intensity;
        originalRange = _light.range;
    }
 
    private void Update()
    {
        lerpPhase -= Time.deltaTime * currentSpeed;

        if (lerpPhase <= 0)
        {
            currentSpeed = Random.Range(flickSpeed.x, flickSpeed.y);
            _light.intensity = Random.Range(flickIntensity.x, flickIntensity.y);
            _light.range = Random.Range(flickRange.x, flickRange.y);
            lerpPhase = 1f;
        }

        _light.intensity = Mathf.Lerp(originalIntensity, _light.intensity, lerpPhase);
        _light.range = Mathf.Lerp(originalRange, _light.range, lerpPhase);
    }
}
