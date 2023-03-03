using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ECharacterStates { ECS_Inoccupied, ECS_LightAttack, ECS_HeavyAttack, ECS_Dodging, ECS_Jumping }

public enum EEnemyState { EES_Inoccupied, EES_Attack, EES_DrawingWeapon }

public enum EBossMode { EBM_FirstCircle, EBM_SecondCircle, EBM_ThirdCircle, EBM_None }

public enum EWeaponType { EWT_LeftHand, EWT_RightHand, EWT_Both }
