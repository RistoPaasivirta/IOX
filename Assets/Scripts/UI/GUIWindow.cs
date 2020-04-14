using UnityEngine;
using UnityEngine.Events;

public class GUIWindow : MonoBehaviour
{
    [SerializeField] private string GUIWindowName = "Crafting Station";
    [SerializeField] private string secondaryName = "";
    [SerializeField] private bool StartOpen = false;

    public UnityEvent OnOpen = new UnityEvent();
    public UnityEvent OnClose = new UnityEvent();

    private void Awake()
    {
        Messaging.GUI.OpenWindow.AddListener((s) => 
        {
            if (s == GUIWindowName || (!string.IsNullOrEmpty(secondaryName) && s == secondaryName))
            {
                if (!gameObject.activeSelf)
                    OnOpen.Invoke();

                gameObject.SetActive(true);
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
    }

    private void Start()
    {
        gameObject.SetActive(StartOpen);
    }
}
