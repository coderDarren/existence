using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : StateMachineBehaviour
{
    public string wpnAnim;
    public float range;    
    
    private ParticleSystem[] particles;
    private GameObject player;
    private GameObject child;
    private GameObject target;
    private bool atkDmg;
    private bool attacking;
    private float pauseSpeed;
    private float safetySpeed;
    private float distance;
    
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        atkDmg = true;
        pauseSpeed = animator.GetFloat("totalSpeed");        
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        safetySpeed = 0;        
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        attacking = animator.GetBool("attacking" + wpnAnim);
        target = GameObject.FindGameObjectWithTag("CombatTestDummy");
        player = GameObject.FindGameObjectWithTag("Player");
        particles = player.GetComponentsInChildren<ParticleSystem>();
        child = target.transform.GetChild(0).gameObject;
        distance = Vector3.Distance(player.transform.position, target.transform.position);        
        
        
        if(!attacking){//Cancel animation
            animator.SetFloat("totalSpeed", pauseSpeed);
            animator.SetTrigger("cycle");            
        }
        if (child.GetComponent<Renderer>().IsVisibleFrom(Camera.main) && range >= distance){//No attack logic
            animator.SetFloat("totalSpeed", pauseSpeed);
        }            
        if (!child.GetComponent<Renderer>().IsVisibleFrom(Camera.main) || range <= distance){
            animator.SetFloat("totalSpeed", safetySpeed);
        } 
        if (stateInfo.normalizedTime >= 1){//Send some Damage and Bullet Effect as long as animation completed            
                        
            animator.SetTrigger("cycle");

            foreach(ParticleSystem particle in particles){
                particle.Play();                    
            } 

            if(atkDmg){//Prevents multiple hits per animation
                GameObject.FindGameObjectWithTag("CombatTestDummy").GetComponent<Mob>().Hit(50);
                atkDmg = false;
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        animator.SetFloat("totalSpeed", pauseSpeed);
    }
}

