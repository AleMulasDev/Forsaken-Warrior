using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProp : MonoBehaviour
{
    [SerializeField] private Transform fulcrum;
    [SerializeField] private float delayBetweenShakes;
    [SerializeField] private float delayBetweenRotation;
    [SerializeField] private float startingRadius;
    [SerializeField] private float selfRotationSpeed;
    [SerializeField] private float rotationSpeed;

    [SerializeField] protected Vector3 m_from = new Vector3(0.0F, 45.0F, 0.0F);
    [SerializeField] protected Vector3 m_to = new Vector3(0.0F, -45.0F, 0.0F);
    [SerializeField] protected float m_frequency = 1.0F;

    private Vector3 _startPosition;
    private float _timer;
    private void Start()
    {
        
    }

    private void Update()
    {
        transform.RotateAround(fulcrum.position, Vector3.up, rotationSpeed * Time.deltaTime);
        SelfRotateObject();
    }

    private void SelfRotateObject()
    {
        Quaternion from = Quaternion.Euler(this.m_from);
        Quaternion to = Quaternion.Euler(this.m_to);

        float lerp = 0.5f * (1.0f + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * this.m_frequency));
        this.transform.localRotation = Quaternion.Lerp(from, to, lerp);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            Shake();
    }

    private void Shake()
    {
        StartCoroutine(ShakeObject());
    }

    private IEnumerator ShakeObject()
    {
        _startPosition = transform.position;

        while(startingRadius > 0)
        {
            yield return new WaitForSeconds(delayBetweenShakes);
            startingRadius -= 0.02f;

            if (startingRadius <= 0)
                startingRadius = 0;

            transform.position = (Random.insideUnitSphere * startingRadius) + _startPosition;
        }

        transform.position = new Vector3(transform.position.x, 2f, transform.position.z); 
    }
}
