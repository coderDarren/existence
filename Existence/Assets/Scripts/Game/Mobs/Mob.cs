﻿
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Mob : Selectable
{
    public float smooth;
    public LayerMask ground;

    private NetworkController m_Network;
    private NetworkMobData m_Data;
    private PlayerController m_Controller;
    private Animator m_Animator;
    private CapsuleCollider m_Collider;
    private Vector3 m_InitialPos;
    private Vector3 m_TargetPos;
    private Vector3 m_InitialRot;
    private Vector3 m_TargetRot;
    private Vector3 m_LastFramePos;
    private float m_UpdateTimer;

    private NetworkController network {
        get {
            if (!m_Network) {
                m_Network = NetworkController.instance;
            }
            if (!m_Network) {
                LogWarning("Trying to get network but no instance of NetworkController was found.");
            }
            return m_Network;
        }
    }

    public string id {
        get {
            return m_Data.id;
        }
    }

#region Unity Functions
    private void Start(){
        m_Animator = GetComponent<Animator>();
    }

    private void Update() {
        //Test();
        
        if (m_UpdateTimer > smooth) return;
        m_UpdateTimer += Time.deltaTime;
        
        transform.position = Vector3.Lerp(m_InitialPos, m_TargetPos, m_UpdateTimer / smooth);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(m_InitialRot), Quaternion.Euler(m_TargetRot), m_UpdateTimer / smooth);
        FindGroundPos();
        DetectMoveAnimation();
    }
#endregion

#region Public Functions
    public void Init(NetworkMobData _data) {
        m_Collider = GetComponent<CapsuleCollider>();
        m_Data = _data;
        m_Nameplate = new NameplateData();
        m_Nameplate.name = m_Data.name;
        m_Controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        transform.rotation = Quaternion.Euler(_data.rot.x, _data.rot.y, _data.rot.z);
        transform.position = new Vector3(_data.pos.x, _data.pos.y, _data.pos.z);
        FindGroundPos();
        m_InitialRot = transform.eulerAngles;
        m_InitialPos = transform.position;
        m_TargetRot = m_InitialPos;
        m_TargetPos = m_InitialPos;
        m_UpdateTimer = smooth + 1;
    }

    public void UpdateTransform(NetworkMobData _data) {
        m_Data.pos = _data.pos;
        m_Data.rot = _data.rot;

        m_TargetPos.x = m_Data.pos.x;
        m_TargetPos.y = m_Data.pos.y;
        m_TargetPos.z = m_Data.pos.z;
        m_TargetRot.x = m_Data.rot.x;
        m_TargetRot.y = m_Data.rot.y;
        m_TargetRot.z = m_Data.rot.z;
        m_InitialRot = transform.eulerAngles;
        m_InitialPos = transform.position;
        m_UpdateTimer = 0;

        UpdateNameplate(m_Data.name, m_Data.health, m_Data.maxHealth);
    }

    public void UpdateCombatState(NetworkMobData _data) {
        m_Animator.SetBool("Combat", _data.inCombat);
    }

    public void UpdateAttackRangeState(NetworkMobData _data) {
        m_Animator.SetBool("Attacking", _data.inAttackRange);

        if (_data.inAttackRange) {
            m_Animator.SetBool("Combat", true);
        }
    }

    public void Attack() {
        m_Animator.SetTrigger("Cycle");
    }

    public void Hit(int _dmg) {
        if (!network) return;
        NetworkMobHitInfo _hitInfo = new NetworkMobHitInfo(m_Data.id, _dmg);
        network.HitMob(_hitInfo);
    }
#endregion

#region Private Functions
    private void FindGroundPos() {
        RaycastHit _hit;
        if (Physics.Raycast(transform.position + Vector3.up, -Vector3.up, out _hit, Mathf.Infinity, ground)) {
            if (_hit.collider.gameObject != gameObject) {
                Vector3 _pos = transform.position;
                _pos.y = _hit.point.y + 0.01f;
                transform.position = _pos;
            }
        }
    }

    private void DetectMoveAnimation() {
        float _dist = Vector3.Distance(m_LastFramePos, transform.position);
        if (_dist > 0.01f) {
            m_Animator.SetBool("Moving", true);
        } else {
            m_Animator.SetBool("Moving", false);
        }

        m_LastFramePos = transform.position;
    }

    private void Test() {
        /*if(Input.GetKeyDown(KeyCode.Z)){
            m_Animator.SetBool("Attacking", true);
            m_Animator.SetBool("Combat", true);
        }
        if(Input.GetKeyDown(KeyCode.X)){
            m_Animator.SetBool("Attacking", false);
            m_Animator.SetBool("Combat", false);
        }
        if(Input.GetKeyDown(KeyCode.C)){
            m_Animator.SetFloat("Speed", 0);
        }*/
    }
#endregion

    public void Global_Mob_AttackEnd(){
        m_Animator.ResetTrigger("Cycle");
        m_Animator.SetTrigger("Recharge");
    }
        
    public void Global_Mob_RechargeEnd(){
        m_Animator.ResetTrigger("Recharge");
        m_Animator.SetTrigger("Cycle");
    }

}
