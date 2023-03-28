using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterEvent : MonoBehaviour
{
    [SerializeField] UnityEvent enterTriggerEvents;
    [SerializeField] UnityEvent exitTriggerEvents;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            enterTriggerEvents.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            exitTriggerEvents.Invoke();
        }
    }
}
