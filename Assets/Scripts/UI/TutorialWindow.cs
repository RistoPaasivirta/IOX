using UnityEngine;
using UnityEngine.Events;

public class TutorialWindow : MonoBehaviour
{
    [SerializeField] private string GUIWindowName = "Crafting Station";
    [SerializeField] private string secondaryName = "";
    [SerializeField] private UnityEvent OnOpen = new UnityEvent();
    [SerializeField] private UnityEvent OnClose = new UnityEvent();

    bool once = false;

    private void Awake()
    {
        Messaging.GUI.OpenWindow.AddListener((s) => 
        {
            if (!Options.Tutorials)
                return;

            if (once)
            {
                gameObject.SetActive(false);
                return;
            }

            if (s == GUIWindowName || (!string.IsNullOrEmpty(secondaryName) && s == secondaryName))
            {
                if (!gameObject.activeSelf)
                    OnOpen.Invoke();

                gameObject.SetActive(true);
                once = true;
            }
            else
            {
                if (gameObject.activeSelf)
                    OnClose.Invoke();

                gameObject.SetActive(false);
            }
        });

        Messaging.GUI.CloseWindow.AddListener((s) =>
        {
            if (s == GUIWindowName)
            {
                if (gameObject.activeSelf)
                    OnClose.Invoke();

                gameObject.SetActive(false);
            }
        });

        Messaging.GUI.CloseWindows.AddListener(() => 
        {
            if (gameObject.activeSelf)
                OnClose.Invoke();

            gameObject.SetActive(false);
        });

        gameObject.SetActive(false);
    }
}
