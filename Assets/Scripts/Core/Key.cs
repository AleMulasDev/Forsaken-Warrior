using AmazingAssets.AdvancedDissolve.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private ParticleSystem onPick;
    [SerializeField] float sinMultiplier;
    [SerializeField] float speed;

    private bool _picked = false;
    private Vector3 _startingPosition;
    private void Start()
    {
        _startingPosition = transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_picked) return;

        if(other.CompareTag("Player"))
        {
            _picked = true;
            GameManager.Instance.AddKey();
            GetComponentInChildren<AnimateCutout>().Dissolve(0f);
            foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
                ps.Stop(true);
            GetComponent<AudioSource>().Stop();
            Destroy(gameObject, 6f);
        }
    }

    private void Update()
    {
        if(_picked) return;

        transform.position = _startingPosition + new Vector3(0.0f, Mathf.Sin(Time.time * speed) * sinMultiplier, 0.0f);
    }
}
