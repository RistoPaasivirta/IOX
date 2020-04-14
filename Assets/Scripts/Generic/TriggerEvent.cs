using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent onPlayerEnter = new UnityEvent();

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerControls>() != null)
            onPlayerEnter.Invoke();
    }
}

