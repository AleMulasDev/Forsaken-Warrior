using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ECharacterStates { ECS_Inoccupied, ECS_LightAttack, ECS_HeavyAttack, ECS_Dodging, ECS_Jumping, ECS_BackwardJumping }

public enum EEnemyState { EES_Inoccupied, EES_Attack, EES_DrawingWeapon, EES_Spawning, EES_Stunned }

public enum EMinibossMode { EMM_FirstCircle, EMM_SecondCircle, EMM_ThirdCircle, EMM_None }

public enum EBossPhase { EBP_FirstPhase, EBP_SecondPhase, EBP_ThirdPhase, EBP_FourthPhase }
public enum EBossWeaponType { EBWT_Projectile, EBWT_Spell, EBWT_Both }
public enum EBossAttackStage { EBAS_FirstStage, EBAS_SecondStage, EBAS_ThirdStage }

public enum EWeaponType { EWT_LeftHand, EWT_RightHand, EWT_Both }
