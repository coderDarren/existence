using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : StateMachineBehaviour
{
    
    public float range;   
    
    private Player m_Player;
    private Targeting m_PlayerController;
    private AnimationEvent attackEnd;   
    private AnimationClip[] clips;
    private AnimationClip currentClip;
    private GameObject player;
    private GameObject child;
    private Mob target;
    private Animation animation;    
    private bool attacking;
    private bool tickBool;
    private float pauseSpeed;
    private float safetySpeed;
    private float distance;
    private float currentLength;
    private string currentName;
    private int currentClipNum;
    private int i;
    
    
    
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex    ){
        
        pauseSpeed = animator.GetFloat("totalSpeed");
        i=0;
        tickBool =  false;
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_PlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<Targeting>();
        safetySpeed = 0;
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        if(!m_PlayerController.m_Target) {
            //animator.SetBool("cycle", true);
        }
        attacking = animator.GetBool(m_Player.weapon.ToString());
        target = m_PlayerController.m_Target;
        player = GameObject.FindGameObjectWithTag("Player"); 
        if(m_PlayerController.m_Target)
            distance = Vector3.Distance(player.transform.position, target.transform.position);
        
        #region Determining end of animation
        if(animator.GetCurrentAnimatorClipInfo(1).Length > 0){
                currentName = animator.GetCurrentAnimatorClipInfo(1)[0].clip.name; 
                currentLength = animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
                
                clips = animator.runtimeAnimatorController.animationClips;
            foreach(AnimationClip clip in clips){
                
                if(clip.name == currentName && i <= clips.Length){                
                    tickBool = true;
                    currentClipNum = i;
                    attackEnd = new AnimationEvent();                
                    attackEnd.time = currentLength;
                    attackEnd.functionName = "AttackEnd";
                    currentClip = animator.runtimeAnimatorController.animationClips[currentClipNum];
                    if(animator.runtimeAnimatorController.animationClips[currentClipNum].events.Length == 0){
                        currentClip.AddEvent(attackEnd);
                    }
                }
                else if(!tickBool)
                    i++;            
            }
        }
        #endregion
        
        #region Cancel/Pause Animaton
        if(!attacking){//Cancel animation
            animator.SetFloat("totalSpeed", pauseSpeed);
            animator.SetBool("cycle", true);            
        }
        if(m_PlayerController.m_Target){            
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

