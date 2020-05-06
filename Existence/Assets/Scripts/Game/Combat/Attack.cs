using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : StateMachineBehaviour
{
    public string wpnAnim;
    
    private ParticleSystem[] particles;
    private GameObject player;
    private bool atkDmg;
    private bool attacking;
    private float pauseSpeed;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        atkDmg = true;
        pauseSpeed = animator.GetFloat("totalSpeed");
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        attacking = animator.GetBool("attacking" + wpnAnim);
        
        
        if(!attacking){//Cancel animation
            animator.SetFloat("totalSpeed", pauseSpeed);
            animator.SetTrigger("cycle");            
        }        
        #region Animation Pause //Roflcakes
        if(Input.GetAxis("Vertical") > 0.1f || Input.GetAxis("Vertical") < -0.1f){
            animator.SetFloat("totalSpeed", 0);
        }        
        if(Input.GetAxis("Horizontal") > 0.1f || Input.GetAxis("Horizontal") < -0.1f){
            animator.SetFloat("totalSpeed", 0);
        }
        if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0){
            animator.SetFloat("totalSpeed", pauseSpeed);
        }        
        
        #endregion
        
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

