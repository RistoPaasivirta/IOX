using UnityEngine;

public class AnimateMissionTrigger : MonoBehaviour
{
    [System.Serializable]
    public class Animation
    {
        public string MissionTrigger = "";
        public float WaitTime = 0f;
        public Vector3 TargetPosition = Vector3.zero;
        public Vector3 TargetRotation = Vector3.zero;
        public Vector3 TargetScale = Vector3.one;
        public float Speed = 1f;
        public bool Impulse = true;
    }

    [SerializeField] private Animation[] Animations = new Animation[0];

    int currentAnimation = -1;
    float waitTimer = 0f;
    float interpolation = 0f;

    Vector3 previousPosition;
    Quaternion previousRotation;
    Vector3 previousScale;

    private void Awake()
    {
        Messaging.Mission.MissionTrigger.AddListener((s) => 
        {
            for (int i = 0; i < Animations.Length; i++)
                if (Animations[i].MissionTrigger == s)
                {
                    currentAnimation = i;
                    waitTimer = Animations[i].WaitTime;
                    interpolation = 0f;

                    previousPosition = transform.localPosition;
                    previousRotation = transform.localRotation;
                    previousScale = transform.localScale;

                    enabled = true;
                }
        });

        enabled = false;
    }

    private void Update()
    {
        if (currentAnimation < 0 || currentAnimation >= Animations.Length)
            return;

        Animation a = Animations[currentAnimation];

        if (a.Impulse)
            interpolation = Mathf.Lerp(interpolation, 1, Time.deltaTime * a.Speed);
        else
            interpolation += Time.deltaTime;

        interpolation = Mathf.Clamp01(interpolation);

        transform.localPosition = Vector3.Lerp(previousPosition, a.TargetPosition, interpolation);
        transform.localRotation = Quaternion.Slerp(previousRotation, Quaternion.Euler(a.TargetRotation), interpolation);
        transform.localScale = Vector3.Lerp(previousScale, a.TargetScale, interpolation);

        if (interpolation > .999f)
            enabled = false;
    }
}
