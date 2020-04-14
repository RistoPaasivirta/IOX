using UnityEngine;

public static class Sounds
{
    public enum MixerGroup
    {
        Master,
        Music,
        Effects,
        Ambient,
        Interface
    }

    [System.Serializable]
    public class SoundDef
    {
        public AudioClip[] clips = new AudioClip[0];
        public float volume = .1f;
        public float minPitch = .95f;
        public float maxPitch = 1.05f;
        public bool spatialBlend = false;
        public MixerGroup mixerGroup = MixerGroup.Effects;
        public int priority = 128;
        public float minDistance = 5f;
    }

    public static void CreateSound(SoundDef def, Vector3 position = new Vector3())
    {
        if (def.clips.Length == 0)
            return;

        GameObject sound = new GameObject("temporary sound");
        AudioSource a = sound.AddComponent<AudioSource>();
        a.clip = def.clips[Random.Range(0, def.clips.Length)];
        a.volume = def.volume;
        a.pitch = Mathf.Lerp(def.minPitch, def.maxPitch, Random.value);
        a.priority = def.priority;

        sound.transform.SetParent(LevelLoader.TemporaryObjects);
        sound.AddComponent<DestroyAfterSoundPlayed>();

        if (def.spatialBlend)
        {
            a.spatialBlend = 1;
            a.minDistance = def.minDistance;
            sound.transform.position = position;
        }
        else
            a.spatialBlend = 0;

        switch (def.mixerGroup)
        {
            default:
            case MixerGroup.Master:
                a.outputAudioMixerGroup = MixerDesignator.MasterMixerGroup;
                break;

            case MixerGroup.Music:
                a.outputAudioMixerGroup = MixerDesignator.MusicMixerGroup;
                break;

            case MixerGroup.Effects:
                a.outputAudioMixerGroup = MixerDesignator.EffectsMixerGroup;
                break;

            case MixerGroup.Ambient:
                a.outputAudioMixerGroup = MixerDesignator.AmbientMixerGroup;
                break;

            case MixerGroup.Interface:
                a.outputAudioMixerGroup = MixerDesignator.InterfaceMixerGroup;
                break;
        }

        a.Play();
    }

    public static void Create3DSound(Vector3 position, AudioClip clip, MixerGroup mixerGroup, float volume, float minPitch, float maxPitch, int priority = 128, float minDistance = 5f)
    {
        GameObject sound = new GameObject("temporary sound");
        AudioSource a = sound.AddComponent<AudioSource>();
        a.clip = clip;
        a.volume = volume;
        a.pitch = Mathf.Lerp(minPitch, maxPitch, Random.value);
        a.spatialBlend = 1;
        a.minDistance = minDistance;
        a.priority = priority;

        sound.transform.SetParent(LevelLoader.TemporaryObjects);
        sound.AddComponent<DestroyAfterSoundPlayed>();
        sound.transform.position = position;

        switch (mixerGroup)
        {
            default:
            case MixerGroup.Master:
                a.outputAudioMixerGroup = MixerDesignator.MasterMixerGroup;
                break;

            case MixerGroup.Music:
                a.outputAudioMixerGroup = MixerDesignator.MusicMixerGroup;
                break;

            case MixerGroup.Effects:
                a.outputAudioMixerGroup = MixerDesignator.EffectsMixerGroup;
                break;

            case MixerGroup.Ambient:
                a.outputAudioMixerGroup = MixerDesignator.AmbientMixerGroup;
                break;

            case MixerGroup.Interface:
                a.outputAudioMixerGroup = MixerDesignator.InterfaceMixerGroup;
                break;
        }

        a.Play();
    }

    public static void Create2DSound(AudioClip clip, MixerGroup mixerGroup, float volume, float minPitch, float maxPitch, int priority = 128)
    {
        GameObject sound = new GameObject("temporary sound");
        AudioSource a = sound.AddComponent<AudioSource>();
        a.clip = clip;
        a.volume = volume;
        a.pitch = Mathf.Lerp(minPitch, maxPitch, Random.value);
        a.spatialBlend = 0;
        a.priority = priority;

        sound.transform.SetParent(LevelLoader.TemporaryObjects);
        sound.AddComponent<DestroyAfterSoundPlayed>();

        switch (mixerGroup)
        {
            default:
            case MixerGroup.Master:
                a.outputAudioMixerGroup = MixerDesignator.MasterMixerGroup;
                break;

            case MixerGroup.Music:
                a.outputAudioMixerGroup = MixerDesignator.MusicMixerGroup;
                break;

            case MixerGroup.Effects:
                a.outputAudioMixerGroup = MixerDesignator.EffectsMixerGroup;
                break;

            case MixerGroup.Ambient:
                a.outputAudioMixerGroup = MixerDesignator.AmbientMixerGroup;
                break;

            case MixerGroup.Interface:
                a.outputAudioMixerGroup = MixerDesignator.InterfaceMixerGroup;
                break;
        }

        a.Play();
    }
}
