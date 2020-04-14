using System;
using UnityEngine;

public class TwoLegAnimation : MonoBehaviour
{
    [SerializeField] private GameObject LeftLeg = null;
    [SerializeField] private GameObject RightLeg = null;
    [SerializeField] private GameObject LeftFoot = null;
    [SerializeField] private GameObject RightFoot = null;
    [SerializeField] private float ReferenceSpeed = .5f;
    [SerializeField] private Vector2 LegMax = new Vector3(.4f, .25f);
    [SerializeField] private Vector2 LegMin = new Vector3(-.3f, 0f);
    [SerializeField] private Vector2 FootMax = new Vector3(.25f, .15f);
    [SerializeField] private Vector2 FootMin = new Vector3(-.15f, 0f);
    [SerializeField] private float LegPreLead = .2f;

    MonsterCharacter character;
    Vector3 LeftLegOffset;
    Vector3 RightLegOffset;
    Vector3 LeftFootOffset;
    Vector3 RightFootOffset;
    float legCycle = 0;

    private void Awake()
    {
        character = GetComponentInParent<MonsterCharacter>();
        character.OnDeath.AddListener(() => { enabled = false; });

        LeftLegOffset = LeftLeg.transform.localPosition;
        RightLegOffset = RightLeg.transform.localPosition;
        LeftFootOffset = LeftFoot.transform.localPosition;
        RightFootOffset = RightFoot.transform.localPosition;
    }

    private void Update()
    {
        transform.rotation = character.LookDirection;
        WalkCycle();
    }

    private void WalkCycle()
    {
        legCycle += character.physicsBody.velocity.magnitude / ReferenceSpeed * Time.deltaTime;

        if (legCycle > Mathf.PI)
            legCycle -= (Mathf.PI * 2);

        if (legCycle < -Math.PI)
            legCycle += (Mathf.PI * 2);

        float lCycle = (Mathf.Sin(legCycle + LegPreLead) + 1f) * .5f;
        float fCycle = (Mathf.Sin(legCycle) + 1f) * .5f;

        Vector3 leg = Vector3.Lerp(LegMax, LegMin, lCycle);
        Vector3 foot = Vector3.Lerp(FootMax, FootMin, fCycle);

        LeftLeg.transform.localPosition = LeftLegOffset + leg;
        RightLeg.transform.localPosition = RightLegOffset - leg;
        LeftFoot.transform.localPosition = LeftFootOffset + foot;
        RightFoot.transform.localPosition = RightFootOffset - foot;
    }
}
