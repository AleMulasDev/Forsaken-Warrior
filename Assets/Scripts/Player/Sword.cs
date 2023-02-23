using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private BoxCollider _boxCollider;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        print(other.tag);

        if (other.tag.Equals("Enemy"))
            other.GetComponent<Health>().TakeDamage(20f);
    }

    public void EnableBox() { _boxCollider.enabled = true; }
    public void DisableBox() { _boxCollider.enabled = false; }
}
