using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private bool isParticleSystem;

    private int _damage;
    private GameObject _player;
    private Vector3 _startingPosition;
    private bool _followPlayer = true;
    private void Start()
    {
        _startingPosition = _player.transform.position + new Vector3(0, _player.GetComponent<CharacterController>().height / 2, 0);

        transform.LookAt(_startingPosition);

        StartCoroutine(DestroyProjectileCoroutine());
    }

    private IEnumerator DestroyProjectileCoroutine()
    {
        yield return new WaitForSeconds(10);
        DestroyProjectile();
    }

    private void Update()
    {
        if (isParticleSystem && Vector3.Distance(transform.position, _player.transform.position) > 2f && _followPlayer)
        {
            transform.LookAt(_player.transform.position + new Vector3(0, 1f, 0));
        } else
            _followPlayer = false;

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

        DestroyProjectile();
    }

    public void DestroyProjectile()
    {
        if (isParticleSystem)
        {
            GetComponent<ParticleSystem>().Stop(true);
            Destroy(gameObject, 2f);
        }
        else
            Destroy(gameObject);
    }
}
