using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] float RotationSpeed = 1f;
    [SerializeField] float MoveSpeed = 4;
    [SerializeField] float Prediction = 0.12f;
    [SerializeField] Vector2 ShakeFrequency = new Vector2(.02f, .05f);
    [SerializeField] Vector2 ShakeModulus = new Vector2(1f, .7f);
    [SerializeField] float ShakeDecay = .92f;
    [SerializeField] float SpeedDelta = 1f;
    [SerializeField] Vector3 offset = new Vector3(0,0,-200);

    float targetSpeed = 1f;
    float speedMultiplier = 1f;
    float targetRotation;
    float ShakeMagnitude;
    bool spectator;
    Vector3 targetPosition;
    Vector3 targetVelocity;
    float shakeTimer;
    float shakeTime;

    private void Awake()
    {
        Messaging.CameraControl.TargetPosition.AddListener((v) => { targetPosition = v; });
        Messaging.CameraControl.TargetRotation.AddListener((f) => { targetRotation = f; });
        Messaging.CameraControl.Spectator.AddListener((b) => { spectator = b; });

        Messaging.Player.Velocity.AddListener((f) => 
        {
            if (!spectator)
                targetVelocity = f;
        });
        Messaging.Player.Position.AddListener((v) => 
        {
            if (!spectator)
                targetPosition = v;
        });

        Messaging.CameraControl.Shake.AddListener((m) => { ShakeMagnitude = Mathf.Max(ShakeMagnitude, m) * Options.CameraShakeStrength; });
        Messaging.CameraControl.RemoveShake.AddListener(() => { ShakeMagnitude = 0f; });

        Messaging.CameraControl.Teleport.AddListener(() => 
        {
            transform.position = targetPosition + offset;
            transform.rotation = Quaternion.Euler(0, 0, targetRotation);
            speedMultiplier = targetSpeed;
            targetVelocity = Vector3.zero;
        });

        Messaging.CameraControl.SpeedMultiplier.AddListener((f) => 
        {
            targetSpeed = f;
        });
    }

    private void Update()
    {
        speedMultiplier = Mathf.Lerp(speedMultiplier, targetSpeed, Time.deltaTime * SpeedDelta);

        //camera shake looks bugged in slowmo :D
        if (TimeScaler.Unconventional)
            ShakeMagnitude = 0f;

        if (ShakeMagnitude > 0.01f)
        {
            shakeTimer += Time.deltaTime;

            if (shakeTimer >= shakeTime)
            {
                shakeTimer = 0f;
                shakeTime = Random.Range(ShakeFrequency.x, ShakeFrequency.y);
                float x = (Random.value * 2 - 1);
                float y = (Random.value * 2 - 1);
                AxMath.SafeNormalize(ref x, ref y);
                transform.position += (Vector3)(new Vector2(x, y) * ShakeModulus * ShakeMagnitude);
                ShakeMagnitude *= ShakeDecay;
            }
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, targetRotation), Time.deltaTime * RotationSpeed * speedMultiplier);
        Vector3 target = targetPosition + targetVelocity * Prediction;
        transform.position = Vector3.Lerp(transform.position, target + offset, Time.deltaTime * MoveSpeed * speedMultiplier);
    }
}
