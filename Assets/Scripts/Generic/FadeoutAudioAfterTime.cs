using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FadeoutAudioAfterTime : MonoBehaviour
{
    [SerializeField] private float DelayTime = 2f;
    [SerializeField] private float FadeoutTime = 4f;

    AudioSource audioSource;

    float timer = 0f;
    float originalVolume;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        originalVolume = audioSource.volume;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= DelayTime + FadeoutTime)
        {
            audioSource.Stop();
            enabled = false;
            return;
        }

        if (timer > DelayTime)
            if (FadeoutTime <= 0) //to avoid division by zero
                audioSource.volume = 0f;
            else
                audioSource.volume = Mathf.Lerp(originalVolume, 0, (timer - DelayTime) / FadeoutTime);
    }
}
