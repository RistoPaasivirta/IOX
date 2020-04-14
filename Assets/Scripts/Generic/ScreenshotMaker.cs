using System.Collections;
using System.IO;
using UnityEngine;

public class ScreenshotMaker : MonoBehaviour
{
    [SerializeField] private int SuperSize = 2;

    private void Update()
    {
        if (Input.GetKeyDown(HotkeyAssigment.ScreenshotKey))
        {
            Directory.CreateDirectory(Wad.BaseFolder + "/Screenshots");

            int i = 0;
            for (; ; i++)
            {
                string path = Wad.BaseFolder + "/Screenshots/Screenshot" + i + ".png";
                if (!File.Exists(path))
                {
                    ScreenCapture.CaptureScreenshot(path, SuperSize);

                    StartCoroutine(Message("Saved screenshot " + i.ToString()));

                    break;
                }
            }
        }
    }

    //used so the message doesn't get saved in the screenshot
    IEnumerator Message(string message)
    {
        yield return new WaitForSeconds(.1f);
        Messaging.GUI.ScreenMessage.Invoke(message, Color.white);
        yield return null;
    }
}
