using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletDamage;

    private GameObject _player;
    private Rigidbody _rigidbody;
    private Vector3 _startingPosition;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _startingPosition = _player.transform.position + new Vector3(0, _player.GetComponent<CharacterController>().height / 2, 0);

        transform.LookAt(_startingPosition);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * bulletSpeed);
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
