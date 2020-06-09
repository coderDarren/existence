﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Targeting : GameSystem
{
    public float radius;
    public KeyCode attack;

    private Player m_Player;
    private Animator m_Animator;
    private Mob m_CurrentTarget;
    public Mob m_Target;
    private Mob[] m_Targets;
    private float[] m_Distances;
    private int m_Amount;
    private Vector3 m_Position; 
    private bool m_AttackInput;
    private bool m_CycleTarget;
    private bool m_CancelTarget;
    private bool m_Attacking;
    private int m_TargetNum;

    public bool attacking {
        get {
            return m_Attacking;
        }
    }

    private void Start()
    {
        m_Amount = 0;
        m_Animator = GetComponent<Animator>();
        m_Player = GetComponent<Player>();
    }

    private void Update(){   
        GetInput();     
        if(m_CycleTarget){
            Search();
        }
        Select();
        Attack();
    }

#region Private Functions 
    private void GetInput() {
        m_AttackInput = Input.GetKeyDown(attack);
        m_CycleTarget = Input.GetKeyDown(KeyCode.Tab);
        m_CancelTarget = Input.GetKeyDown(KeyCode.Escape);
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
                Log("attacking");
                m_Attacking = true;
                if (m_Target) {
                    m_Target.nameplate.isVisible = false;
                }
                m_Target = m_CurrentTarget;
                m_Animator.SetBool(m_Player.weapon.ToString(), true);
                m_Animator.SetBool("cycle", false);
                m_Animator.SetBool("attacking", true);
            }
            else {
                m_Attacking = false;
                CancelTarget(ref m_Target);
                m_Animator.SetBool(m_Player.weapon.ToString(), false);    
                m_Animator.SetBool("attacking", false);
            }
        }
        if(m_CancelTarget || !m_CurrentTarget){
            Cancel();
        }
    } 

    private void Cancel() {
        CancelTarget(ref m_Target);
        CancelTarget(ref m_CurrentTarget);
        SelectionController.instance.selection = null;
        m_Attacking = false;
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
