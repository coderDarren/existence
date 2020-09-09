using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : StateMachineBehaviour
{
    
    private float range;
    private Player m_Player;
    private PlayerCombatController m_PlayerCombat;
    private NetworkPlayer m_Network;
    private GameObject child;
    private Mob target;
    private bool attacking;
    private float pauseSpeed;
    private float distance;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_PlayerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombatController>();
        m_Network = GameObject.FindGameObjectWithTag("Player").GetComponent<NetworkPlayer>();
        range = m_Player.attackRange;
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        pauseSpeed = animator.GetFloat("totalSpeed");
        attacking = animator.GetBool(m_Player.weapon.ToString());
        target = m_PlayerCombat.target; 

        #region Cancel/Pause Animaton
        if(!attacking){//Cancel animation
            animator.SetFloat("totalSpeed", pauseSpeed);
            if (animator.GetBool("cycle") == false) {
                animator.SetBool("cycle", true);
                m_Network.Network_WriteAnimBool("cycle", true);
            }
        }
        if(m_PlayerCombat.target){            //set animation weights to 0     
            range = m_Player.attackRange;         
            distance = Vector3.Distance(animator.gameObject.transform.position, target.transform.position);

            if (target.GetComponentInChildren<Renderer>().IsVisibleFrom(Camera.main) == false || range <= distance){
                animator.SetFloat("totalSpeed", 0);
                //animator.SetLayerWeight(1,0);
            }
            
            if (target.GetComponentInChildren<Renderer>().IsVisibleFrom(Camera.main) && range >= distance){//No attack logic
                animator.SetFloat("totalSpeed", 1);
                //animator.SetLayerWeight(1,1);
                
            }
        }
        #endregion
             
    }
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        animator.SetFloat("totalSpeed", pauseSpeed);
    }
}

