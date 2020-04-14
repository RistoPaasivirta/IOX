using UnityEngine;

public class CharacterSelectionGrid : MonoBehaviour
{
    [SerializeField] private GameObject CharacterSelectionButtonPrefab = null;

    private void OnEnable()
    {
        if (CharacterSelectionButtonPrefab == null)
        {
            Debug.LogError("CharacterSelectionGrid: OnEnable: CharacterSelectionButtonPrefab == null");
            return;
        }

        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        for (int i = 0; i < PlayerInfoHolder.LoadedHolders.Count; i++)
        {
            CharacterSelectionButton b = Instantiate(CharacterSelectionButtonPrefab, transform).GetComponentInChildren<CharacterSelectionButton>();
            if (b == null)
                Debug.LogError("CharacterSelectionGrid: OnEnable: no component \"CharacterSelectionButton\" in instances button prefab");
            else
                b.LoadPlayerInfo(i);
        }
    }
}
