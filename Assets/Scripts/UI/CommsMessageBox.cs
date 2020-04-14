using UnityEngine;
using UnityEngine.UI;

public class CommsMessageBox : MonoBehaviour
{
    [SerializeField] private int Index = 0;
    [SerializeField] private float NeedleDelay = .2f;
    [SerializeField] private float Cooldown = 2f;
    [SerializeField] private float FadeoutTime = 2f;


    State currentState = State.Writing;
    Image[] images;
    public Image Portrait;
    AudioSource audiosource;
    Text CommsText;
    int needle;
    float timer;
    string currentMessage;

    enum State
    {
        Writing,
        Cooldown,
        Fading
    }

    private void Awake()
    {
        images = GetComponentsInChildren<Image>();
        CommsText = GetComponentInChildren<Text>();
        audiosource = GetComponent<AudioSource>();

        if (CommsText == null)
        {
            Debug.LogError("CommsMessage \"" + gameObject.name + "\": Awake: no Text component found in children");
            return;
        }

        Messaging.GUI.CommsMessage.AddListener((index, portrait, text) =>
        {
            if (index != Index)
                return;

            if (string.IsNullOrEmpty(text))
                return;

            CommsText.text = "";

            gameObject.SetActive(true);
            currentMessage = text.Replace("\\n", "\n");
            timer = 0f;
            needle = 0;
            currentState = State.Writing;

            if (Portrait != null)
                Portrait.sprite = portrait;

            SetAlphas(1f);

            if (audiosource != null)
                audiosource.Play();
        });

        gameObject.SetActive(false);
    }

    private void SetAlphas(float a)
    {
        foreach (Image i in images)
            i.color = new Color(i.color.r, i.color.g, i.color.b, a);

        CommsText.color = new Color(CommsText.color.r, CommsText.color.g, CommsText.color.b, a);
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;

        switch (currentState)
        {
            default:
                break;

            case State.Writing:
                if (timer >= NeedleDelay)
                {
                    timer -= NeedleDelay;
                    CommsText.text = currentMessage.Substring(0, ++needle);

                    if (needle >= currentMessage.Length)
                    {
                        currentState = State.Cooldown;
                        timer = 0f;
                    }
                }
                break;

            case State.Cooldown:
                if (timer >= Cooldown)
                {
                    currentState = State.Fading;
                    timer = 0f;
                }
                break;

            case State.Fading:
                SetAlphas(1f - (timer / FadeoutTime));

                if (timer >= FadeoutTime)
                    gameObject.SetActive(false);
                break;
        }
    }
}
