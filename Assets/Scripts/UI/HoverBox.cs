using UnityEngine;
using UnityEngine.UI;

public class HoverBox : MonoBehaviour
{
    Text text;

    private void Awake()
    {
        text = GetComponentInChildren<Text>();

        Messaging.GUI.HoverBox.AddListener((t) =>
        {
            if (string.IsNullOrEmpty(t))
            {
                text.text = "";
                gameObject.SetActive(false);
                return;
            }

            text.text = t;
            gameObject.SetActive(true);
            transform.position = Input.mousePosition;
        });

        Messaging.GUI.CloseWindows.AddListener(() => 
        {
            text.text = "";
            gameObject.SetActive(false);
        });

        gameObject.SetActive(false);
    }

    private void Update()
    {
        //this works only with canvas mode ScreenSpaceOverlay
        transform.position = Input.mousePosition;

        //this works with all three render modes
        //if you use WorldSpace the EventCamera of the canvas must not be null
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out Vector2 pos);
        //transform.position = myCanvas.transform.TransformPoint(pos);
    }
}
