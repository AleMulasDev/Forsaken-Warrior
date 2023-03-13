using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : AIController
{
    [SerializeField] private Spawner[] mobSpawners;
    [SerializeField] private BossBar bossBar;

    private EBossPhase _bossPhase = EBossPhase.EBP_FirstPhase;

    private float _spawnTimer = 0;
    private float _bossPhasePercentage = 0;
    private int _enemiesToSpawn = 0;
    private List<AIController> spawnedEnemies;

    override protected void Start()
    {
        base.Start();
        InitiateBossPhase();
    }

    private void Update()
    {
        switch (_bossPhase)
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

        if (_bossPhase == EBossPhase.EBP_SecondPhase && _bossPhasePercentage >= 66)
        {
            _bossPhase = EBossPhase.EBP_ThirdPhase;
            foreach (AIController ai in spawnedEnemies)
                ai.GetComponent<Health>().Kill();
        }
    }

    public void InitiateBossPhase()
    {
        StartCoroutine(BossPhaseCoroutine());
    }

    private IEnumerator BossPhaseCoroutine()
    {
        spawnedEnemies = new List<AIController>();
        _bossPhase = EBossPhase.EBP_FirstPhase;
        while (_bossPhasePercentage < 33f) // First phase
        {
            if (_enemiesToSpawn < 10)
                _enemiesToSpawn++;

            _spawnTimer += 2;
            yield return new WaitForSeconds(_spawnTimer);
            spawnedEnemies = mobSpawners[Random.Range(0, mobSpawners.Length)].SpawnEnemies(_enemiesToSpawn);

            foreach (SimpleEnemyController enemy in spawnedEnemies)
            {
                enemy.onEnemyBossKill.AddListener(() => IncreaseBossPhasePercentage(1f));
            }
        }

        //IncreaseBossPhasePercentage(33);

        _bossPhase = EBossPhase.EBP_SecondPhase;
        _spawnTimer = 10;
        _enemiesToSpawn = 0;
        spawnedEnemies.Clear();

        foreach (Spawner s in mobSpawners)
            s.SpawnProp();

        while (_bossPhasePercentage < 66f) // Second phase
        {
            if (_enemiesToSpawn < 5)
                _enemiesToSpawn++;

            foreach (Spawner s in mobSpawners)
            {
                UpdateEnemyList(s.SpawnEnemies(_enemiesToSpawn));
            }
            yield return new WaitForSeconds(_spawnTimer);
        }
    }

    public void UpdateEnemyList(List<AIController> list)
    {
        foreach (AIController ai in list)
            spawnedEnemies.Add(ai);
    }

    public void IncreaseBossPhasePercentage(float amount)
    {
        _bossPhasePercentage += amount;
        bossBar.UpdateBossBar(_bossPhasePercentage / 100f);
    }
}
