using UnityEngine;

public class ApplicationFocus : MonoBehaviour
{
    private void OnApplicationFocus(bool focus)
    {
        if (!focus && Options.MuteOnBackground)
            AudioListener.volume = 0f;
        else
            AudioListener.volume = 1f;
    }
}
