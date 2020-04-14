using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Designators/Missions")]
public class MissionDesignator : ReferenceData
{
    [System.Serializable]
    public struct MissionDesignation
    {
        public string ReferenceName;
        public string DisplayName;
        public GameObject DescriptionPrefab;
        public Sprite Layout;
        public string EstimatedTime;
    }

    [SerializeField] private MissionDesignation[] _Designations = new MissionDesignation[0];
    public static Dictionary<string, MissionDesignation> Designations { get; private set; } = new Dictionary<string, MissionDesignation>();

    public override void Activate()
    {
        Designations.Clear();

        foreach (MissionDesignation d in _Designations)
            Designations.Add(d.ReferenceName, d);
    }
}
