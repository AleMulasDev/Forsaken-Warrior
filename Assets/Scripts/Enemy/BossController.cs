using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : AIController
{
    [SerializeField] private Spawner mobSpawner;
    [SerializeField] private BossBar bossBar;

    private EBossPhase _bossPhase = EBossPhase.EBP_FirstPhase;

    private float _spawnTimer = 0;
    private float _bossPhasePercentage = 0;
    private int _enemiesToSpawn = 0;

    override protected void Start()
    {
        base.Start();
        InitiateBossPhase();
    }

    private void Update()
    {
        switch(_bossPhase)
        {
            case EBossPhase.EBP_FirstPhase:
                break;
            case EBossPhase.EBP_SecondPhase:
                break;
            case EBossPhase.EBP_ThirdPhase:
                break;
            default:
                Debug.LogWarning("Error while evaluating the current boss phase");
                break;
        }
    }

    public void InitiateBossPhase()
    {
        StartCoroutine(BossPhaseCoroutine());
    }

    private IEnumerator BossPhaseCoroutine()
    {
        List<AIController> spawnedEnemies = new List<AIController>();

        while (_bossPhasePercentage < 33f) // First phase
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

        spawnedEnemies.Clear();
        _spawnTimer = 0;
        _enemiesToSpawn = 0;

        while (_bossPhasePercentage < 66f) // Second phase
        {
            if (_enemiesToSpawn < 10)
                _enemiesToSpawn++;

            _spawnTimer += 3;
            yield return new WaitForSeconds(_spawnTimer);
            spawnedEnemies = mobSpawner.Spawn(_enemiesToSpawn);

            foreach (SimpleEnemyController enemy in spawnedEnemies)
            {
                enemy.onEnemyBossKill.AddListener(HandleEnemyBossKill);
            }
        }
    }

    private void HandleEnemyBossKill()
    {
        _bossPhasePercentage += 1;
        bossBar.UpdateBossBar(_bossPhasePercentage/100f);
    }
}
