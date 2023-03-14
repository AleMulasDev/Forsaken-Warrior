using AmazingAssets.AdvancedDissolve.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : AIController
{
    [SerializeField] private Spawner[] mobSpawners;
    [SerializeField] private BossBar bossBar;
    [SerializeField] private float standDelay;
    [SerializeField] private ParticleSystem groundCrackEffect;

    private EBossPhase _bossPhase = EBossPhase.EBP_FirstPhase;

    private float _spawnTimer = 0;
    private float _timer = 0;
    private float _bossPhasePercentage = 0;
    private int _enemiesToSpawn = 0;
    private bool _introOver = false;
    private List<AIController> spawnedEnemies = new List<AIController>();

    override protected void Start()
    {
        base.Start();
        StartCoroutine(StandCoroutine());
    }

    private IEnumerator StandCoroutine()
    {
        yield return new WaitForSeconds(standDelay);
        _animator.SetTrigger("standUp");
    }

    public void FinishIntro()
    {
        _introOver = true;
        foreach (AnimateCutout ac in GetComponentsInChildren<AnimateCutout>())
            ac.Dissolve(0f);
    }

    private void Update()
    {
        if (!_introOver) return;

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
                ThirdPhase();
                break;
            default:
                Debug.LogWarning("Error while evaluating the current boss phase");
                break;
        }
    }
    
    private void ThirdPhase()
    {
        if (_bossPhasePercentage == 100) return;

        Utils.UIWindowHandler(EUIMode.EUIM_Hide, bossBar.GetComponent<CanvasGroup>());
    }

    private void FirstPhase()
    {
        if (_bossPhasePercentage > 32) return;

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

        if (_bossPhasePercentage == 32)
        {
            IncreaseBossPhasePercentage(1f);
            _spawnTimer = 0;
            _enemiesToSpawn = 0;

            foreach (SimpleEnemyController enemy in spawnedEnemies)
            {
                if (enemy == null) continue;

                enemy.onEnemyBossKill.RemoveAllListeners();
                enemy.GetComponent<Health>().Kill();
            }
            spawnedEnemies.Clear();
            StartCoroutine(EndFirstPhase());
        }
    }

    private IEnumerator EndFirstPhase()
    {
        foreach (AnimateCutout ac in GetComponentsInChildren<AnimateCutout>())
            ac.SpawnEffect();

        yield return new WaitForSeconds(3f);

        _animator.SetTrigger("spawnEnemies");
    }

    public void SpawnEnemies()
    {
        CinemachineShake.instance.ShakeCamera(3.5f, 3.5f);
        ParticleSystem groundCrackInstance = Instantiate(groundCrackEffect, transform.position, groundCrackEffect.transform.rotation);
        groundCrackInstance.transform.position = new Vector3(groundCrackInstance.transform.position.x, 0.01f, groundCrackInstance.transform.position.z);
        Destroy(groundCrackInstance.gameObject, 2f);

        _bossPhase = EBossPhase.EBP_SecondPhase;

        foreach (Spawner s in mobSpawners)
            s.SpawnProp();

        foreach (AnimateCutout ac in GetComponentsInChildren<AnimateCutout>())
            ac.Dissolve(1f);
    }

    public void DestroyProps()
    {
        foreach (Spawner s in mobSpawners)
            s.DestroyProp();
    }

    public void SpawnMiniboss()
    {
        StartCoroutine(SpawnMinibossesCoroutine());
    }

    private IEnumerator SpawnMinibossesCoroutine()
    {
        for (int i = 0; i < mobSpawners.Length; i++)
        {
            if (mobSpawners[i].GetShouldSpawnMiniboss())
                mobSpawners[i].SpawnMiniboss();
            else
                continue;
            yield return new WaitForSeconds(5f);
        }

        _animator.SetTrigger("endCast");

        yield return new WaitForSeconds(2f);

        foreach (AnimateCutout ac in GetComponentsInChildren<AnimateCutout>())
            ac.Dissolve(1f);
    }

    private void SecondPhase()
    {
        if (_bossPhasePercentage > 66) return;

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

        if (_bossPhasePercentage == 66)
        {
            _spawnTimer = 0;
            _enemiesToSpawn = 0;
            IncreaseBossPhasePercentage(1f);
            foreach (SimpleEnemyController enemy in spawnedEnemies)
                enemy.GetComponent<Health>().Kill();
            spawnedEnemies.Clear();
            StartCoroutine(EndSecondPhase());
        }
    }

    private IEnumerator EndSecondPhase()
    {
        foreach (AnimateCutout ac in GetComponentsInChildren<AnimateCutout>())
            ac.SpawnEffect();

        yield return new WaitForSeconds(3f);

        _animator.SetTrigger("spawnMinibosses");

        yield return new WaitForSeconds(1f);

        SpawnMiniboss();
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