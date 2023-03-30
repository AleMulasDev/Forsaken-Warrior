using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterEvent : MonoBehaviour
{
    [SerializeField] UnityEvent enterTriggerEvents;
    [SerializeField] UnityEvent exitTriggerEvents;
    [SerializeField] private bool shouldTriggerOnce;

    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(_triggered) return;

        if(other.CompareTag("Player"))
        {
            _triggered = true;
            enterTriggerEvents.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_triggered) return;

        if (other.CompareTag("Player"))
        {
            _triggered = true;
            exitTriggerEvents.Invoke();
        }
    }
}
