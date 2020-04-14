using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private CursorTexture[] CursorTextures = new CursorTexture[0];

    int CurrentCursor;

    [System.Serializable]
    public struct CursorTexture
    {
        public Texture2D texture;
        public bool Center;
    }

    private void Awake()
    {
        if (CursorTextures.Length < 2)
            Debug.LogWarning("CursorController: Awake: Too few cursor textures (need 2)");

        Messaging.GUI.ChangeCursor.AddListener((i) => { ChangeCursor(i); });
    }

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

    private void OnGUI()
    {
        if (PlayerInfo.CurrentLocal.CursorItem == null)
            return;

        if (!LevelLoader.LevelLoaded || !PlayerInfo.CurrentLocal.PlayerHasBeenInitialized)
            return;

        Vector2 m = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

        float w = PlayerInfo.CurrentLocal.CursorItem.CursorIcon.texture.width;
        float h = PlayerInfo.CurrentLocal.CursorItem.CursorIcon.texture.height;

        //variable cursor item size
        //GUI.DrawTexture(new Rect(m.x - (w / 2), m.y - (h / 2), w, h), PlayerInfo.CurrentLocal.CursorItem.CursorIcon.texture);
        
        //fixed cursor item size
        GUI.DrawTexture(new Rect(m.x - 32, m.y - 32, 64, 64), PlayerInfo.CurrentLocal.CursorItem.CursorIcon.texture);
    }
}
