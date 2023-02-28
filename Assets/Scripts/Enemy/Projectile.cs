using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletDamage;

    private GameObject _player;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        transform.LookAt(_player.transform.position + new Vector3(0, _player.GetComponent<CharacterController>().height/2, 0));
    }

    private void Update()
    {
        _rigidbody.AddForce(transform.forward * bulletSpeed);
    }

    public void SetProjectile(GameObject player)
    {
        _player = player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Player"))
            return;

        _player.GetComponent<Health>().TakeDamage(bulletDamage);
        Destroy(gameObject);
    }
}
