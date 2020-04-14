using UnityEngine;

[RequireComponent(typeof(Behavior))]
public class PlayMusicOnGainTarget : MonoBehaviour
{
    [SerializeField] private MusicPlayer.MusicDef Music = null;

    private void Awake()
    {
        GetComponent<Behavior>().OnGainTarget.AddListener(() => 
        {
            MusicPlayer.PlayMusic(Music);
        });
    }
}
