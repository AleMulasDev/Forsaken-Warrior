using AmazingAssets.AdvancedDissolve.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProp : MonoBehaviour
{
    [SerializeField] private float delayBetweenShakes;
    [SerializeField] private float startingRadius;
    [SerializeField] private float selfRotationSpeed;
    [SerializeField] private float rotationSpeed;

    [SerializeField] protected Vector3 m_from = new Vector3(0.0F, 45.0F, 0.0F);
    [SerializeField] protected Vector3 m_to = new Vector3(0.0F, -45.0F, 0.0F);
    [SerializeField] protected float m_frequency = 1.0F;

    private Vector3 _startPosition;
    private EEnemyState _enemyState = EEnemyState.EES_Inoccupied; 
    private float _tempStartingRadius;

    private float _selfRotateTimer;
    private bool _isDestroyed = false;
    private BossController _boss;
    private void Start()
    {
        _boss = FindObjectOfType<BossController>();
    }
    private void Update()
    {
        if (_enemyState == EEnemyState.EES_Spawning || _isDestroyed) return;

        _selfRotateTimer += Time.deltaTime;
        transform.RotateAround(transform.parent.parent.position, Vector3.up, rotationSpeed * Time.deltaTime);
        SelfRotateObject();
    }

    public void SetEnemyState(EEnemyState enemyState)
    {
        _enemyState = enemyState;
    }

    private void SelfRotateObject()
    {
        Quaternion from = Quaternion.Euler(this.m_from);
        Quaternion to = Quaternion.Euler(this.m_to);

        float lerp = 0.5f * (1.0f + Mathf.Sin(Mathf.PI * _selfRotateTimer * this.m_frequency));
        this.transform.localRotation = Quaternion.Lerp(from, to, lerp);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword") && !_isDestroyed)
        {
            _isDestroyed = true;
            Shake();
        }
    }

    private void Shake()
    {
        StartCoroutine(ShakeObject());

    }

    private IEnumerator ShakeObject()
    {
        _startPosition = transform.position;
        _tempStartingRadius = startingRadius;

        while (_tempStartingRadius > 0)
        {
            yield return new WaitForSeconds(delayBetweenShakes);
            _tempStartingRadius -= 0.02f;

            if (_tempStartingRadius <= 0)
                _tempStartingRadius = 0;

            transform.position = (Random.insideUnitSphere * _tempStartingRadius) + _startPosition;
        }

        transform.position = new Vector3(transform.position.x, 2f, transform.position.z);
        GetComponent<AnimateCutout>().Dissolve(0f);
        _boss.IncreaseBossPhasePercentage(8.25f);
    }
}
