using UnityEngine;

public class PlayerMapPositionMarker : MonoBehaviour
{
    Vec2I BottomLeft;
    Vec2I TopRight;
    RectTransform rt;

    private void Awake()
    {
        rt = transform as RectTransform;

        Messaging.GUI.UIMapSprite.AddListener((_, bl, tr) =>
        {
            BottomLeft = bl;
            TopRight = tr;
        });

        Messaging.Player.PlayerGridPosition.AddListener((p) =>
        {
            Vector2 world = TheGrid.WorldPosition(p);
            Vector2 map;

            map.x = Mathf.InverseLerp(BottomLeft.x, TopRight.x, world.x);
            map.y = Mathf.InverseLerp(BottomLeft.y, TopRight.y, world.y);

            rt.anchorMin = map;
            rt.anchorMax = map;
        });
    }
}
