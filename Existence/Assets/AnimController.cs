using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : StateMachineBehaviour
{
    private bool m_Attacking;
    private string m_Weapon;    
   
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        m_Attacking = animator.GetBool("attacking");
        m_Weapon = animator.gameObject.GetComponent<Player>().weapon.ToString();

        if(m_Attacking)
            animator.SetBool(m_Weapon, true);

        else
           animator.SetBool(m_Weapon, false);
    }
}
