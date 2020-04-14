using UnityEngine;
using UnityEngine.UI;

public class BadgeSelectGrid : MonoBehaviour
{
    [SerializeField] private GameObject CellPrefab = null;

    private void Start()
    {
        foreach (Sprite s in PlayerCustomization.PlayerBadges)
        {
            GameObject g = Instantiate(CellPrefab, transform);
            g.transform.GetChild(0).GetComponent<Image>().sprite = s;
        }
    }
}
