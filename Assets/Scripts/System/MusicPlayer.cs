using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    static MusicPlayer instance;

    [System.Serializable]
    public class MusicDef
    {
        public AudioClip clip;
        public float volume = 1f;
        public float endLoopPoint = 10f;
        public float startLoopPoint = 4f;
    }

    AudioSource[] audioSources;
    bool Channel = false;
    MusicDef currentMusic;
    float TransitionSpeed = 1f;

    public static void PlayMusic(MusicDef musicDef)
    {
        if (instance == null)
        {
            instance = new GameObject("Music Player").AddComponent<MusicPlayer>();
            DontDestroyOnLoad(instance);
        }

        instance.currentMusic = musicDef;
        instance.PlayCurrentMusic();
    }

    private void Awake()
    {
        audioSources = new AudioSource[2];

        for (int i = 0; i < 2; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].outputAudioMixerGroup = MixerDesignator.MusicMixerGroup;
            audioSources[i].volume = 1f;
            audioSources[i].priority = 0;
            audioSources[i].playOnAwake = false;
            audioSources[i].loop = true;
        }
    }

    private void Update()
    {
        if (currentMusic == null)
            return;

        audioSources[0].volume = Mathf.Lerp(audioSources[0].volume, Channel ? 0 : currentMusic.volume, Time.unscaledDeltaTime * TransitionSpeed);
        audioSources[1].volume = Mathf.Lerp(audioSources[1].volume, Channel ? currentMusic.volume : 0, Time.unscaledDeltaTime * TransitionSpeed);

        if (audioSources[0].volume < .01f) audioSources[0].Stop();
        if (audioSources[1].volume < .01f) audioSources[1].Stop();

        if (currentMusic == null) return;
        if (currentMusic.clip == null) return;

        if (Input.GetKeyDown(KeyCode.F8))
            audioSources[Channel ? 1 : 0].time = currentMusic.clip.length - (currentMusic.endLoopPoint + 4);

        if (audioSources[Channel ? 1 : 0].time > currentMusic.clip.length - currentMusic.endLoopPoint)
            PlayCurrentMusic(true);
    }

    private void PlayCurrentMusic(bool loop = false)
    {
        if (currentMusic == null) return;
        if (currentMusic.clip == null) return;

        if (loop)
        {
            audioSources[Channel ? 0 : 1].clip = currentMusic.clip;
            audioSources[Channel ? 0 : 1].time = currentMusic.startLoopPoint;
            audioSources[Channel ? 0 : 1].volume = 0.01f;
            audioSources[Channel ? 0 : 1].Play();
            Channel = !Channel;
            return;
        }

        audioSources[Channel ? 0 : 1].clip = currentMusic.clip;
        audioSources[Channel ? 0 : 1].time = 0f; //need this to prevent a weird seek issue if previous track time has been modified 
        audioSources[Channel ? 0 : 1].volume = 0.01f;
        audioSources[Channel ? 0 : 1].Play();
        Channel = !Channel;
    }
}