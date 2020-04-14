using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlinkImage : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private Color color1 = Color.white;
    [SerializeField] private Color color2 = Color.black;

    float time = 0f;
    Image image;

    void Start() => 
        image = GetComponent<Image>();
    
	void Update ()
    {
        time -= Time.deltaTime * speed;
        if (time <= 0f)
            time += 1f;

        image.color = Color.Lerp(color1, color2, time);
	}
}
