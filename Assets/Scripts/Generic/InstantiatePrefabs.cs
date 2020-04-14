using UnityEngine;

public class InstantiatePrefabs : MonoBehaviour
{
    [SerializeField] private GameObject[] Prefabs = new GameObject[0];

    private void Awake()
    {
        foreach (GameObject prefab in Prefabs)
            Instantiate(prefab);
    }
}
