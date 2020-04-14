using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class OnColliderEvent : MonoBehaviour
{
    [SerializeField] private bool OnlyPlayer = true;
    [SerializeField] private UnityEvent OnTriggerEnter = new UnityEvent();
    [SerializeField] private UnityEvent OnTriggerExit = new UnityEvent();
    [SerializeField] private bool OneUse = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnlyPlayer)
            if (collision.GetComponent<PlayerControls>() == null)
                return;

        OnTriggerEnter.Invoke();

        if (OneUse)
            Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (OnlyPlayer)
            if (collision.GetComponent<PlayerControls>() == null)
                return;

        OnTriggerExit.Invoke();

        if (OneUse)
            Destroy(gameObject);
    }
}
