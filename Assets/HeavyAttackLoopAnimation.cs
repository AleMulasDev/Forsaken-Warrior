using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeavyAttackLoopAnimation : StateMachineBehaviour
{
    [SerializeField] private InputActionReference heavyAttackControl;
    private static float maxAttackDuration = 1f;
    private float currentAttackDuration;
    private static readonly int IsHeavyAttackEnded = Animator.StringToHash("isHeavyAttackEnded");

    private void OnEnable()
    {
        heavyAttackControl.action.Enable();
    }

    private void OnDisable()
    {
        heavyAttackControl.action.Disable();
    }
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentAttackDuration > maxAttackDuration || heavyAttackControl.action.WasReleasedThisFrame())
        {
            currentAttackDuration = 0;
            animator.SetBool(IsHeavyAttackEnded, true);
        }
        else
        {
            currentAttackDuration += Time.deltaTime;
            animator.SetBool(IsHeavyAttackEnded, false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(IsHeavyAttackEnded, false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
