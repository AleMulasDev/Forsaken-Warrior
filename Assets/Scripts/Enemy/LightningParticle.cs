using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightningParticle : MonoBehaviour
{
    [SerializeField] private float onEnableTime;
    [SerializeField] private float onDisableTime;

    private int damage;

    private void Start()
    {
        SetDamage(1);
        StartCoroutine(SwitchCollider(true, onEnableTime));
        StartCoroutine(SwitchCollider(false, onDisableTime));
    }

    private IEnumerator SwitchCollider(bool enabled, float time)
    {
        yield return new WaitForSeconds(time);

        GetComponent<Collider>().enabled = enabled;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && other is CharacterController)
        {
            other.GetComponent<Health>().TakeDamage(damage);
        }
    }
}
