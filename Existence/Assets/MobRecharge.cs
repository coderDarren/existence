using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobRecharge : StateMachineBehaviour
{
    private string startName;
    private string rechargeName;
    private float rechargeSpeed;
    private AnimationEvent rechargeEnd;
    private AnimationClip[] clips;
    private AnimationClip currentClip;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        startName = animator.GetCurrentAnimatorClipInfo(1)[0].clip.name;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        rechargeName = animator.GetCurrentAnimatorClipInfo(1)[0].clip.name;
        rechargeSpeed = animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
        clips = animator.runtimeAnimatorController.animationClips;
        Debug.Log(rechargeName);
        if(rechargeName != startName){    
            for(int i=0; i < clips.Length; i++){
                    if(clips[i].name == rechargeName){ 
                    rechargeEnd = new AnimationEvent();
                    rechargeEnd.time = rechargeSpeed;
                    rechargeEnd.functionName = "Global_Mob_RechargeEnd";
                    currentClip = animator.runtimeAnimatorController.animationClips[i];
                    if(animator.runtimeAnimatorController.animationClips[i].events.Length == 0){
                        currentClip.AddEvent(rechargeEnd);
                    }
                    Debug.Log(clips[i].name);
                }
            }
        }
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
