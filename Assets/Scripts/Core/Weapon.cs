using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private float damage;

    private PlayerController _playerController;
    private void Start()
    {
        _playerController = GetComponentInParent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(_playerController != null && _playerController.GetOneShotPowerup() && !(other.tag.Equals("Player")))
        {
            other.GetComponent<Health>().Kill();
        }
        else if(other.tag.Equals(targetTag))
        {
            other.GetComponent<Health>().TakeDamage(damage);
        }
    }
}
