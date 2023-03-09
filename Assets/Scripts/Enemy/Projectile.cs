using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;

    private int _damage;
    private GameObject _player;
    private Vector3 _startingPosition;
    private void Start()
    {

        _startingPosition = _player.transform.position + new Vector3(0, _player.GetComponent<CharacterController>().height / 2, 0);

        transform.LookAt(_startingPosition);

        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * bulletSpeed);
    }

    public void SetProjectile(GameObject player, int damage)
    {
        _player = player;
        _damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Player"))
            return;

        _player.GetComponent<Health>().TakeDamage(_damage);
        Destroy(gameObject);
    }
}
