using UnityEngine;

public class FactionColor : MonoBehaviour
{
    [SerializeField] private int faction = 0;

    private void Awake() =>
        Messaging.Player.FactionColors.AddListener(() => { UpdateSprites(); });

    private void Start() =>
        UpdateSprites();

    private void UpdateSprites()
    {
        if (faction < 0 || faction >= PlayerInfo.CurrentLocal.FactionColors.Length)
            return;

        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
            sr.color = PlayerInfo.CurrentLocal.FactionColors[faction];
    }
}
