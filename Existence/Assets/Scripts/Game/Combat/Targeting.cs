using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Targeting : GameSystem
{
    public float radius;
    public KeyCode attack;
    public enum Special{chargeShot, quickSlice};
    public Special m_Special;
    public float specialTimer;
    public float projectileSpeed;

    private Player m_Player;
    private Animator m_Animator;
    private PlayerController m_PlayerController;
    private ParticleSystem m_Glow;
    private ParticleSystem m_Effect;
    private Mob m_CurrentTarget;
    public Mob m_Target;
    private Mob[] m_Targets;
    private Material bladeMat;
    private float[] m_Distances;
    private float specialRecharge;
    private int m_Amount;
    private Vector3 m_Position; 
    private bool m_AttackInput;
    private bool m_CycleTarget;
    private bool m_CancelTarget;
    private bool m_Attacking;
    private bool m_Targeting;
    private bool m_SpecialInput;
    private int m_TargetNum;
    

    public bool attacking {
        get {
            return m_Attacking;
        }
    }
    public bool specialInput {
        get {
            return m_SpecialInput;
        }
    }

    private void Start()
    {
        m_Amount = 0;
        m_Animator = GetComponent<Animator>();
        m_Player = GetComponent<Player>();
        m_PlayerController = GetComponent<PlayerController>();
        m_Glow = transform.FindDeepChild("Glow").GetComponent<ParticleSystem>();
        m_Effect = transform.FindDeepChild("Effect").GetComponent<ParticleSystem>();
        RechargeTimer(specialTimer);
    }

    private void Update(){   
        GetInput();     
        if(m_CycleTarget){
            Search();
        }
        if(m_Target != null || m_CurrentTarget != null) m_Targeting = true;
        else m_Targeting = false;

        specialRecharge += Time.deltaTime;
        if(specialRecharge >= specialTimer){
            //m_Glow.Play();
        }
        Select();
        Attack();
        if(m_SpecialInput) SpecialAttack(m_Special);
    }

#region Private Functions 
    private void GetInput() {
        m_AttackInput = Input.GetKeyDown(attack);
        m_CycleTarget = Input.GetKeyDown(KeyCode.Tab);
        m_CancelTarget = Input.GetKeyDown(KeyCode.Escape);
        m_SpecialInput = Input.GetKeyDown(KeyCode.Alpha1);
    }

    private void Search(){
        m_Position = transform.position;
        m_Targets = NetworkEntityHandler.instance.mobs.ToArray();
        m_Distances = new float[m_Targets.Length];

        for(int i=0; i < m_Targets.Length; i++){
            m_Distances[i] = Vector3.Distance(m_Targets[i].transform.position, m_Position);
        }

        Array.Sort(m_Distances, m_Targets);
        
        if(m_Targets.Length != m_Amount){
            FindTarget(m_Targets);
            m_Amount = m_Targets.Length;           
        }
        else{
            CycleTarget();    
        }
    }

    private void Select() {
        Selectable _s = SelectionController.instance.selection;
        if (_s && _s.GetType().IsAssignableFrom(typeof(Mob))) {
            if (m_CurrentTarget) {
                m_CurrentTarget.nameplate.isVisible = false;
            }
            m_CurrentTarget = (Mob)_s;
        }

        if (m_Target) {
            m_Target.nameplate.isVisible = true;
        }
        
        if (m_CurrentTarget) {
            m_CurrentTarget.nameplate.isVisible = true;
        }
    }

    private void FindTarget(Mob[] _targets){
        m_CurrentTarget = _targets[0];
        m_Targets = new Mob[_targets.Length];
        m_Targets = _targets;
    }
    
    private void CycleTarget(){
        if (m_CurrentTarget) {
            m_CurrentTarget.nameplate.isVisible = false;
        }
        m_CurrentTarget = m_Targets[m_TargetNum];
        m_TargetNum++;
        if(m_TargetNum >= m_Targets.Length)
            m_TargetNum = 0;
    }    

    private void Attack(){
        
        
        if (m_AttackInput) {
            if(!m_Attacking){
                m_Attacking = true;
                if (m_Target) {
                    m_Target.nameplate.isVisible = false;
                }
                m_Target = m_CurrentTarget;
                m_Animator.SetBool(m_Player.weapon.ToString(), true);
                
            }
            else {
                m_Attacking = false;
                CancelTarget(ref m_Target);
                m_Animator.SetBool(m_Player.weapon.ToString(), false);  
            }
        }
        if(m_CancelTarget || !m_Target){
            Cancel();
            m_Attacking = false;
            m_Animator.SetBool(m_Player.weapon.ToString(), false);
        }
    } 

    public void SpecialAttack(Special _special){
           
        Debug.Log("Attempting to use special");
        if(!m_Targeting){
            Debug.Log("You have no target");
            return;
        }
        if(m_Target == null)m_Target = m_CurrentTarget;
        
        if(Vector3.Distance(transform.position, m_Target.transform.position) >= m_Player.range){// Weapon range is now a variable on the player
            Debug.Log("Too far away");
            return;
        }
        if(specialRecharge >= specialTimer){     // All the events that happen when you use your special successfully     
            specialRecharge = 0;            
            m_Attacking = true;
            m_Animator.SetBool(m_Special.ToString(), true);
            //m_Effect.Play();
            
            m_PlayerController.GetComponent<Targeting>().m_Target.Hit(25);
            //m_Glow.Stop();
            
        }
        else Debug.Log("Skill is not ready yet, on cooldown for another: " + Mathf.Round(specialTimer - specialRecharge) +" seconds");
    }

    public void RechargeTimer(float _specialTimer){
        specialTimer = _specialTimer;
        specialRecharge = _specialTimer;
    }

    private void Cancel() {
        CancelTarget(ref m_Target);
        //CancelTarget(ref m_CurrentTarget);
        SelectionController.instance.selection = null;
        m_Animator.SetBool(m_Player.weapon.ToString(), false); 
    }

    private void CancelTarget(ref Mob _target) {
        if (_target) {
            _target.nameplate.isVisible = false;
        }
        _target = null;  
    }
#endregion
}
