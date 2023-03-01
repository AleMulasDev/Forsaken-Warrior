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
}

public class MinibossController : AIController
{
    [SerializeField] private int maxCircles;
    [SerializeField] CircleWeapon[] weapons;

    private EBossMode _bossMode = EBossMode.EBM_None;
    private CircleWeapon _circleWeapon;

    // Update is called once per frame
    override protected void Update()
    {
        if (_bossMode == EBossMode.EBM_None)
            return;

        _circleWeapon = weapons[(int)EBossMode.EBM_FirstCircle];

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
        switch(_bossMode)
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

    public void SetBossMode(EBossMode bossMode)
    {
        print("Switched boss mode to: " + bossMode);
        _bossMode = bossMode;
    }
}
