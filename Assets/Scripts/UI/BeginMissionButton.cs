using UnityEngine;

[RequireComponent(typeof(GUIButton))]
public class BeginMissionButton : MonoBehaviour
{
    [SerializeField] private AudioClip FailSound = null;

    string TargetLevel = "";

    private void Awake()
    {
        GetComponent<GUIButton>().onClick.AddListener(() => 
        {
            if (string.IsNullOrEmpty(TargetLevel))
            {
                if (FailSound != null)
                    Sounds.Create2DSound(FailSound, Sounds.MixerGroup.Interface, .6f, 1f, 1f, 255);

                return;
            }

            Messaging.Mission.BeginMission.Invoke();
            Messaging.GUI.CloseWindows.Invoke();

            if (LevelLoader.LevelLoaded)
            {
                Messaging.System.SetTimeScale.Invoke(TimeScale.Standard);
                Messaging.GUI.ChangeCursor.Invoke(1);
            }
        });

        Messaging.Mission.SetNextMission.AddListener((s) =>
        {
            TargetLevel = s;

            gameObject.SetActive(!string.IsNullOrEmpty(TargetLevel));
        });

        gameObject.SetActive(false);
    }
}
