using UnityEngine;

public abstract class Talent : MonoBehaviour
{
    public abstract void Activate(MonsterCharacter character);
    public abstract void DeActivate(MonsterCharacter character);
    public string Description;
    public string ReferenceName;
}
