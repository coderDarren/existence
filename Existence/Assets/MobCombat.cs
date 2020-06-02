using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCombat : StateMachineBehaviour
{
    public float attackSpeed;
    public float rechargeSpeed;

    private AnimationClip[] clips;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        try {
            clips = animator.runtimeAnimatorController.animationClips;
            
            for(int i=0; i < clips.Length; i++){
                
                if(clips[i].name == "MechSpider_attack")
                    attackSpeed = animator.runtimeAnimatorController.animationClips[i].length;
                if(clips[i].name == "MechSpider_attackRecharge")
                    rechargeSpeed = animator.runtimeAnimatorController.animationClips[i].length;
            }
        } catch (System.Exception _e) {}
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
