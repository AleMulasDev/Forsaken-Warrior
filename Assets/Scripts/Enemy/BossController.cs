using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : AIController
{
    [SerializeField] private Spawner mobSpawner;
    [SerializeField] private BossBar bossBar;

    private float _spawnTimer = 0;
    private float _bossPhasePercentage = 0;
    private int _enemiesToSpawn = 0;

    public void SpawnEnemy()
    {
        StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine()
    {
        List<AIController> spawnedEnemies = new List<AIController>();

        while (_bossPhasePercentage < 33f)
        {
            if (_enemiesToSpawn < 10)
                _enemiesToSpawn++;

            _spawnTimer += 3;
            yield return new WaitForSeconds(_spawnTimer);
            spawnedEnemies = mobSpawner.Spawn(_enemiesToSpawn);

            foreach(SimpleEnemyController enemy in spawnedEnemies)
            {
                enemy.onEnemyBossKill.AddListener(HandleEnemyBossKill);
            }
        }
    }

    private void HandleEnemyBossKill()
    {
        print("ok");
        _bossPhasePercentage += 1;
        bossBar.UpdateBossBar(_bossPhasePercentage/100f);
    }
}
