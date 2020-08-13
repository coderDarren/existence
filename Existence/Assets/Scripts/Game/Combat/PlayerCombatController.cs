using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCombatController : GameSystem
{
    public static PlayerCombatController instance;

    public KeyCode attack;
    public float specialTimer;
    public float projectileSpeed;
    public string special;

    private Player m_Player;
    private Animator m_Animator;
    private GameObject m_Weapon;
    private WeaponItemData m_WepData;
    private ParticleSystem[] m_Glow;
    private ParticleSystem[] m_Effect;
    private ParticleSystem[] m_Charge;
    private ParticleSystem[] m_Projectile;
    private ParticleSystem.Particle[] m_CurrentParticle;
    private Mob m_CurrentTarget;
    private Mob m_Target;
    private Material bladeMat;
    private float specialRecharge;
    private int m_Amount;
    private int m_ParticleNum;
    private Vector3 m_Position;
    private Vector3 m_ParticlePos; 
    private bool m_AttackInput;
    private bool m_CancelTarget;
    private bool m_Attacking;
    private bool m_SpecialInput;    
    
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

    public Mob target {
        get {
            return m_Target;
        }
    }

#region Unity Functions
    private void Start()
    {
        if (instance != this) return;

        m_Amount = 0;
        m_Animator = GetComponent<Animator>();
        m_Player = GetComponent<Player>();
        

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

    private void OnEnable() {
        if (!instance) {
            instance = this;
        } else return;
    }

    private void OnDisable() {
        if (instance == this) {
            instance = null;
        }
    }

    private void Update(){   
        if (instance != this) return;
        specialRecharge += Time.deltaTime;
        m_Weapon = GameObject.FindGameObjectWithTag("Weapon");

        if (m_Player.data.equipment.weapons.Count > 0)
            m_WepData = m_Player.data.equipment.weapons[0];

        GetInput();     
        Attack();
        if(Input.GetKeyDown(KeyCode.Alpha1))
            SpecialAttack();
        ProjectileMove();        
        CheckCombatIntegrity();
    }
#endregion

#region Public Functions
    public void AttackEnd(){        
        if(!m_Target) return;
        if (m_Weapon.GetComponent<AudioSource>())
            m_Weapon.GetComponent<AudioSource>().Play();
        
        // get damage
        if (m_WepData == null) {
            LogWarning("Unable to get weapon data for player.");
            return;
        }
        int _dmg = UnityEngine.Random.Range(m_WepData.damageMin, m_WepData.damageMax+1) + m_Player.attackRatingBoost;
        bool _crit = m_Player.RollCrit();
        if (_crit) {
            _dmg = (int)((float)m_WepData.damageMax * 1.5f) + m_Player.attackRatingBoost;
        }

        m_Target.Hit(_dmg, _crit);         
    }

    public void SelectTarget(Selectable _s, bool _primary) {
        if (_primary) {
            m_Target = (Mob)_s;
        }
    }
    
    public void DeselectTarget(Selectable _s, bool _primary) {
        if (_primary) {
            m_Target = null;
            StopAutoAttack();
        }
    }

    public void SpecialAttack() {
        special = m_Player.specialAttack;
        Log("Attempting to use special: "+special);

        if (special == string.Empty) {
            Chatbox.instance.EmitMessageLocal("No combat special available.");
            return;
        }

        if (m_Target == null) {
            Chatbox.instance.EmitMessageLocal("No target selected.");
            return;
        }
        
        if(Vector3.Distance(transform.position, m_Target.transform.position) >= m_Player.attackRange){
            Chatbox.instance.EmitMessageLocal("You are out of range.");
            return;
        }
        
        if(specialRecharge >= specialTimer){     // All the events that happen when you use your special successfully     
            specialRecharge = 0;
            m_SpecialInput = true;            
            StartAutoAttack();
            m_Animator.SetBool(special, true);
            if (m_Weapon.GetComponent<AudioSource>())
                m_Weapon.GetComponent<AudioSource>().Play();
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

            // get damage
            if (m_WepData == null) {
                LogWarning("Unable to get weapon data for player.");
                return;
            }
            int _dmg = UnityEngine.Random.Range(m_WepData.damageMin, m_WepData.damageMax)/2;

            m_Target.Hit(_dmg, false);
            ChargeEffects();
            m_SpecialInput = false; 
            
        }
        else Chatbox.instance.EmitMessageLocal("Skill is not ready yet, on cooldown for another: " + Mathf.Round(specialTimer - specialRecharge) +" seconds");
    }
#endregion

#region Private Functions 

    private void CheckCombatIntegrity() {
        // if mob dies stop attacking
        Mob _m = (Mob)m_Target;
        if ((!_m || _m.data.dead) && m_Attacking) {
            StopAutoAttack(); 
        }
    }

    private void GetInput() {
        m_AttackInput = Input.GetKeyDown(attack);
        m_CancelTarget = Input.GetKeyDown(KeyCode.Escape);
        
    }

    private void Attack(){
        if (m_AttackInput) {
            if(!m_Attacking){
                StartAutoAttack();
            }
            else {
                StopAutoAttack(); 
            }
        }
        if ((m_CancelTarget || !m_Target) && m_Attacking) {
            StopAutoAttack(); 
        }
    } 

    private void StartAutoAttack() {
        m_Attacking = true;
        m_Animator.SetBool(m_Player.weapon.ToString(), true);
    }

    public void StopAutoAttack() {
        m_Attacking = false;
        m_Animator.SetBool(m_Player.weapon.ToString(), false); 
    }


    private void RechargeTimer(float _specialTimer){
        specialTimer = _specialTimer;
        specialRecharge = _specialTimer;
        ChargeEffects();
    }

    private void ChargeEffects(){
        if (m_Glow == null || m_Charge == null) return;
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
            for(int i = 0; i < m_Projectile.Length; i++){
                ParticleSystem m_currentSystem = m_Projectile[i];
                m_ParticleNum = m_currentSystem.GetParticles(m_CurrentParticle);
                for(int j = 0; j < m_ParticleNum; j++){                    
                    if(!m_Target){
                        m_currentSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    }
                    m_CurrentParticle[j].position = Vector3.MoveTowards(m_CurrentParticle[j].position, m_Target.transform.position, projectileSpeed * Time.deltaTime);
                }
                m_currentSystem.SetParticles(m_CurrentParticle, m_ParticleNum);
               
            }
        }   catch(System.Exception _e){
            }
    }
#endregion
}
