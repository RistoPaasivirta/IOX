using UnityEngine;
using UnityEngine.UI;

public class PortraitSelectGrid : MonoBehaviour
{
    [SerializeField] private GameObject CellPrefab = null;

    private void Start()
    {
        foreach (Sprite s in PlayerCustomization.PlayerPortraits)
        {
            GameObject g = Instantiate(CellPrefab, transform);
            g.transform.GetChild(0).GetComponent<Image>().sprite = s;
        }
    }
}
