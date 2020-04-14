using System;
using System.Collections.Generic;
using UnityEngine;

public class MultiArmAnimation : MonoBehaviour
{
    [SerializeField] private List<Arm> Arms = new List<Arm>();

    [Serializable]
    public class Arm
    {
        [SerializeField] private GameObject TargetObject = null;
        [SerializeField] private float IdleAngleDeviation = 10.0f;
        [SerializeField] private float AttackAngle = 40.0f;
        [SerializeField] private float AttackAngleDeviation = 5.0f;
        [SerializeField] private float IdleMaxTime = 2.0f;
        [SerializeField] private float IdleMinTime = 0.5f;
        [SerializeField] private float IdleMaxSpeed = 2.0f;
        [SerializeField] private float IdleMinSpeed = 1.0f;
        [SerializeField] private float AttackMaxTime = 0.2f;
        [SerializeField] private float AttackMinTime = 0.1f;
        [SerializeField] private float AttackMaxSpeed = 6.0f;
        [SerializeField] private float AttackMinSpeed = 4.0f;
        [SerializeField] private float CooldownMaxTime = 0.4f;
        [SerializeField] private float CooldownMinTime = 0.3f;
        [SerializeField] private float CooldownMaxSpeed = 3.0f;
        [SerializeField] private float CooldownMinSpeed = 2.0f;
        [SerializeField] private float MaxDelay = .2f;

        float idleAngle;
        float timer = 0;
        float targetAngle;
        float speed;

        enum State
        {
            Idle,
            WaitingToAttack,
            Attacking,
            CoolingDown
        }

        State currentState = State.Idle;

        public void Init() =>
            idleAngle = TargetObject.transform.localRotation.eulerAngles.z;

        public void Update()
        {
            TargetObject.transform.localRotation = Quaternion.Slerp(TargetObject.transform.localRotation, Quaternion.Euler(0, 0, targetAngle), Time.deltaTime * speed);
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                switch (currentState)
                {
                    default:
                    case State.Idle:
                    case State.CoolingDown:
                    {
                        float value = UnityEngine.Random.value;
                        targetAngle = idleAngle + UnityEngine.Random.Range(-IdleAngleDeviation, +IdleAngleDeviation);
                        timer = Mathf.Lerp(IdleMinTime, IdleMaxTime, value);
                        speed = Mathf.Lerp(IdleMinSpeed, IdleMaxSpeed, 1 - value);
                        break;
                    }

                    case State.WaitingToAttack:
                    {
                        float value = UnityEngine.Random.value;
                        targetAngle = AttackAngle + UnityEngine.Random.Range(-AttackAngleDeviation, +AttackAngleDeviation);
                        timer = Mathf.Lerp(AttackMinTime, AttackMaxTime, value);
                        speed = Mathf.Lerp(AttackMinSpeed, AttackMaxSpeed, 1 - value);
                        currentState = State.Attacking;
                        break;
                    }

                    case State.Attacking:
                    {
                        float value = UnityEngine.Random.value;
                        targetAngle = idleAngle + UnityEngine.Random.Range(-IdleAngleDeviation, +IdleAngleDeviation);
                        timer = Mathf.Lerp(CooldownMinTime, CooldownMaxTime, value);
                        speed = Mathf.Lerp(CooldownMinSpeed, CooldownMaxSpeed, 1 - value);
                        currentState = State.CoolingDown;
                        break;
                    }
                }
            }
        }

        public void Attack(bool instant = false)
        {
            currentState = State.WaitingToAttack;
            timer = instant ? 0f : UnityEngine.Random.value * MaxDelay;
        }
    }

    void Start ()
    {
        foreach (Arm arm in Arms)
            arm.Init();
    }

	void Update ()
    {
        foreach (Arm arm in Arms)
            arm.Update();
    }

    public void Attack()
    {
        foreach(Arm arm in Arms)
            arm.Attack();

        //make one arm attack instantly
        Arms[UnityEngine.Random.Range(0, Arms.Count)].Attack(true);
    }
}
