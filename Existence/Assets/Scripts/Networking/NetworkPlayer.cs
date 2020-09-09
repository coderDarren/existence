using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// NetworkPlayer is placed on all players including the local client
///
/// This class is used to manage incoming and outgoing NetworkPlayerData
/// </summary>
[RequireComponent(typeof(Animator))]
public class NetworkPlayer : Selectable
{
    [Header("Client Settings")]
    public bool isClient;
    public float sendRate = 0.15f;
    public float idleDetectionSeconds = 2;
    
    [Header("Network Player Settings")]
    public float moveSmooth=0.1f;
    
    private Session m_Session;
    private NetworkController m_Network;
    private Vector3 m_InitialPos;
    private Vector3 m_TargetPos;
    private Vector3 m_InitialEuler;
    private Vector3 m_TargetEuler;
    private NetworkPlayerData m_ClientData;
    private NetworkPlayerData m_NetworkData;
    private NetworkPlayerData m_LastFrameData;
    private PlayerController m_PlayerController;
    private PlayerCombatController m_PlayerCombat;
    private EquipmentController m_EquipmentController;
    private Player m_Player;
    private Animator m_Animator;
    private Hashtable m_FloatAnimTargets;
    private float m_InitialRunning;
    private float m_TargetRunning;
    private float m_Running;
    private float m_InitialStrafing;
    private float m_TargetStrafing;
    private float m_Strafing;
    private bool m_Grounded;
    private bool m_Attacking;
    private bool m_AttackCycle;
    private bool m_SpecialInput;
    private float m_UpdateTimer;
    private float m_IdleTimer;
    private long m_LastUpdateMillis;
    private float m_Smooth;
    private float m_AttackSpeed;
    private string m_Weapon;
    private string m_Special;

    // get Session with integrity
    private Session session {
        get {
            if (!m_Session) {
                m_Session = Session.instance;
            }
            if (!m_Session) {
                LogError("Trying to use Session, but no instance could be found.");
            }
            return m_Session;
        }
    }

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

    private EquipmentController equipmentController {
        get {
            if (!m_EquipmentController) {
                m_EquipmentController = GetComponent<EquipmentController>();
            }
            if (!m_EquipmentController) {
                LogWarning("Trying to get equipment but no instance of EquipmentController was found.");
            }
            return m_EquipmentController;
        }
    }

    public NetworkPlayerData clientData {
        get {
            return m_ClientData;
        }
    }

    public string name {
        get {
            if (m_LastFrameData == null) {
                return string.Empty;
            }
            return m_LastFrameData.name;
        }
    }

#region Unity Functions
    private void Start() {
        m_Animator = GetComponent<Animator>();
        if (isClient) {
            m_PlayerController = GetComponent<PlayerController>();
            m_PlayerCombat = GetComponent<PlayerCombatController>();
            m_Player = GetComponent<Player>();
            m_NameplateData = new NameplateData();
            m_NameplateData.name = m_Player.data.player.name;
            NameplateController.instance.TrackSelectable(this);
        } else {
            m_FloatAnimTargets = new Hashtable();
            m_Animator.SetBool("grounded", true);
        }
    }

    private void Update() {
        if (isClient) {
            UpdateClient();
        } else {
            UpdateNetworkPlayer();
        }
    }
#endregion

#region Public Functions
    /*
     * This function called by NetworkEntityHandler when a new..
     * ..NetworkPlayer enters the scene
     */ 
    public void Init(NetworkPlayerData _data) {
        if (isClient) return;
        m_NetworkData = new NetworkPlayerData();
        m_NameplateData = new NameplateData();
        m_NameplateData.name = _data.name;
        m_TargetPos = new Vector3(_data.transform.pos.x, _data.transform.pos.y, _data.transform.pos.z);
        m_TargetEuler = new Vector3(_data.transform.rot.x, _data.transform.rot.y, _data.transform.rot.z);
        transform.position = m_TargetPos;

        foreach (ArmorItemData _item in _data.equipment.armor) {
            equipmentController.Equip(_item);
        }

        foreach (WeaponItemData _item in _data.equipment.weapons) {
            equipmentController.Equip(_item);
        }

        m_UpdateTimer = 0;
        m_NetworkData = _data;
        m_LastFrameData = _data;
        m_Animator = GetComponent<Animator>();
        m_Animator.SetFloat(_data.anim.running.anim, _data.anim.running.val);
        m_Animator.SetBool(_data.anim.grounded.anim, _data.anim.grounded.val);
        m_Animator.SetBool(_data.anim.attacking.anim, _data.anim.attacking.val);
        m_Animator.SetBool(_data.anim.special.anim, _data.anim.special.val);
        m_Animator.SetBool(_data.anim.cycle.anim, _data.anim.cycle.val);
        UpdateNameplate(m_NetworkData.name, m_NetworkData.health.health, m_NetworkData.health.maxHealth, m_NetworkData.lvl);
    }

