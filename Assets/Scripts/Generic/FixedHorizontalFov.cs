using UnityEngine;

public class FixedHorizontalFov : MonoBehaviour
{
    //used in perspective mode
    public float horizontalFOV = 120f;

    //used in ortographic mode
    public float targetAspect = 1.6f;
    private float initialSize = 0f;

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();

        initialSize = cam.orthographicSize;
}

private void Update()
    {
        if (cam.orthographic)
        {
            cam.orthographicSize = initialSize * targetAspect / cam.aspect;
        }
        else
        {
            float hFOVInRads = horizontalFOV * Mathf.Deg2Rad;
            float vFOVInRads = 2 * Mathf.Atan(Mathf.Tan(hFOVInRads / 2) / cam.aspect);
            float vFOV = vFOVInRads * Mathf.Rad2Deg;
            cam.fieldOfView = vFOV;
        }
    }
}
