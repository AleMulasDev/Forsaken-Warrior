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
}

public class MinibossController : AIController
{
    [SerializeField] private int maxCircles;
    [SerializeField] CircleWeapon[] weapons;

    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    private EBossMode _bossMode = EBossMode.EBM_None;
    private CircleWeapon _circleWeapon;

    private Weapon _currentLeftHandWeaponInstance;
    private Weapon _currentRightHandWeaponInstance;

    // Update is called once per frame
    void Update()
    {
        if (_bossMode != EBossMode.EBM_None)
            _circleWeapon = weapons[(int)_bossMode];

        if (_enemyState == EEnemyState.EES_Inoccupied)
            RotateToPlayer();

        if (_bossMode == EBossMode.EBM_None)
            return;

        switch (_bossMode)
        {
            case EBossMode.EBM_FirstCircle:
                ShootBehaviour();
                break;
            case EBossMode.EBM_SecondCircle:
                ShootBehaviour();
                break;
            case EBossMode.EBM_ThirdCircle:
                //AttackBehaviour();
                break;
            case EBossMode.EBM_None:
                break;
            default:
                Debug.LogWarning("Error while evaluating the current miniboss circle");
                break;
        }

        _attackTimer += Time.deltaTime;
    }

    private void ShootBehaviour()
    {
        if (_enemyState == EEnemyState.EES_Attack)
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
            default:
                return string.Empty;
        }
    }

    public void SetBossMode(EBossMode bossMode)
    {
        _bossMode = bossMode;
        _animator.SetTrigger(GetCurrentUnsheathTrigger());
    }

    private void WeaponSwitch()
    {
        if (_currentRightHandWeaponInstance != null)
            Destroy(_currentRightHandWeaponInstance.gameObject);
        if (_currentLeftHandWeaponInstance != null)
            Destroy(_currentLeftHandWeaponInstance.gameObject);

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
    }

    private void ShootR()
    {
        _currentRightHandWeaponInstance.Shoot(_circleWeapon.bullet, _playerController);
    }

    private void ShootL()
    {
        _currentLeftHandWeaponInstance.Shoot(_circleWeapon.bullet, _playerController);
    }

    private void Shoot()
    {
        _currentRightHandWeaponInstance.Shoot(_circleWeapon.bullet, _playerController);
    }
}
