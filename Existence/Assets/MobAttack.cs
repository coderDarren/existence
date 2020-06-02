using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobAttack : StateMachineBehaviour
{
    
    private string attackName;
    private float attackSpeed;
    
    private bool attacking;
    private bool combat;
    private AnimationEvent attackEnd;
    private AnimationClip[] clips;
    private AnimationClip currentClip;


    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        clips = animator.runtimeAnimatorController.animationClips;   
    }

    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        try {
            attackName = animator.GetCurrentAnimatorClipInfo(1)[0].clip.name;
            attackSpeed = animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
            attacking = animator.GetBool("Attacking");
            combat = animator.GetBool("Combat");

            for(int i=0; i < clips.Length; i++){
                        

                if(clips[i].name == attackName){ //check for changes from server in attacking/combat bool set pause event based on attack as long as combat is true, 
                    attackEnd = new AnimationEvent();                
                    attackEnd.time = attackSpeed;
                    attackEnd.functionName = "Global_Mob_AttackEnd";
                    currentClip = animator.runtimeAnimatorController.animationClips[i];
                    if(animator.runtimeAnimatorController.animationClips[i].events.Length == 0){
                        currentClip.AddEvent(attackEnd);
                    }
                }
            }

            if(!attacking && combat) {
                animator.SetFloat("Speed", 0);  
                //animator.SetLayerWeight(1, 0);
            }              
        
            if(attacking && combat) {
                animator.SetFloat("Speed", 1);
                //animator.SetLayerWeight(1, 1);
            }
            
            if(!combat)
                animator.SetFloat("Speed", 1);
        } catch (System.Exception _e) {
            
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
