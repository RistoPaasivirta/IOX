using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider = null;
    [SerializeField] private float Speed = .4f;

    //float timer;
    //public float Cooldown = 4f;
    //public float Fadeout = 5f;
    float targetValue;
    CanvasGroup cg;

    private void Awake()
    {
        if (slider == null)
        {
            Debug.LogError("BossHealthBar: Awake: slider reference not set");
            return;
        }

        cg = GetComponent<CanvasGroup>();

        Messaging.Mission.BossHealth.AddListener((f) => 
        {
            if (f == 0f)
            {
                gameObject.SetActive(false);
                return;
            }

            //timer = 0f;
            gameObject.SetActive(true);
            targetValue = f;
            cg.alpha = 1f;
        });

        slider.SetValueWithoutNotify(0);
        cg.alpha = 0f;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        slider.SetValueWithoutNotify(Mathf.Lerp(slider.value, targetValue, Time.deltaTime * Speed));

        /*timer += Time.deltaTime;
        if (timer >= Cooldown)
        {
            cg.alpha = 1f - ((timer - Cooldown) / Fadeout);
            if (cg.alpha <= 0f)
                gameObject.SetActive(false);
        }*/
    }
}
