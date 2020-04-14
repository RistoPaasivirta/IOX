using UnityEngine;

public class CursorControllerSimple : MonoBehaviour
{
    [SerializeField] private CursorTexture[] CursorTextures = new CursorTexture[0];

    int CurrentCursor;

    [System.Serializable]
    public struct CursorTexture
    {
        public Texture2D texture;
        public bool Center;
    }

    private void Awake() =>
        Messaging.GUI.ChangeCursor.AddListener((i) => { ChangeCursor(i); });

    private void Start() =>
        ChangeCursor(CurrentCursor);

    private void ChangeCursor(int number)
    {
        if (number < 0 || number >= CursorTextures.Length) return;

        CurrentCursor = number;

        CursorTexture c = CursorTextures[number];
        Vector2 hotspot = c.Center ? new Vector2(c.texture.width, c.texture.height) * .5f : Vector2.zero;
        Cursor.SetCursor(c.texture, hotspot, CursorMode.Auto);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
            ChangeCursor(CurrentCursor);
    }
}
