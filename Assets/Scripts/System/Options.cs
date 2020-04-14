using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Options
{
    public static bool Tutorials = true;
    public static readonly Vector2 ScreenDistance = new Vector2(27, 17);
    public static readonly Vector2 AlertDistance = new Vector2(31, 21);
    public const float RagdollTime = 4;
    public const int LoggingInterval = 50;
    public const float MaxAttackDistance = 30f;
    public const float MaxImpulse = 10f;
    public const float OnScreenLingerTime = 3f;
    public static float CameraShakeStrength = 1f;
    public static bool ShowFPS = false;
    public static bool CustomPermanentDeath = false;
    public static bool CustomSharedStash = true;
    public static int CustomPlayerDamage = 100;
    public static int CustomEnemyDamage = 100;
    public static bool MuteOnBackground = true;

    public static List<Lump> Serialize()
    {
        List<Lump> lumps = new List<Lump>();

        if (MixerDesignator.MainMixer == null)
        {
            Debug.LogError("Options: Serialize: MixerDesignator.MainMixer == null");
            return lumps;
        }

        //AUDIO LEVELS
        {
            MixerDesignator.MainMixer.GetFloat("MasterVolume", out float masterVolume);
            MixerDesignator.MainMixer.GetFloat("MusicVolume", out float musicVolume);
            MixerDesignator.MainMixer.GetFloat("EffectsVolume", out float effectsVolume);
            MixerDesignator.MainMixer.GetFloat("AmbientVolume", out float ambientVolume);
            MixerDesignator.MainMixer.GetFloat("InterfaceVolume", out float interfaceVolume);

            masterVolume = (float)Math.Pow(10, (masterVolume / 20f));
            musicVolume = (float)Math.Pow(10, (musicVolume / 20f));
            effectsVolume = (float)Math.Pow(10, (effectsVolume / 20f));
            ambientVolume = (float)Math.Pow(10, (ambientVolume / 20f));
            interfaceVolume = (float)Math.Pow(10, (interfaceVolume / 20f));

            lumps.Add(new Lump("MASTERVOL", BitConverter.GetBytes(masterVolume)));
            lumps.Add(new Lump("MUSICVOL", BitConverter.GetBytes(musicVolume)));
            lumps.Add(new Lump("EFFECTSVOL", BitConverter.GetBytes(effectsVolume)));
            lumps.Add(new Lump("AMBIENTVOL", BitConverter.GetBytes(ambientVolume)));
            lumps.Add(new Lump("INTERFACEVOL", BitConverter.GetBytes(interfaceVolume)));
        }

        lumps.Add(new Lump("MUTEINBG", BitConverter.GetBytes(MuteOnBackground)));
        lumps.Add(new Lump("CAMERASHAKE", BitConverter.GetBytes(CameraShakeStrength)));
        lumps.Add(new Lump("SHOWFPS", BitConverter.GetBytes(ShowFPS)));
        lumps.Add(new Lump("VSYNC", BitConverter.GetBytes(QualitySettings.vSyncCount)));
        lumps.Add(new Lump("TUTORIALS", BitConverter.GetBytes(Tutorials)));

        //HOTKEYS
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);

            foreach (HotkeyAssigment hotkey in HotkeyAssigment.Assigments)
            {
                bw.Write((int)hotkey.inputDevice);
                bw.Write(hotkey.mouseButton);
                bw.Write((int)hotkey.assignedKey);
            }

            byte[] data = stream.ToArray();
            bw.Close();
            stream.Close();

            lumps.Add(new Lump("HOTKEYS", data));
        }

        return lumps;
    }

    public static void Deserialize(List<Lump> lumps)
    {
        if (MixerDesignator.MainMixer == null)
        {
            Debug.LogError("Options: DeSerialize: MixerDesignator.MainMixer == null");
            return;
        }

        int i = 0;

        //AUDIOLEVELS
        {
            float masterVolume = BitConverter.ToSingle(lumps[i++].data, 0);
            float musicVolume = BitConverter.ToSingle(lumps[i++].data, 0);
            float effectsVolume = BitConverter.ToSingle(lumps[i++].data, 0);
            float ambientVolume = BitConverter.ToSingle(lumps[i++].data, 0);
            float interfaceVolume = BitConverter.ToSingle(lumps[i++].data, 0);

            masterVolume = Mathf.Log10(masterVolume) * 20f;
            musicVolume = Mathf.Log10(musicVolume) * 20f;
            effectsVolume = Mathf.Log10(effectsVolume) * 20f;
            ambientVolume = Mathf.Log10(ambientVolume) * 20f;
            interfaceVolume = Mathf.Log10(interfaceVolume) * 20f;

            MixerDesignator.MainMixer.SetFloat("MasterVolume", masterVolume);
            MixerDesignator.MainMixer.SetFloat("MusicVolume", musicVolume);
            MixerDesignator.MainMixer.SetFloat("EffectsVolume", effectsVolume);
            MixerDesignator.MainMixer.SetFloat("AmbientVolume", ambientVolume);
            MixerDesignator.MainMixer.SetFloat("InterfaceVolume", interfaceVolume);
        }

        MuteOnBackground = BitConverter.ToBoolean(lumps[i++].data, 0);
        CameraShakeStrength = BitConverter.ToSingle(lumps[i++].data, 0);
        ShowFPS = BitConverter.ToBoolean(lumps[i++].data, 0);
        Messaging.GUI.ShowFPS.Invoke(ShowFPS);
        QualitySettings.vSyncCount = BitConverter.ToInt32(lumps[i++].data, 0);
        Tutorials = BitConverter.ToBoolean(lumps[i++].data, 0);

        //HOTKEYS
        {
            MemoryStream stream = new MemoryStream(lumps[i++].data);
            BinaryReader br = new BinaryReader(stream);

            for (int h = 0; h < HotkeyAssigment.Assigments.Length; h++)
                HotkeyAssigment.Assigments[h] = new HotkeyAssigment(br.ReadInt32(), br.ReadInt32(), br.ReadInt32());

            br.Close();
            stream.Close();
        }
    }
}
