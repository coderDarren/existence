using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : StateMachineBehaviour
{
    
    private float range;
    private Player m_Player;
    private Targeting m_Targeting;
    private GameObject child;
    private Mob target;
    private bool attacking;
    private float pauseSpeed;
    private float safetySpeed;
    private float distance;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex    ){
        pauseSpeed = animator.GetFloat("totalSpeed");

        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_Targeting = GameObject.FindGameObjectWithTag("Player").GetComponent<Targeting>();
        range = m_Player.range;
        safetySpeed = 0;
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        
        attacking = animator.GetBool(m_Player.weapon.ToString());
        target = m_Targeting.m_Target; 

        #region Cancel/Pause Animaton
        if(!attacking){//Cancel animation
            animator.SetFloat("totalSpeed", pauseSpeed);
            animator.SetBool("cycle", true);            
        }
        if(m_Targeting.m_Target){            
            distance = Vector3.Distance(animator.gameObject.transform.position, target.transform.position);

            if (!target.GetComponentInChildren<Renderer>().IsVisibleFrom(Camera.main) || range <= distance){
                animator.SetFloat("totalSpeed", safetySpeed);
            }
            
            if (target.GetComponentInChildren<Renderer>().IsVisibleFrom(Camera.main) && range >= distance){//No attack logic
                animator.SetFloat("totalSpeed", pauseSpeed);
            }
        }
        #endregion
             
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        animator.SetFloat("totalSpeed", pauseSpeed);
    }
}

