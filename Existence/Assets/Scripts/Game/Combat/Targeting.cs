using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Targeting : GameSystem
{
    public float radius;
    public KeyCode attack;
    public enum Special {chargeShot, quickSlice};
    public Special m_Special;
    public float specialTimer;
    public float projectileSpeed;

    private Player m_Player;
    private Animator m_Animator;
    private PlayerController m_PlayerController;
    private ParticleSystem[] m_Glow;
    private ParticleSystem[] m_Effect;
    private ParticleSystem[] m_Charge;
    private ParticleSystem[] m_Projectile;
    private ParticleSystem.Particle[] m_CurrentParticle;
    private Mob m_CurrentTarget;
    public Mob m_Target;
    private Mob[] m_Targets;
    private Material bladeMat;
    private float[] m_Distances;
    private float specialRecharge;
    private int m_Amount;
    private Vector3 m_Position;
    private Vector3 m_ParticlePos; 
    private bool m_AttackInput;
    private bool m_CycleTarget;
    private bool m_CancelTarget;
    private bool m_Attacking;
    private bool m_Targeting;
    private bool m_SpecialInput;
    private int m_TargetNum;
    private int m_ParticleNum;
    

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
        try{
            m_Charge = transform.FindDeepChild("Charge").GetComponentsInChildren<ParticleSystem>();
        } catch (System.Exception _e) {
            
        }
        try{
            m_Glow = transform.FindDeepChild("Glow").GetComponentsInChildren<ParticleSystem>();
        } catch (System.Exception _e) {
            
        }
        try{
            m_Effect = transform.FindDeepChild("Effect").GetComponentsInChildren<ParticleSystem>();
        } catch (System.Exception _e) {
            
        }
        try{
            m_Projectile = transform.FindDeepChild("Projectile").GetComponentsInChildren<ParticleSystem>();
        } catch (System.Exception _e) {
            
        }
        m_CurrentParticle = new ParticleSystem.Particle[1000];
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
            
        }
        Select();
        Attack();
        ProjectileMove();
        
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
            try{
                for(int i = 0; i < m_Effect.Length; i++){
                    ParticleSystem m_currentSystem = m_Effect[i];
                    m_currentSystem.Play();
                }
            } catch (System.Exception _e){
            }
            try{
                for(int i = 0; i < m_Projectile.Length; i++){
                    ParticleSystem m_currentSystem = m_Projectile[i];
                    m_currentSystem.Play();
                }
            } catch (System.Exception _e){
            }
            m_PlayerController.GetComponent<Targeting>().m_Target.Hit(25);
            ChargeEffects(); 
            
        }
        else Debug.Log("Skill is not ready yet, on cooldown for another: " + Mathf.Round(specialTimer - specialRecharge) +" seconds");
    }

    public void RechargeTimer(float _specialTimer){
        specialTimer = _specialTimer;
        specialRecharge = _specialTimer;
        ChargeEffects();
    }

    private void ChargeEffects(){
        
        if(m_Glow[0].gameObject != null){
            for(int i = 0; i < m_Glow.Length; i++){                
                ParticleSystem m_currentSystem = m_Glow[i];
                var glowMain = m_currentSystem.main;
                m_currentSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                glowMain.duration = specialTimer;
                m_currentSystem.startDelay = specialTimer;
                m_currentSystem.Play();
            }
        }
       
        if(m_Charge[0].gameObject != null){
            for(int i = 0; i < m_Charge.Length; i++){
                ParticleSystem m_currentSystem = m_Charge[i];
                var chargeMain = m_currentSystem.main;
                m_currentSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                chargeMain.duration = specialTimer;
                m_currentSystem.startDelay = specialTimer - m_currentSystem.startLifetime;
                m_currentSystem.Play();
            }
        }
    }

    private void ProjectileMove(){
        
        try{
            if(m_Projectile[0].isPlaying){
                Debug.Log("Translating effects");
                for(int i = 0; i < m_Projectile.Length; i++){
                    ParticleSystem m_currentSystem = m_Projectile[i];
                    m_ParticleNum = m_currentSystem.GetParticles(m_CurrentParticle);
                    for(int j = 0; j < m_ParticleNum; j++){                    
                        
                        m_CurrentParticle[j].position = Vector3.MoveTowards(m_CurrentParticle[j].position, m_Target.transform.position, projectileSpeed * Time.deltaTime);
                        Debug.Log(m_CurrentParticle[j].position);                 
                    }
                    m_currentSystem.SetParticles(m_CurrentParticle, m_ParticleNum);
                }
            }
        }   catch(System.Exception _e){
            }
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

    public void AttackEnd(){        
        if(!m_Target) return;
        m_PlayerController.GetComponent<Targeting>().m_Target.Hit(50);         
    }
#endregion
}
