using System;
using UnityEngine;

[RequireComponent(typeof(MonsterCharacter))]
public class SpawnOnEvent : MonoBehaviour
{
    [Serializable]
    public struct SpawnObject
    {
        public GameObject spawnObject;
        public Vector3 position;
        public Vector3 rotation;
    }

    [SerializeField] private SpawnObject[] SpawnOnDeath = new SpawnObject[0];
    [SerializeField] private SpawnObject[] SpawnOnDamage = new SpawnObject[0];

    MonsterCharacter character;

    public bool Disabled = false; //hack needed by falling death area 

    private void Start()
    {
        character = GetComponent<MonsterCharacter>();

        if (SpawnOnDeath.Length > 0)
            character.OnDeath.AddListener(() => { OnDeath(); });

        if (SpawnOnDamage.Length > 0)
            character.OnDamage.AddListener((f) => { OnDamage(f); });
    }

    private void OnDeath()
    {
        //hack needed by falling death area 
        if (Disabled)
            return;

        foreach(SpawnObject a in SpawnOnDeath)
        {
            GameObject o = Instantiate(a.spawnObject, LevelLoader.TemporaryObjects);
            o.transform.position = transform.position + a.position;
            o.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + a.rotation);
        }
    }

    private void OnDamage(DamageFrame f)
    {
        if (Disabled)
            return;

        foreach (SpawnObject a in SpawnOnDamage)
        {
            GameObject o = Instantiate(a.spawnObject, LevelLoader.TemporaryObjects);
            o.transform.position = (Vector3)f.hitPosition + a.position;
        }
    }
}
