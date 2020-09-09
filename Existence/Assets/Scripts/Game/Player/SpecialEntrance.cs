using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEntrance : StateMachineBehaviour
{
    private Player m_Player;
    private PlayerCombatController m_PlayerCombat;
    private NetworkPlayer m_Network;
   
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_PlayerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombatController>();
        m_Network = GameObject.FindGameObjectWithTag("Player").GetComponent<NetworkPlayer>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

   
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.SetBool(m_PlayerCombat.special, false);
       m_Network.Network_WriteAnimSpecial(m_PlayerCombat.special, false);

       if(animator.GetBool(m_Player.weapon.ToString()) == false){
            animator.SetBool(m_Player.weapon.ToString(), true);
            animator.SetBool("cycle", false); 
            m_Network.Network_WriteAnimAttack(m_Player.weapon.ToString(), true);
            m_Network.Network_WriteAnimBool("cycle", false);
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
