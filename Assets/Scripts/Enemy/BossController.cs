using AmazingAssets.AdvancedDissolve.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BossWeapon
{
    public EBossWeaponType bossWeaponType;
    public float fireRate;
    public int damage;
    public float teleportTime;
}

public class BossController : AIController
{
    [SerializeField] private Spawner[] mobSpawners;
    [SerializeField] private BossBar bossBar;
    [SerializeField] private EnemyHealthBar bossHealthBar;
    [SerializeField] private float standDelay;
    [SerializeField] private ParticleSystem groundCrackEffect;
    [SerializeField] private EBossPhase startingPhase;

    [SerializeField] private Projectile bullet;
    [SerializeField] private ParticleSystem spell;

    [SerializeField] private List<BossWeapon> weapons;

    [SerializeField] private Weapon spellbook;

    private EBossPhase _bossPhase = EBossPhase.EBP_FirstPhase;
    private EBossAttackStage _attackStage = EBossAttackStage.EBAS_FirstStage;

    private float _spawnTimer = 0;
    private float _timer = 0;
    private float _teleportTimer = 0;
    private float _bossPhasePercentage = 0;
    private int _enemiesToSpawn = 0;
    private bool _introOver = false;
    private bool _isTeleporting = false;

    private List<AIController> spawnedEnemies = new List<AIController>();

    override protected void Start()
    {
        base.Start();
        _bossPhase = startingPhase;
        Disappear();
        //StartCoroutine(StandCoroutine());
    }

    private void Update()
    {
        //if (!_introOver) return;

        _timer += Time.deltaTime;
        _attackTimer += Time.deltaTime;
        _teleportTimer += Time.deltaTime;

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
            case EBossPhase.EBP_FourthPhase:
                FourthPhase();
                break;
            default:
                Debug.LogWarning("Error while evaluating the current boss phase");
                break;
        }
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
        Appear();

        yield return new WaitForSeconds(3f);

        _animator.SetTrigger("spawnEnemies");
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
        Appear();

        yield return new WaitForSeconds(3f);

        _animator.SetTrigger("spawnMinibosses");

        yield return new WaitForSeconds(1f);

        SpawnMiniboss();
    }

    private void ThirdPhase()
    {
        if (_bossPhasePercentage == 100) return;

        Utils.UIWindowHandler(EUIMode.EUIM_Hide, bossBar.GetComponent<CanvasGroup>());
        Utils.UIWindowHandler(EUIMode.EUIM_Show, bossHealthBar.GetComponent<CanvasGroup>());
    }

    private void FourthPhase()
    {
        RotateToPlayer();

        _animator.SetBool("attackMode", true);

        float currentHealth = _health.GetHealth();
        float maxHealth = _health.GetMaxHealth();

        if(currentHealth > (maxHealth * 0.66f))
        {
            FirstAttackStage();
        } else if (currentHealth > (maxHealth * 0.33f))
        {

        } else
        {

        }
    }

    public void FirstAttackStage()
    {
        _attackStage = EBossAttackStage.EBAS_FirstStage;

        if (_enemyState == EEnemyState.EES_Inoccupied && _attackTimer > weapons[(int)_attackStage].fireRate && !_isTeleporting)
        {
            _attackTimer = 0f;
            StartCoroutine(AttackCoroutine());
        } else if (_teleportTimer > weapons[(int)_attackStage].teleportTime && !_isTeleporting)
        {
            _isTeleporting = true;
            StartCoroutine(TeleportCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        Appear();

        yield return new WaitForSeconds(1);

        _enemyState = EEnemyState.EES_Attack;
        _animator.SetTrigger("shootProjectile");
    }

    private IEnumerator TeleportCoroutine()
    {
        Disappear();

        yield return new WaitForSeconds(1f);

        _teleportTimer = 0f;
        _attackTimer = 0.0f;
        _isTeleporting = false;

        Vector3 randomPos = Random.insideUnitCircle * 25;
        Vector3 spawnPosition = new Vector3(randomPos.x + transform.position.x, 0, randomPos.y + transform.position.z);
        //_navMeshAgent.Warp(spawnPosition);
        //_navMeshAgent.isStopped = true;
        transform.position = spawnPosition;
        transform.LookAt(_playerController.transform);
    }

    public void Appear()
    {
        foreach (AnimateCutout ac in GetComponentsInChildren<AnimateCutout>())
            ac.SpawnEffect();
    }

    public void Disappear()
    {
        foreach (AnimateCutout ac in GetComponentsInChildren<AnimateCutout>())
            ac.Dissolve(0f);
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

        Disappear();
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
                mobSpawners[i].SpawnMiniboss().onEnemyBossKill.AddListener(() => IncreaseBossPhasePercentage(11f));
            else
                continue;
            yield return new WaitForSeconds(5f);
        }

        _animator.SetTrigger("endCast");

        yield return new WaitForSeconds(2f);

        Disappear();
    }

    private IEnumerator StandCoroutine()
    {
        yield return new WaitForSeconds(standDelay);
        _animator.SetTrigger("standUp");
    }

    public void FinishIntro()
    {
        _introOver = true;

        Disappear();
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

    private void ShootL()
    {
        _enemyState = EEnemyState.EES_Attack;
        spellbook.Shoot(bullet, _playerController);
    }
}