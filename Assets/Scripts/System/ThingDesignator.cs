using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Designators/Things")]
public class ThingDesignator : ReferenceData
{
    [System.Serializable]
    public struct ThingDesignation
    {
        public string SpawnName;
        public GameObject SpawnObject;
    }

    [SerializeField] private ThingDesignation[] _Designations = new ThingDesignation[0];
    public static Dictionary<string, GameObject> Designations { get; private set; } = new Dictionary<string, GameObject>();

    public override void Activate()
    {
        Designations.Clear();

        foreach (ThingDesignation d in _Designations)
            Designations.Add(d.SpawnName, d.SpawnObject);
    }
}