    /*
     * This function called by Session when this client loads in with his data
     */
    public void Init(PlayerData _data) {
        m_ClientData = new NetworkPlayerData();
        m_ClientData.id = _data.player.ID;
        m_ClientData.name = _data.player.name;
        m_ClientData.tix = _data.player.tix;
        m_ClientData.health.health = _data.player.health;
        m_ClientData.health.maxHealth = _data.player.maxHealth;
        m_ClientData.anim.running.val = 0;
        m_ClientData.anim.grounded.val = false;
        m_ClientData.equipment = _data.equipment;
    }

    public void Dispose() {
        if (!isClient) return;
        NameplateController.instance.ForgetSelectable(this);
    }

#region Network Readers
    public void Network_ReadTransform(NetworkTransform _data) {
        if (isClient) return;

        // if (network.usePredictiveSmoothing && _data.timestamp != null && m_LastFrameData != null && m_LastFrameData.timestamp != null) {
        //     if (m_LastFrameData == null) {
        //         m_LastFrameData = _data;
        //     }
        //     long _diff = long.Parse(_data.timestamp) - long.Parse(m_LastFrameData.timestamp);
        //     //Log(_data.timestamp+" Latency: "+_diff+"ms");
        //     m_Smooth = _diff / 1000.0F;
        // } else {
        //     m_Smooth = moveSmooth;
        // }

        m_InitialPos = transform.position;
        m_TargetPos = new Vector3(_data.pos.x, _data.pos.y, _data.pos.z);
        m_InitialEuler = transform.eulerAngles;
        m_TargetEuler = new Vector3(_data.rot.x, _data.rot.y, _data.rot.z);
        m_UpdateTimer = 0;
        m_Smooth = moveSmooth;
        
        // m_LastFrameData = _data;
    }

    public void Network_ReadHealth(NetworkPlayerHealth _data) {
        m_NetworkData.health = _data;
        UpdateNameplate(m_NetworkData.health.id, m_NetworkData.health.health, m_NetworkData.health.maxHealth, m_NetworkData.lvl);
    }
    
    public void Network_ReadLevel(NetworkPlayerLvl _data) {
        
    }

    public void Network_ReadAnimFloat(NetworkAnimFloat _data) {
        if (!m_FloatAnimTargets.ContainsKey(_data.anim)) {
            m_FloatAnimTargets.Add(_data.anim, _data.val);
        } else {
            m_FloatAnimTargets[_data.anim] = _data.val;
        }
    }
    
    public void Network_ReadAnimBool(NetworkAnimBool _data) {
        m_Animator.SetBool(_data.anim, _data.val);
    }
#endregion

#region Network Writers
    public void Network_WriteTransform() {
        network.SendPlayerTransform(m_ClientData.transform);
    }
    
    public void Network_WriteHealth() {
        m_ClientData.health.health = m_Player.data.player.health;
        m_ClientData.health.maxHealth = m_Player.MaxHealth();
        network.SendPlayerHealth(m_ClientData.health);
    }
    
    public void Network_WriteLevel() {
        
    }

