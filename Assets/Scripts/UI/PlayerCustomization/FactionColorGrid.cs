using UnityEngine;
using UnityEngine.UI;

public class FactionColorGrid : MonoBehaviour
{
    [SerializeField] private GameObject CellPrefab = null;

    private void Start()
    {
        foreach (Color c in PlayerCustomization.FactionColors)
        {
            GameObject g = Instantiate(CellPrefab, transform);
            g.transform.GetChild(0).GetComponent<Image>().color = c;
        }
    }
}
