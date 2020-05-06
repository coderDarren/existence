using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : StateMachineBehaviour
{
    private ParticleSystem[] particles;
    private GameObject player;
    private bool atkDmg;
    private bool attacking;
    public string wpnAnim;

    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        atkDmg = true;
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        attacking = animator.GetBool("attacking" + wpnAnim);
        if(!attacking){//Cancel animation
            animator.SetTrigger("cycle");            
        }
        if (stateInfo.normalizedTime >= 1){//Send some Damage and Bullet Effect as long as animation completed
            player = GameObject.FindGameObjectWithTag("Player");
            particles = player.GetComponentsInChildren<ParticleSystem>();
            
            animator.SetTrigger("cycle");   
            
            foreach(ParticleSystem particle in particles){
                particle.Play();                    
            } 

            if(atkDmg){
                GameObject.FindGameObjectWithTag("CombatTestDummy").GetComponent<Mob>().Hit(50);
                atkDmg = false;
            }
        }
    }
}

