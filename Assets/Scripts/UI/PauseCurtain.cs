using UnityEngine;

public class PauseCurtain : MonoBehaviour
{
    private void Awake()
    {
        Messaging.GUI.PauseCurtain.AddListener(() => 
        {
            gameObject.SetActive(true);
        });

        Messaging.GUI.CloseWindows.AddListener(() => 
        {
            gameObject.SetActive(false);
        });

        gameObject.SetActive(false);
    }
}
