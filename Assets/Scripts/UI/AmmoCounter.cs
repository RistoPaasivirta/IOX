using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class AmmoCounter : MonoBehaviour
{
    [SerializeField] private int index = 0;

    //with GUI prefabs you need to keep the reference in the prefab or it will try to access old destroyed instance
    Text text;

    private void Awake()
    {
        text = GetComponent<Text>();

        Messaging.Player.Ammo.AddListener((i, a, m) =>
        {
            if (i != index)
                return;

            if (m == -1)
                text.text = "unlimited";
            else
                text.text = a + "/" + m;
        });
    }
}
