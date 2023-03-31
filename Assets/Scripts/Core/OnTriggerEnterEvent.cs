using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterEvent : MonoBehaviour
{
    [SerializeField] UnityEvent enterTriggerEvents;
    [SerializeField] UnityEvent exitTriggerEvents;
    [SerializeField] private bool shouldTriggerOnce;

    private bool _enterTriggered = false;
    private bool _exitTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if(_enterTriggered) return;

        if(other.CompareTag("Player"))
        {
            if(shouldTriggerOnce)
                _exitTriggered = true;

            enterTriggerEvents.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_exitTriggered) return;

        if (other.CompareTag("Player"))
        {
            if (shouldTriggerOnce)
                _exitTriggered = true;

            exitTriggerEvents.Invoke();
        }
    }
}
