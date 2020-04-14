public enum SaveObjectType
{
    Monster = 0,
    Projectile = 1,
    Skill = 2,
    LevelObject = 3,
    Pickup
}

public interface SaveGameObject
{
    byte[] Serialize();
    void Deserialize(byte[] data);
    int SaveIndex { get; set; }
    string SpawnName { get; set; }
    SaveObjectType ObjectType { get; }
    void AfterCreated();
}
