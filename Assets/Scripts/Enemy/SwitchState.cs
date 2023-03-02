using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchState : MonoBehaviour
{
    [SerializeField] private EBossMode changeTo;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Player"))
            return;

        MinibossController miniboss = GetComponentInParent<MinibossController>();

        if(miniboss != null && other.gameObject.tag.Equals("Player"))
            miniboss.SetBossMode(changeTo);
    }
}
