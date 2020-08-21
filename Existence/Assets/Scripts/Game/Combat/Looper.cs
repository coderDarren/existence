using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looper : StateMachineBehaviour
{   
    public float buffSpeed;

    private Player m_Player;
    private bool attacking;
    private float totalSpeed;
    


    // Need to reference weapon attack speed and create a parameter in <Player> that handles debuffs applied to the character.
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        //animator.SetBool("cycle", false);
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        attacking = animator.GetBool(m_Player.weapon.ToString());        // Move this to inside of attack and recharge scripts
        if(attacking){
            animator.SetFloat("totalSpeed", totalSpeed);
        }
        animator.SetBool("cycle", false);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.SetBool("cycle", false);    
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