    public void Network_WriteAnimAttack(string _anim, bool _val) {
        m_ClientData.anim.attacking = new NetworkAnimBool(m_ClientData.name, _anim, _val);
        network.SendPlayerAnimBool(m_ClientData.anim.attacking);
    }

    public void Network_WriteAnimSpecial(string _anim, bool _val) {
        m_ClientData.anim.special = new NetworkAnimBool(m_ClientData.name, _anim, _val);
        network.SendPlayerAnimBool(m_ClientData.anim.special);
    }
    
    public void Network_WriteAnimFloat(string _anim, float _val) {
        switch (_anim) {
            case "running":
                m_ClientData.anim.running = new NetworkAnimFloat(m_ClientData.name, _anim, _val);
                network.SendPlayerAnimFloat(m_ClientData.anim.running);
                break;
        }
    }
    
    public void Network_WriteAnimBool(string _anim, bool _val) {
        switch (_anim) {
            case "grounded":
                m_ClientData.anim.grounded = new NetworkAnimBool(m_ClientData.name, _anim, _val);
                network.SendPlayerAnimBool(m_ClientData.anim.grounded);
                break;
            case "cycle":
                m_ClientData.anim.cycle = new NetworkAnimBool(m_ClientData.name, _anim, _val);
                network.SendPlayerAnimBool(m_ClientData.anim.cycle);
                break;
        }
    }
#endregion
#endregion

#region Private Functions
    // Player controlled by this client
    private void UpdateClient() {
        if (!network) return;

        if (ClientHasNotChanged()) {
            m_IdleTimer += Time.deltaTime;
        } else {
            m_IdleTimer = 0;
        }

        m_ClientData.name = session.playerData.player.name;
        m_ClientData.UpdatePos(transform.position);
        m_ClientData.UpdateRot(transform.eulerAngles);
        m_ClientData.lvl = m_Player.data.player.level;
        m_ClientData.equipment = m_Player.data.equipment;

        UpdateNameplate(m_ClientData.name, m_ClientData.health.health, m_ClientData.health.maxHealth, m_ClientData.lvl);
      
        m_UpdateTimer += Time.deltaTime;
        if (m_UpdateTimer >= sendRate && m_IdleTimer < idleDetectionSeconds) {
            Network_WriteTransform();
            m_UpdateTimer = 0;
        }
    }

    private bool ClientHasNotChanged() {
        return  m_ClientData.transform.pos.x == transform.position.x && 
                m_ClientData.transform.pos.y == transform.position.y && 
                m_ClientData.transform.pos.z == transform.position.z &&
                m_ClientData.transform.rot.x == transform.eulerAngles.x && 
                m_ClientData.transform.rot.y == transform.eulerAngles.y && 
                m_ClientData.transform.rot.z == transform.eulerAngles.z &&
                m_ClientData.anim.attacking.val == false;   
    }

    // Player not controlled by this client
    private void UpdateNetworkPlayer() {
        if (m_UpdateTimer > m_Smooth) return;
        m_UpdateTimer += Time.deltaTime;
        transform.position = Vector3.Lerp(m_InitialPos, m_TargetPos, m_UpdateTimer / m_Smooth);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(m_InitialEuler), Quaternion.Euler(m_TargetEuler), m_UpdateTimer / m_Smooth);
        
        // animation
        foreach (DictionaryEntry _entry in m_FloatAnimTargets) {
            string _key = (string)_entry.Key;
            float _target = (float)_entry.Value;
            float _curr = m_Animator.GetFloat(_key);
            m_Animator.SetFloat(_key, Mathf.Lerp(_curr, _target, 3 * Time.deltaTime));
        }
       
    }
    
    private void PollPredictiveSmoothing() {
        if (m_LastUpdateMillis == 0) {
            m_LastUpdateMillis = NetworkTimestamp.NowMilliseconds();
            return;
        }
        long _currMillis = NetworkTimestamp.NowMilliseconds();
        long _diff = _currMillis - m_LastUpdateMillis;
        m_LastUpdateMillis = _currMillis;

        Log("Network Delta: "+_diff);
        m_Smooth = _diff / 1000.0f;
    }
    
#endregion

}
