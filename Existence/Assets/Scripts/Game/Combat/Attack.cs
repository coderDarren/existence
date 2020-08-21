﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : StateMachineBehaviour
{
    
    private float range;
    private Player m_Player;
    private PlayerCombatController m_PlayerCombat;
    private GameObject child;
    private Mob target;
    private bool attacking;
    private float pauseSpeed;
    private float distance;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex    ){
        pauseSpeed = animator.GetFloat("totalSpeed");

        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_PlayerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombatController>();
        range = m_Player.attackRange;
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        

        attacking = animator.GetBool(m_Player.weapon.ToString());
        target = m_PlayerCombat.target; 

        #region Cancel/Pause Animaton
        if(!attacking){//Cancel animation
            animator.SetFloat("totalSpeed", pauseSpeed);
            animator.SetBool("cycle", true);
        if(m_PlayerCombat.target){            //set animation weights to 0                        
        }
            distance = Vector3.Distance(animator.gameObject.transform.position, target.transform.position);

            if (target.GetComponentInChildren<Renderer>().IsVisibleFrom(Camera.main) == false || range <= distance){
                animator.SetFloat("totalSpeed", 0);
                //animator.SetLayerWeight(1,0);
            }
            
            if (target.GetComponentInChildren<Renderer>().IsVisibleFrom(Camera.main) && range >= distance){//No attack logic
                animator.SetFloat("totalSpeed", pauseSpeed);
                //animator.SetLayerWeight(1,1);
                
            }
        }
        #endregion
             
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        animator.SetFloat("totalSpeed", pauseSpeed);
    }
}

