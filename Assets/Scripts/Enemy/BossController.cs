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
    public float teleportDelay;
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

    [SerializeField] private List<BossWeapon> weapons;

    [SerializeField] private Weapon spellbook;
    [SerializeField] private Transform rHand;

    [SerializeField] private AudioClip teleportAudioClip;
    [SerializeField] private AudioClip groundHitAudioClip;

    [SerializeField] private float minibossSpawnDelay;
    
    private EBossPhase _bossPhase = EBossPhase.EBP_FirstPhase;
    private EBossAttackStage _attackStage = EBossAttackStage.EBAS_FirstStage;

    private float _spawnTimer = 0;
    private float _timer = 0;
    private float _teleportTimer = 0;
    private float _bossPhasePercentage = 0;
    private int _enemiesToSpawn = 0;
    private bool _introOver = false;
    private bool _teleported = true;
    private bool _eligibleForTakeDamage = true;
    private bool _needsReset = false;

    private Weapon _spellbookInstance;

    private List<AIController> _spawnedEnemies = new List<AIController>();
    private List<MinibossController> _spawnedMiniboss = new List<MinibossController>();

    private ParticleSystem _lightningStrike;
    private GameObject _targetMarker;

    private Coroutine _infiniteLightningCoroutine;
    private Coroutine _lightningCoroutine;

    private int _minibossIndex = 0;

    override protected void Start()
    {
        base.Start();
        _eligibleForTakeDamage = false;
        StartCoroutine(StandCoroutine());
        _animator.SetFloat("attackSpeed", 0.75f);
        _lightningStrike = Resources.Load<ParticleSystem>("MalignoSpell/LightningStrike");
        _targetMarker = Resources.Load<GameObject>("MalignoSpell/TargetMarker");
    }

    private void Update()
    {
        if (!_introOver) return;

        _timer += Time.deltaTime;
        _attackTimer += Time.deltaTime;
        _teleportTimer += Time.deltaTime;

        if (_enemyState == EEnemyState.EES_Inoccupied)
            RotateToPlayer();

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

        if (_timer > _spawnTimer)
        {
            _timer = 0;
            _spawnTimer += 3;

            if (_enemiesToSpawn < 10)
                _enemiesToSpawn++;

            UpdateEnemyList(mobSpawners[Random.Range(0, mobSpawners.Length)].SpawnEnemies(_enemiesToSpawn));

            foreach (SimpleEnemyController enemy in _spawnedEnemies)
                enemy.onEnemyBossKill.AddListener(() => IncreaseBossPhasePercentage(1f));
        }

        if (_bossPhasePercentage == 32)
        {
            IncreaseBossPhasePercentage(1f);
            _spawnTimer = 0;
            _enemiesToSpawn = 0;

            foreach (SimpleEnemyController enemy in _spawnedEnemies)
            {
                if (enemy == null) continue;

                enemy.onEnemyBossKill.RemoveAllListeners();
                enemy.GetComponent<Health>().Kill();
            }
            _spawnedEnemies.Clear();
            StartCoroutine(EndFirstPhase());
        }
    }

    private IEnumerator EndFirstPhase()
    {
        Appear();

        yield return new WaitForSeconds(3f);

        _animator.SetTrigger("spawnEnemies");
    }

    public void KillAllMiniboss()
    {
        for (int i = 0; i < _spawnedMiniboss.Count; i++)
        {
            if(i == _minibossIndex)
                _spawnedMiniboss[i].GetComponent<Health>().Kill();
        }
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
            foreach (SimpleEnemyController enemy in _spawnedEnemies)
            {
                if(enemy != null)
                    enemy.GetComponent<Health>().Kill();
            }
            _spawnedEnemies.Clear();
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

        _bossPhase = EBossPhase.EBP_ThirdPhase;
    }

    private void ThirdPhase()
    {
        if (_bossPhasePercentage == 100)
        {
            StartCoroutine(EndThirdPhase());
            IncreaseBossPhasePercentage(1f);
        }
    }

    private IEnumerator EndThirdPhase()
    {
        yield return StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Hide, bossBar.GetComponent<CanvasGroup>()));
        yield return StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, bossHealthBar.GetComponent<CanvasGroup>()));

        yield return new WaitForSeconds(1f);

        _bossPhase = EBossPhase.EBP_FourthPhase;
        _attackTimer = 0;
        _teleportTimer = 0;
        Appear();
        _spellbookInstance = Instantiate(spellbook, rHand);
    }

    private void FourthPhase()
    {
        if(_needsReset)
        {
            _needsReset = false;
            _animator.SetTrigger("reset");
            _attackTimer = 0;
            _teleportTimer = 0;
            StartCoroutine(TeleportCoroutine());
            return;
        }

        RotateToPlayer();

        _animator.SetBool("attackMode", true);

        float currentHealth = _health.GetHealth();
        float maxHealth = _health.GetMaxHealth();

        if (currentHealth > (maxHealth * 0.66f))
        {
            FirstAttackStage();
        }
        else if (currentHealth > (maxHealth * 0.33f))
        {
            SecondAttackStage();
        }
        else
        {
            ThirdAttackStage();
        }
    }

    private void ThirdAttackStage()
    {
        AttackBehaviour(EBossAttackStage.EBAS_ThirdStage);

        if (_infiniteLightningCoroutine == null)
        {
            StopCoroutine(_lightningCoroutine);
            _infiniteLightningCoroutine = InfiniteLightnings();
        }
    }

    private void SecondAttackStage()
    {
        AttackBehaviour(EBossAttackStage.EBAS_SecondStage);
    }

    private void AttackBehaviour(EBossAttackStage attackStage)
    {
        _attackStage = attackStage;

        if (_enemyState == EEnemyState.EES_Inoccupied && _attackTimer > weapons[(int)_attackStage].fireRate && _teleported)
        {
            _teleported = false;
            _attackTimer = 0f;
            Attack();
        }
        else if (_teleportTimer > weapons[(int)_attackStage].teleportTime && !_teleported)
        {
            StartCoroutine(TeleportCoroutine());
        }
    }
    private void FirstAttackStage()
    {
        AttackBehaviour(EBossAttackStage.EBAS_FirstStage);
    }

    private void Attack()
    {
        _enemyState = EEnemyState.EES_Attack;


        if (weapons[(int)_attackStage].bossWeaponType == EBossWeaponType.EBWT_Projectile)
        {
            _animator.SetTrigger("shootProjectile");
        }
        else if (weapons[(int)_attackStage].bossWeaponType == EBossWeaponType.EBWT_Spell)
            _animator.SetTrigger("castSpell");
        else
        {
            _animator.SetTrigger("shootProjectile");
        }
    }

    private IEnumerator TeleportCoroutine()
    {
        _eligibleForTakeDamage = false;
        Disappear(0f);
        _teleportTimer = 0f;
        _attackTimer = 0.0f;

        yield return new WaitForSeconds(weapons[(int)_attackStage].teleportDelay);

        Vector3 randomPos = Random.insideUnitCircle * 25;
        Vector3 spawnPosition = new Vector3(randomPos.x + transform.position.x, 0, randomPos.y + transform.position.z);
        transform.position = spawnPosition;
        transform.LookAt(_playerController.transform);
        _teleported = true;
        Appear();
        _playerController.GetComponent<PlayerController>().SetEnemy(transform);
        _eligibleForTakeDamage = true;
    }

    public void Appear()
    {
        AudioManager.Instance.PlaySoundEffect(_audioSource, teleportAudioClip);
        foreach (AnimateCutout ac in GetComponentsInChildren<AnimateCutout>())
            ac.SpawnEffect();

        GetComponent<Collider>().enabled = true;
    }

    public void Disappear(float delay)
    {
        foreach (AnimateCutout ac in GetComponentsInChildren<AnimateCutout>())
            ac.Dissolve(delay);

        GetComponent<Collider>().enabled = false;

    }

    public void SpawnEnemies()
    {
        CinemachineShake.instance.ShakeCamera(5f, 5f);
        ParticleSystem groundCrackInstance = Instantiate(groundCrackEffect, transform.position, groundCrackEffect.transform.rotation);
        groundCrackInstance.transform.position = new Vector3(groundCrackInstance.transform.position.x, 0.01f, groundCrackInstance.transform.position.z);
        AudioManager.Instance.PlaySoundEffect(_audioSource, groundHitAudioClip);
        Destroy(groundCrackInstance.gameObject, 2f);

        _bossPhase = EBossPhase.EBP_SecondPhase;

        foreach (Spawner s in mobSpawners)
            s.SpawnProp();

        Disappear(2f);
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
            {
                var minibossInstance = mobSpawners[i].SpawnMiniboss();
                minibossInstance.onEnemyBossKill.AddListener(() => IncreaseBossPhasePercentage(11f));
                _spawnedMiniboss.Add(minibossInstance);
            }
            else
                continue;

            if (_minibossIndex != 2)
            {
                yield return new WaitForSeconds(minibossSpawnDelay);
                _minibossIndex++;
            }

            
        }

        _animator.SetTrigger("endCast");

        yield return new WaitForSeconds(2f);

        Disappear(0f);
    }

    private IEnumerator StandCoroutine()
    {
        yield return new WaitForSeconds(standDelay);
        StartCoroutine(Utils.UIWindowHandler(EUIMode.EUIM_Show, bossBar.gameObject.GetComponent<CanvasGroup>()));
        _animator.SetTrigger("standUp");
    }

    public void FinishIntro()
    {
        _introOver = true;
        Disappear(0f);
    }

    public void UpdateEnemyList(List<AIController> list)
    {
        foreach (AIController ai in list)
            _spawnedEnemies.Add(ai);
    }

    public void IncreaseBossPhasePercentage(float amount)
    {
        _bossPhasePercentage += amount;
        bossBar.UpdateBossBar(_bossPhasePercentage / 100f);
    }

    private void Shoot()
    {
        _enemyState = EEnemyState.EES_Attack;
        _spellbookInstance.Shoot(bullet, _playerController);
    }

    private void CastSpell()
    {
        _lightningCoroutine = StartCoroutine(CastSpellCoroutine(false, 1f));
        AudioManager.Instance.PlaySoundEffect(_audioSource, groundHitAudioClip);
    }


    private Coroutine InfiniteLightnings()
    {
        _animator.SetFloat("attackSpeed", 1f);
        return StartCoroutine(CastSpellCoroutine(true, 0.75f));
    }

    private IEnumerator CastSpellCoroutine(bool loop, float delay)
    {
        int total = 0;

        while (total < 10 || loop)
        {
            total++;
            SpawnLightning();
            yield return new WaitForSeconds(delay);
        }
    }

    public void SpawnLightning()
    {
        StartCoroutine(LightningCoroutine());
    }

    private IEnumerator LightningCoroutine()
    {
        bool isMoving = _playerController.GetComponent<PlayerController>().GetIsMoving();
        var markerInstance = Instantiate(_targetMarker, _playerController.transform);

        if (!isMoving)
            markerInstance.transform.localPosition += new Vector3(Random.Range(-1f, 1f), 0, 1f);
        else
            markerInstance.transform.localPosition += new Vector3(Random.Range(-1f, 1f), 0, 3f);
        
        markerInstance.transform.parent = null;

        Destroy(markerInstance.gameObject, 0.75f);

        yield return new WaitForSeconds(0.25f);

        Vector3 spawnPos = new Vector3(markerInstance.transform.position.x, 0, markerInstance.transform.position.z);

        Destroy(Instantiate(_lightningStrike, spawnPos, Quaternion.identity).gameObject, 3f);
    }

    public void NeedsReset()
    {
        _needsReset = true;
    }

    public bool EligibleForTakeDamage() { return _eligibleForTakeDamage; }

    public EBossAttackStage GetAttackStage() { return _attackStage; }
}