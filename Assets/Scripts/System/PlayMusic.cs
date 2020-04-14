using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    [SerializeField] private MusicPlayer.MusicDef music = new MusicPlayer.MusicDef();
    [SerializeField] private bool AutoPlay = true;

    private void Start()
    {
        if (AutoPlay)
            PlayRandomMusic();
    }

    public void PlayRandomMusic() =>
        MusicPlayer.PlayMusic(music);
}
