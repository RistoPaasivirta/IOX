using UnityEngine;

public class ReferenceDataActivator : MonoBehaviour
{
    [SerializeField] private ReferenceData[] Data = new ReferenceData[0];

    private void Awake()
    {
        foreach (ReferenceData data in Data)
            data.Activate();
    }
}
