using AmazingAssets.AdvancedDissolve.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private bool fixedSpeed;
    [SerializeField] private float incrementalSpeedStep;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float detachDistance;
    [SerializeField] private bool isParticleSystem;

    [SerializeField] private bool isThrowable;
    [SerializeField] private float throwableSpeed;
    [SerializeField] private bool shouldDestroy;
    [SerializeField] private bool shouldDissolve;

    private int _damage;
    private GameObject _player;
    private bool _followPlayer = true;
    private bool _shouldMove = true;


    private void Start()
    {
        StartCoroutine(DestroyProjectileCoroutine());
        transform.LookAt(_player.transform.position + new Vector3(0, 1f, 0));
    }

    private IEnumerator DestroyProjectileCoroutine()
    {
        yield return new WaitForSeconds(10);
        DestroyProjectile();
    }

    private void Update()
    {
        if (!_shouldMove) return;

        if (isParticleSystem && Vector3.Distance(transform.position, _player.transform.position) > detachDistance && _followPlayer)
        {
            transform.LookAt(_player.transform.position + new Vector3(0, 1f, 0));
        } else
            _followPlayer = false;

        transform.Translate(Vector3.forward * Time.deltaTime * GetSpeed());
    }

    public float GetSpeed()
    {
        if (fixedSpeed)
            return bulletSpeed;

        bulletSpeed += incrementalSpeedStep;

        return bulletSpeed;
    }

    public void SetProjectile(GameObject player, int damage)
    {
        _player = player;
        _damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Player") || other is BoxCollider)
            return;

        print(other);

        _player.GetComponent<Health>().TakeDamage(_damage);

        DestroyProjectile();
    }

    public void DestroyProjectile()
    {
        _shouldMove = false;

        if(shouldDissolve)
        {
            GetComponentInChildren<AnimateCutout>().Dissolve(0f);
            GetComponentInChildren<RotationRandomizer>().StopRotating();
            Destroy(gameObject, 2f);
            return;
        }

        if (isParticleSystem && GetComponentInChildren<ParticleSystem>() != null)
        {
            GetComponentInChildren<ParticleSystem>().Stop(true);
            Destroy(gameObject, 2f);
        }
        else if (shouldDestroy)
            Destroy(gameObject);
        else
            GetComponentInChildren<Collider>().isTrigger = false;
    }
}
