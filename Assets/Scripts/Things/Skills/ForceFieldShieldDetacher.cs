using UnityEngine;

[RequireComponent(typeof(ForceFieldController))]
public class ForceFieldShieldDetacher : MonoBehaviour
{
    private void Awake()
    {
        ShieldSkill s = GetComponentInParent<ShieldSkill>();

        if (s == null)
            return;

        s.OnDeactivateInstance.AddListener(() => 
        {
            GetComponent<ForceFieldController>().TargetValue = 0f;
        });
    }
}
