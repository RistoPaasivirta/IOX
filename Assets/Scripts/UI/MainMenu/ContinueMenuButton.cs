using UnityEngine;

[RequireComponent(typeof(SimpleButton))]
public class ContinueMenuButton : MonoBehaviour
{
    [SerializeField] private string CheckWindowName = "Main Buttons Menu";
    [SerializeField] private string OpenWindowName = "Continue Window";
    [SerializeField] private GameObject DisabledImage = null;
    [SerializeField] private AudioClip ClickSound = null;
    [SerializeField] private AudioClip DisabledSound = null;

    SimpleButton button;

    private void Awake()
    {
        button = GetComponent<SimpleButton>();

        Messaging.GUI.OpenWindow.AddListener((s) => 
        {
            if (s == CheckWindowName)
                Refresh();
        });
       
        button.OnClick.AddListener(() => 
        {
            if (PlayerInfoHolder.LoadedHolders.Count > 0)
            {
                if (ClickSound != null)
                    Sounds.Create2DSound(ClickSound, Sounds.MixerGroup.Interface, .2f, 1f, 1f, 255);

                Messaging.GUI.OpenWindow.Invoke(OpenWindowName);
            }
            else if (DisabledSound != null)
                Sounds.Create2DSound(DisabledSound, Sounds.MixerGroup.Interface, .4f, 1f, 1f, 0);
        });
    }

    private void OnEnable() => 
        Refresh();

    private void Refresh() =>
        DisabledImage.SetActive(PlayerInfoHolder.LoadedHolders.Count == 0);
}
