using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct CircleWeapon
{
    public EBossMode circle;
    public float fireRate;
    public float damage;
    public Weapon rightHandWeapon;
    public Weapon leftHandWeapon;
    public Projectile bullet;
    public EWeaponType weaponType;
    public Collider collider;
}

public class MinibossController : AIController
{
    [SerializeField] private int maxCircles;
    [SerializeField] CircleWeapon[] weapons;
    

    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private float spawnTimer;

    private EBossMode _bossMode = EBossMode.EBM_None;
    private CircleWeapon _circleWeapon;
    private float _spawnTimer;
    private List<Spawner> enemiesSpawners;

    private Weapon _currentLeftHandWeaponInstance;
    private Weapon _currentRightHandWeaponInstance;

    protected override void Start()
    {
        base.Start();

        enemiesSpawners = new List<Spawner>(GetComponentsInChildren<Spawner>());
        _spawnTimer = Mathf.Infinity;
    }

    protected override void Die()
    {
        GetComponentInChildren<MinibossHealthBar>().HideHealthBar();
        base.Die();
    }

    // Update is called once per frame
    void Update()
    {
        if (_health.IsDead())
        {
            Die();
            return;
        }

        if (_bossMode != EBossMode.EBM_None)
            _circleWeapon = weapons[(int)_bossMode];

        if (_enemyState == EEnemyState.EES_Inoccupied)
            RotateToPlayer();

        switch (_bossMode)
        {
            case EBossMode.EBM_FirstCircle:
                ShootBehaviour();
                break;
            case EBossMode.EBM_SecondCircle:
                ShootBehaviour();
                break;
            case EBossMode.EBM_ThirdCircle:
                AttackBehaviour();
                break;
            case EBossMode.EBM_None:
                SpawnEnemies();
                break;
            default:
                Debug.LogWarning("Error while evaluating the current miniboss circle");
                break;
        }

        _attackTimer += Time.deltaTime;
        _spawnTimer += Time.deltaTime;
    }

    private void SpawnEnemies()
    {
        if (_spawnTimer > spawnTimer)
        {
            _spawnTimer = 0f;
            _animator.SetTrigger("spawnEnemies");
        }
    }

    private void Spawn()
    {
        foreach(Spawner s in enemiesSpawners)
            s.SpawnAll();
    }

    private void AttackBehaviour()
    {
        if (_enemyState != EEnemyState.EES_Inoccupied)
            return;

        UpdateAnimator();

        if (!_playerController.GetComponent<Health>().IsDead())
            newDestination = _playerController.transform.position;

        if (Vector3.Distance(transform.position, newDestination) > 1.5f)
        {
            EnableNavMesh();
        }
        else
        {
            Attack();
        }
    }

    private void Attack()
    {
        DisableNavMesh();

        if (Vector3.Distance(transform.position, newDestination) <= 1.5f && _attackTimer > _circleWeapon.fireRate
            && !_playerController.GetComponent<Health>().IsDead())
        {
            _enemyState = EEnemyState.EES_Attack;
            _animator.SetTrigger(GetCurrentTrigger());
        }
    }

    private void ShootBehaviour()
    {
        if (_enemyState != EEnemyState.EES_Inoccupied)
            return;

        DisableNavMesh();

        if (_attackTimer > _circleWeapon.fireRate && !_playerController.GetComponent<Health>().IsDead())
        {
            _enemyState = EEnemyState.EES_Attack;
            _animator.SetTrigger(GetCurrentTrigger());
        }
    }

    private string GetCurrentTrigger()
    {
        switch (_bossMode)
        {
            case EBossMode.EBM_FirstCircle:
                return "firstCircleAttack";
            case EBossMode.EBM_SecondCircle:
                return "secondCircleAttack";
            case EBossMode.EBM_ThirdCircle:
                return "thirdCircleAttack";
            default:
                return string.Empty;
        }
    }

    private string GetCurrentUnsheathTrigger()
    {
        switch (_bossMode)
        {
            case EBossMode.EBM_FirstCircle:
                return "firstCircleUnsheath";
            case EBossMode.EBM_SecondCircle:
                return "secondCircleUnsheath";
            case EBossMode.EBM_ThirdCircle:
                return "thirdCircleUnsheath";
            case EBossMode.EBM_None:
                return "unequip";
            default:
                return string.Empty;
        }
    }

    public void SetBossMode(EBossMode bossMode)
    {
        DisableNavMesh();
        _bossMode = bossMode;
        _animator.SetTrigger(GetCurrentUnsheathTrigger());
        _enemyState = EEnemyState.EES_DrawingWeapon;

        if (bossMode == EBossMode.EBM_ThirdCircle)
            GetComponentInChildren<MinibossHealthBar>().ShowHealthBar();
        else
            GetComponentInChildren<MinibossHealthBar>().HideHealthBar();
    }

    private void WeaponSwitch()
    {
        if (_currentRightHandWeaponInstance != null)
            Destroy(_currentRightHandWeaponInstance.gameObject);
        if (_currentLeftHandWeaponInstance != null)
            Destroy(_currentLeftHandWeaponInstance.gameObject);

        if (_bossMode == EBossMode.EBM_None) return;

        switch (_circleWeapon.weaponType)
        {
            case EWeaponType.EWT_LeftHand:
                _currentLeftHandWeaponInstance = Instantiate(_circleWeapon.leftHandWeapon, leftHand);
                break;
            case EWeaponType.EWT_RightHand:
                _currentRightHandWeaponInstance = Instantiate(_circleWeapon.rightHandWeapon, rightHand);
                break;
            case EWeaponType.EWT_Both:
                _currentLeftHandWeaponInstance = Instantiate(_circleWeapon.leftHandWeapon, leftHand);
                _currentRightHandWeaponInstance = Instantiate(_circleWeapon.rightHandWeapon, rightHand);
                break;
        }

        if (_currentLeftHandWeaponInstance != null && _currentLeftHandWeaponInstance.TryGetComponent<Collider>(out Collider leftWeaponCollider))
            colliders.Add(leftWeaponCollider);

        if (_currentRightHandWeaponInstance != null && _currentRightHandWeaponInstance.TryGetComponent<Collider>(out Collider rightWeaponCollider))
            colliders.Add(rightWeaponCollider);

    }

    private void ShootR()
    {
        _enemyState = EEnemyState.EES_Attack;
        _currentRightHandWeaponInstance.Shoot(_circleWeapon.bullet, _playerController);
    }

    private void ShootL()
    {
        _enemyState = EEnemyState.EES_Attack;
        _currentLeftHandWeaponInstance.Shoot(_circleWeapon.bullet, _playerController);
    }

    private void Shoot()
    {
        _enemyState = EEnemyState.EES_Attack;
        _currentRightHandWeaponInstance.Shoot(_circleWeapon.bullet, _playerController);
    }
}
