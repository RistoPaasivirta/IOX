using UnityEngine;
using UnityEngine.Events;

public class Elevator : MonoBehaviour
{
    [SerializeField] private Vector3 bottom = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 top = new Vector3(0, 4, 0);
    [SerializeField] private float speed = 4;
    [SerializeField] private UnityEvent OnStartRising = new UnityEvent();
    [SerializeField] private UnityEvent OnStopRising = new UnityEvent();
    [SerializeField] private UnityEvent OnStartLowering = new UnityEvent();
    [SerializeField] private UnityEvent OnStopLowering = new UnityEvent();

    State currentState = State.AtTop;

    public enum State
    {
        AtTop,
        Lowering,
        AtBottom,
        Rising
    }

    private void SetToTop()
    {
        transform.localPosition = top;
        currentState = State.AtTop;
    }

    private void SetToBottom()
    {
        transform.localPosition = bottom;
        currentState = State.AtBottom;
    }

    public State CurrentState
    {
        get { return currentState; }
        set
        {
            currentState = value;

            switch (CurrentState)
            {
                default:
                    return;

                case State.AtTop:
                    OnStopRising.Invoke();
                    break;

                case State.AtBottom:
                    OnStopLowering.Invoke();
                    break;

                case State.Lowering:
                    OnStartLowering.Invoke();
                    break;

                case State.Rising:
                    OnStartRising.Invoke();
                    break;

            }
        }
    }

    private void Toggle()
    {
        switch (CurrentState)
        {
            default:
                return;

            case State.AtTop:
                CurrentState = State.Lowering;
                break;

            case State.AtBottom:
                CurrentState = State.Rising;
                break;
        }
    }

    void Update ()
    {
        switch (CurrentState)
        {
            default:
                return;

            case State.Lowering:
                {
                    Vector3 dif = bottom - transform.localPosition;
                    float step = speed * Time.deltaTime;
                    if (dif.magnitude < step)
                    {
                        transform.localPosition = bottom;
                        CurrentState = State.AtBottom;
                        break;
                    }
                    transform.localPosition += dif.normalized * step;
                }
                break;

            case State.Rising:
                {
                    Vector3 dif = top - transform.localPosition;
                    float step = speed * Time.deltaTime;
                    if (dif.magnitude < step)
                    {
                        transform.localPosition = top;
                        CurrentState = State.AtTop;
                        break;
                    }
                    transform.localPosition += dif.normalized * step;
                }
                break;
        }
    }
}
