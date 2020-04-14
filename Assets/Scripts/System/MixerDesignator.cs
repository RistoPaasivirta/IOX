using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Designators/Mixers")]
public class MixerDesignator : ReferenceData
{
    public static AudioMixer MainMixer { get; private set; }
    public static AudioMixerGroup MasterMixerGroup { get; private set; }
    public static AudioMixerGroup MusicMixerGroup { get; private set; }
    public static AudioMixerGroup EffectsMixerGroup { get; private set; }
    public static AudioMixerGroup AmbientMixerGroup { get; private set; }
    public static AudioMixerGroup InterfaceMixerGroup { get; private set; }

    [SerializeField] private AudioMixer mainMixer = null;
    [SerializeField] private AudioMixerGroup masterMixerGroup = null;
    [SerializeField] private AudioMixerGroup musicMixerGroup = null;
    [SerializeField] private AudioMixerGroup effectsMixerGroup = null;
    [SerializeField] private AudioMixerGroup ambientMixerGroup = null;
    [SerializeField] private AudioMixerGroup interfaceMixerGroup = null;

    public enum MixerGroup
    {
        Master,
        Music,
        Effects,
        Ambient,
        Interface
    }

    public override void Activate()
    {
        MainMixer = mainMixer;
        MasterMixerGroup = masterMixerGroup;
        MusicMixerGroup = musicMixerGroup;
        EffectsMixerGroup = effectsMixerGroup;
        AmbientMixerGroup = ambientMixerGroup;
        InterfaceMixerGroup = interfaceMixerGroup;
    }
}
