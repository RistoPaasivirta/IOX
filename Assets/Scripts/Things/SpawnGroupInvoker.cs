using UnityEngine;

public class SpawnGroupInvoker : MonoBehaviour
{
    public void InvokeSpawnGroup(int group) =>
        Messaging.System.SpawnLevelObjects.Invoke(group);
}
