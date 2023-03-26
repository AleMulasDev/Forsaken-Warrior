using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct MinibossWeapon
{
    public EMinibossMode circle;
    public float fireRate;
    public int damage;
    public Weapon rightHandWeapon;
    public Weapon leftHandWeapon;
    public Projectile bullet;
    public EWeaponType weaponType;
}

public class MinibossController : AIController
{
    [SerializeField] private int maxCircles;
    [SerializeField] MinibossWeapon[] weapons;

    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private float spawnTimer;
    [SerializeField] private bool skipUnsheath;

    private EMinibossMode _bossMode = EMinibossMode.EMM_None;
    private MinibossWeapon _circleWeapon;
    private float _spawnTimer;
    private List<Spawner> _enemiesSpawners;
    private List<AIController> _spawnedEnemies = new List<AIController>();

    private Weapon _currentLeftHandWeaponInstance;
    private Weapon _currentRightHandWeaponInstance;

    private bool _shouldSpawnEnemies = false;
    private bool _deadEntities = false;

    private GameObject _pickedInstance;

    protected override void Start()
    {
        base.Start();

        _enemiesSpawners = new List<Spawner>(GetComponentsInChildren<Spawner>());
        _spawnTimer = Mathf.Infinity;
        StartCoroutine(DelaySpawn());
    }

