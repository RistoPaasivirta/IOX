using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class OverCurtain : MonoBehaviour
{
    [SerializeField] private float Speed = .75f;

    Image image;
    float alpha = 1.25f;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.color = new Color(0, 0, 0, Mathf.Clamp01(alpha));

        Activate();
    }

    private void OnEnable() =>
        Activate();

    private void Activate()
    {
        alpha = 1.25f;
        image.color = new Color(0, 0, 0, 1);
        
        //gameObject.SetActive(true);
    }

    private void Update()
    {
        if (alpha > 0)
        {
            alpha -= Time.deltaTime * Speed;
            image.color = new Color(0, 0, 0, Mathf.Clamp01(alpha));
        }
        //else
            //gameObject.SetActive(false);
    }
}
