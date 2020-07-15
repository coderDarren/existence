﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEntrance : StateMachineBehaviour
{
    private Player m_Player;
    private PlayerCombatController m_PlayerCombat;
    
   
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_PlayerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombatController>();
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

   
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.SetBool(m_PlayerCombat.special, false);
       
       if(animator.GetBool(m_Player.weapon.ToString()) == false){
            animator.SetBool(m_Player.weapon.ToString(), true);
            animator.SetBool("cycle", false); 
        }
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
