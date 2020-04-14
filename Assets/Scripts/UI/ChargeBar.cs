using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChargeBar : MonoBehaviour
{
    //with GUI prefabs you need to keep the reference in the prefab or it will try to access old destroyed instance
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();

        Messaging.Player.Charge.AddListener((c, m) =>
        {
            float f = (float)c / m;
            image.fillAmount = f * .92f + .04f;
        });
    }
}
