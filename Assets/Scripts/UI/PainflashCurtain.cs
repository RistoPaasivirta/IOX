using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PainflashCurtain : MonoBehaviour
{
    [SerializeField] private float DissipationSpeed = .5f;
    [SerializeField] private float Max = .06f;

    Image image;
    float amount = 0f;

    private void Awake()
    {
        image = GetComponent<Image>();

        Messaging.GUI.Painflash.AddListener((f) =>
        {
            amount = f;
            gameObject.SetActive(true);
        });

        Messaging.System.LevelLoaded.AddListener((i) => 
        {
            amount = 0f;
            gameObject.SetActive(false);
        });
    }

    private void Update()
    {
        amount = Mathf.Clamp01(amount - Time.deltaTime * DissipationSpeed);
        image.color = new Color(image.color.r, image.color.g, image.color.b, amount * Max);
        
        if (amount == 0f)
            gameObject.SetActive(false);
    }
}
