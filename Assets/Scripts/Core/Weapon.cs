using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private float damage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals(targetTag))
        {
            other.GetComponent<Health>().TakeDamage(damage);
        }
    }
}
