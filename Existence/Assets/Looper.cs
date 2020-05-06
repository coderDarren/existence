using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looper : StateMachineBehaviour
{   
    public float atkSpeed;
    public float buffSpeed;
    public string wpnAnim;
    
    private bool attacking;
    private Animation animation;
    private float totalSpeed;
    


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){        
        animator.ResetTrigger("cycle"); // Resets over attack animation logic
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        attacking = animator.GetBool("attacking"+ wpnAnim);
        if(attacking){
            totalSpeed = buffSpeed * atkSpeed;//Mash the raw wpn speed and buffspeed potatoes
            animator.SetFloat("totalSpeed", totalSpeed);//pour gravy on potatoes           
            animator.Play(wpnAnim, 1);
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