    private IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(1f);
        _shouldSpawnEnemies = true;
    }

    protected override void Die()
    {
        GameManager.Instance.MinibossKilled();
        
        GetComponentInChildren<EnemyHealthBar>().HideHealthBar();

        if(!_deadEntities)
            foreach (AIController enemy in _spawnedEnemies)
                enemy.GetComponent<Health>().Kill();

        _deadEntities = true;

        base.Die();
    }

    public void SetShouldSpawnEnemies(bool shouldSpawn)
    {
        _shouldSpawnEnemies = shouldSpawn;
    }

    // Update is called once per frame
    void Update()
    {
        if (_enemyState == EEnemyState.EES_Spawning) return;

        _animator.SetFloat("bossMode", (int)_bossMode);

        if (_health.IsDead())
        {
            Die();
            return;
        }

        if (_enemyState == EEnemyState.EES_Inoccupied)
            RotateToPlayer();

        switch (_bossMode)
        {
            case EMinibossMode.EMM_FirstCircle:
                ShootBehaviour();
                break;
            case EMinibossMode.EMM_SecondCircle:
                ShootBehaviour();
                break;
            case EMinibossMode.EMM_ThirdCircle:
                AttackBehaviour();
                break;
            case EMinibossMode.EMM_None:
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
        if (!_shouldSpawnEnemies) return;

        if (_spawnTimer > spawnTimer)
        {
            _spawnTimer = 0f;
            _animator.SetTrigger("spawnEnemies");
        }
    }

    private void Spawn()
    {
        foreach (Spawner s in _enemiesSpawners)
        {
            UpdateEnemyList(s.SpawnAllEnemies());
        }
    }

    public void UpdateEnemyList(List<AIController> list)
    {
        foreach (AIController ai in list)
        {
            _spawnedEnemies.Add(ai);
        }
    }

    private void AttackBehaviour()
    {
        if (_enemyState != EEnemyState.EES_Inoccupied)
            return;

        UpdateAnimator();

        if (!_playerController.GetComponent<Health>().IsDead())
            newDestination = _playerController.transform.position;

        if (Vector3.Distance(transform.position, newDestination) > 2f)
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

        if (Vector3.Distance(transform.position, newDestination) <= 2f && _attackTimer > _circleWeapon.fireRate
            && !_playerController.GetComponent<Health>().IsDead())
        {
            _enemyState = EEnemyState.EES_Attack;
            _animator.SetTrigger("attack");
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
            _animator.SetTrigger("attack");
        }
    }

    public void SetBossMode(EMinibossMode bossMode)
    {
        DisableNavMesh();
        _bossMode = bossMode;

        if (_bossMode != EMinibossMode.EMM_None)
            _circleWeapon = weapons[(int)_bossMode];

        if (skipUnsheath)
        {
            WeaponSwitch();
            _enemyState = EEnemyState.EES_Inoccupied;
            _animator.SetTrigger("reset");
        }
        else
            UnsheathWeapon();

        _attackTimer = 0f;

        if (bossMode == EMinibossMode.EMM_ThirdCircle)
            GetComponentInChildren<EnemyHealthBar>().ShowHealthBar();
        else
            GetComponentInChildren<EnemyHealthBar>().HideHealthBar();
    }

    public void UnsheathWeapon()
    {
        _animator.SetTrigger("unsheath");
        _enemyState = EEnemyState.EES_DrawingWeapon;
    }

    private void WeaponSwitch()
    {
        colliders.Clear();

        if (_pickedInstance != null)
            Destroy(_pickedInstance);

        if (_currentRightHandWeaponInstance != null)
            Destroy(_currentRightHandWeaponInstance.gameObject);
        if (_currentLeftHandWeaponInstance != null)
            Destroy(_currentLeftHandWeaponInstance.gameObject);

        if (_bossMode == EMinibossMode.EMM_None) return;

        switch (_circleWeapon.weaponType)
        {
            case EWeaponType.EWT_LeftHand:
                _currentLeftHandWeaponInstance = Instantiate(_circleWeapon.leftHandWeapon, leftHand);
                UpdateWeapon();
                break;
            case EWeaponType.EWT_RightHand:
                _currentRightHandWeaponInstance = Instantiate(_circleWeapon.rightHandWeapon, rightHand);
                UpdateWeapon();
                break;
            case EWeaponType.EWT_Both:
                _currentLeftHandWeaponInstance = Instantiate(_circleWeapon.leftHandWeapon, leftHand);
                _currentRightHandWeaponInstance = Instantiate(_circleWeapon.rightHandWeapon, rightHand);
                UpdateWeapon();
                break;
        }

        if (_currentLeftHandWeaponInstance != null && _currentLeftHandWeaponInstance.TryGetComponent<Collider>(out Collider leftWeaponCollider))
            colliders.Add(leftWeaponCollider);

        if (_currentRightHandWeaponInstance != null && _currentRightHandWeaponInstance.TryGetComponent<Collider>(out Collider rightWeaponCollider))
            colliders.Add(rightWeaponCollider);

    }

    private void SpawnWeapon(bool isRightHanded)
    {
        if (isRightHanded)
            _currentRightHandWeaponInstance = Instantiate(_circleWeapon.rightHandWeapon, rightHand);
        else
            _currentLeftHandWeaponInstance = Instantiate(_circleWeapon.leftHandWeapon, leftHand);
    }

    private void UpdateWeapon()
    {
        _currentRightHandWeaponInstance?.SetDamage(_circleWeapon.damage);
        _currentLeftHandWeaponInstance?.SetDamage(_circleWeapon.damage);
    }

    private void ShootR()
    {
        _enemyState = EEnemyState.EES_Attack;
        _currentRightHandWeaponInstance.Shoot(_circleWeapon.bullet, _playerController);
        AudioManager.Instance.PlaySoundEffect(_audioSource, _currentRightHandWeaponInstance.GetWeaponAudioClip());
    }

    private void ShootL()
    {
        _enemyState = EEnemyState.EES_Attack;
        _currentLeftHandWeaponInstance.Shoot(_circleWeapon.bullet, _playerController);
        AudioManager.Instance.PlaySoundEffect(_audioSource, _currentLeftHandWeaponInstance.GetWeaponAudioClip());


        if (_pickedInstance != null)
            Destroy(_pickedInstance.gameObject);
    }

    private void Shoot()
    {
        _enemyState = EEnemyState.EES_Attack;
        _currentRightHandWeaponInstance.Shoot(_circleWeapon.bullet, _playerController);
        AudioManager.Instance.PlaySoundEffect(_audioSource, _currentRightHandWeaponInstance.GetWeaponAudioClip());
    }

    private void Pick()
    {
        string pickedElement;

        if (_bossMode == EMinibossMode.EMM_FirstCircle)
            pickedElement = "Log";
        else
            pickedElement = "Boulder";

        _pickedInstance = Instantiate(Resources.Load<GameObject>("MBThrowables/" + pickedElement), leftHand);
    }

    public void GroundHit()
    {
        GetComponentInChildren<SpawnParticle>().SpawnParticleAtPosition();
        CinemachineShake.instance.ShakeCamera(5f, 5f);
        AudioManager.Instance.PlaySoundEffect(_audioSource, _currentRightHandWeaponInstance.GetWeaponAudioClip());
    }

}
