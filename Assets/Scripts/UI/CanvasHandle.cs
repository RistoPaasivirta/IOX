using UnityEngine;

public class CanvasHandle : MonoBehaviour
{
    [SerializeField] private string CanvasName = "GameCanvas";
    [SerializeField] private bool StartOpen = true;

    private void Awake()
    {
        Messaging.GUI.OpenCanvas.AddListener((s) => 
        {
            gameObject.SetActive(s == CanvasName);
        });
    }

    private void Start() =>
        gameObject.SetActive(StartOpen);
}
