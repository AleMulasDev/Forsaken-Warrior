using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchState : MonoBehaviour
{
    [SerializeField] private EMinibossMode onEnter;
    [SerializeField] private EMinibossMode onExit;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Player") || other is BoxCollider)
            return;

        MinibossController miniboss = GetComponentInParent<MinibossController>();

        if (miniboss != null && other.gameObject.tag.Equals("Player"))
            miniboss.SetBossMode(onEnter);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.tag.Equals("Player") || other is BoxCollider)
            return;

        MinibossController miniboss = GetComponentInParent<MinibossController>();

        if (miniboss != null && other.gameObject.tag.Equals("Player"))
            miniboss.SetBossMode(onExit);
    }
}
