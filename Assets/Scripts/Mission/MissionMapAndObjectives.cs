using UnityEngine;

public class MissionMapAndObjectives : MonoBehaviour
{
    [SerializeField] private Sprite UIMapSprite = null;
    [SerializeField] private string UIMapName = "";
    [SerializeField] private string UIMapObjectives = "";
    [SerializeField] private Vec2I MapUIBottomLeft = Vec2I.zero;
    [SerializeField] private Vec2I MapUITopRight = Vec2I.zero;

    private void Start()
    {
        Messaging.GUI.UIMapSprite.Invoke(UIMapSprite, MapUIBottomLeft, MapUITopRight);
        Messaging.GUI.UIMapName.Invoke(UIMapName);
        Messaging.GUI.UIMapObjectives.Invoke(UIMapObjectives);
    }
}
