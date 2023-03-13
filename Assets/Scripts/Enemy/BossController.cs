using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : AIController
{
    [SerializeField] private Spawner[] mobSpawners;
    [SerializeField] private BossBar bossBar;

    private EBossPhase _bossPhase = EBossPhase.EBP_FirstPhase;

    private float _spawnTimer = 0;
    private float _timer = 0;
    private float _bossPhasePercentage = 0;
    private int _enemiesToSpawn = 0;
    private List<AIController> spawnedEnemies = new List<AIController>();

    override protected void Start()
    {
        base.Start();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        switch (_bossPhase)
        {
            case EBossPhase.EBP_FirstPhase:
                FirstPhase();
                break;
            case EBossPhase.EBP_SecondPhase:
                SecondPhase();
                break;
            case EBossPhase.EBP_ThirdPhase:
                break;
            default:
                Debug.LogWarning("Error while evaluating the current boss phase");
                break;
        }
    }

    private void FirstPhase()
    {
        if(_timer > _spawnTimer)
        {
            _timer = 0;
            _spawnTimer += 3;

            if (_enemiesToSpawn < 10)
                _enemiesToSpawn++;

            UpdateEnemyList(mobSpawners[Random.Range(0, mobSpawners.Length)].SpawnEnemies(_enemiesToSpawn));

            foreach (SimpleEnemyController enemy in spawnedEnemies)
                enemy.onEnemyBossKill.AddListener(() => IncreaseBossPhasePercentage(1f));
        }

        if (_bossPhasePercentage >= 33)
        {
            _bossPhase = EBossPhase.EBP_SecondPhase;
            _spawnTimer = 0;
            _enemiesToSpawn = 0;

            foreach (SimpleEnemyController enemy in spawnedEnemies)
            {
                if (enemy == null) continue;

                enemy.onEnemyBossKill.RemoveAllListeners();
                enemy.GetComponent<Health>().Kill();
            }

            foreach (Spawner s in mobSpawners)
                s.SpawnProp();

            spawnedEnemies.Clear();
        }
    }

    private void SecondPhase()
    {
        if (_timer > _spawnTimer)
        {
            _timer = 0;
            _spawnTimer += 5;

            if (_enemiesToSpawn < 5)
                _enemiesToSpawn++;

            foreach (Spawner s in mobSpawners)
            {
                UpdateEnemyList(s.SpawnEnemies(_enemiesToSpawn));
            }
        }

        if (_bossPhasePercentage >= 66)
        {
            _bossPhase = EBossPhase.EBP_ThirdPhase;
            foreach (SimpleEnemyController enemy in spawnedEnemies)
                enemy.GetComponent<Health>().Kill();
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