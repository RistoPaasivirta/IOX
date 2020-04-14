using UnityEngine;

[RequireComponent(typeof(CommonUsableObject), typeof(AudioSource))]
public class OnUseSound : MonoBehaviour
{
    private void Awake()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        GetComponent<CommonUsableObject>().OnUse.AddListener((_) => 
        {
            audioSource.Play();
        });
    }
}
