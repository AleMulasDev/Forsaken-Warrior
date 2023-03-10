using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : AIController
{
    [SerializeField] private float spawnTimer;

    private float bossPhasePercentage = 0;

    public void SpawnEnemy()
    {

    }

    private IEnumerator SpawnCoroutine()
    {
        while(bossPhasePercentage < 33f)
        {
            yield return new WaitForSeconds(spawnTimer);

        }
    }
}
