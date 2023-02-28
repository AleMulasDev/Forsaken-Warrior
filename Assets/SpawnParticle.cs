using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;

    public void SpawnParticleAtPosition()
    {
        Destroy(Instantiate(ps, transform.position, ps.transform.rotation), 2f);
    }
}
