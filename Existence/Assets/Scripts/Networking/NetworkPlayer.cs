
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
    private NetworkPlayerData m_LastFrameData;
    private PlayerController m_PlayerController;
    private PlayerCombatController m_PlayerCombat;
    private EquipmentController m_EquipmentController;
    private Player m_Player;
    private Animator m_Animator;
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
    public void Init(NetworkPlayerData _data) {
        if (isClient) return;
        m_NameplateData = new NameplateData();
        m_NameplateData.name = _data.name;
        m_TargetPos = new Vector3(_data.pos.x, _data.pos.y, _data.pos.z);
        m_TargetEuler = new Vector3(_data.rot.x, _data.rot.y, _data.rot.z);
        transform.position = m_TargetPos;

        foreach (ArmorItemData _item in _data.equipment.armor) {
            equipmentController.Equip(_item);
        }

        foreach (WeaponItemData _item in _data.equipment.weapons) {
            equipmentController.Equip(_item);
        }
    }

    public void Init(PlayerData _data) {
        m_ClientData = new NetworkPlayerData();
        m_ClientData.id = _data.player.ID;
        m_ClientData.name = _data.player.name;
        m_ClientData.weaponName = Player.Weapon.oneHandRanged.ToString(); // ???
    }

    public void Dispose() {
        if (!isClient) return;
        NameplateController.instance.ForgetSelectable(this);
    }

    public void UpdateServerPlayer(NetworkPlayerData _data) {
        if (isClient) return;

        if (network.usePredictiveSmoothing && _data.timestamp != null && m_LastFrameData != null && m_LastFrameData.timestamp != null) {
            if (m_LastFrameData == null) {
                m_LastFrameData = _data;
            }
            long _diff = long.Parse(_data.timestamp) - long.Parse(m_LastFrameData.timestamp);
            //Log(_data.timestamp+" Latency: "+_diff+"ms");
            m_Smooth = _diff / 1000.0F;
        } else {
            m_Smooth = moveSmooth;
        }

        m_InitialPos = transform.position;
        m_TargetPos = new Vector3(_data.pos.x, _data.pos.y, _data.pos.z);
        m_InitialEuler = transform.eulerAngles;
        m_TargetEuler = new Vector3(_data.rot.x, _data.rot.y, _data.rot.z);
        m_InitialRunning = m_Running;
        m_TargetRunning = _data.input.running;
        m_InitialStrafing = m_Strafing;
        m_TargetStrafing = _data.input.strafing;
        m_Grounded = _data.input.grounded;
        m_Attacking = _data.input.attacking;
        m_AttackCycle = _data.input.cycle;
        m_AttackSpeed = _data.input.attackSpeed;
        m_SpecialInput = _data.input.special;
        m_Weapon = _data.weaponName;
        m_Special = _data.specialName;
        
        m_UpdateTimer = 0;

        UpdateNameplate(_data.name, _data.health, _data.maxHealth, _data.lvl);
        
        m_LastFrameData = _data;
    }

    public void UpdatePlayerHealth(NetworkPlayerHitInfo _data) {
        if (!isClient) return;
        m_Player.data.player.health = _data.health;
    }
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
        m_ClientData.pos.x = transform.position.x;
        m_ClientData.pos.y = transform.position.y;
        m_ClientData.pos.z = transform.position.z;
        m_ClientData.rot.x = transform.eulerAngles.x;
        m_ClientData.rot.y = transform.eulerAngles.y;
        m_ClientData.rot.z = transform.eulerAngles.z;
        m_ClientData.input.running = m_PlayerController.runAnimation;
        m_ClientData.input.strafing = m_PlayerController.strafeAnimation;
        m_ClientData.input.grounded = m_PlayerController.grounded;
        m_ClientData.input.attacking = m_PlayerCombat.attacking;
        m_ClientData.input.cycle = m_Animator.GetBool("cycle");
        m_ClientData.input.attackSpeed = m_Animator.GetFloat("totalSpeed");
        m_ClientData.input.special = m_Animator.GetBool(m_PlayerCombat.special);
        m_ClientData.specialName = m_PlayerCombat.special; 
        m_ClientData.weaponName = m_Player.weapon.ToString();
        m_ClientData.maxHealth = m_Player.MaxHealth();
        m_ClientData.health = m_Player.data.player.health;
        m_ClientData.lvl = m_Player.data.player.level;
        m_ClientData.equipment = m_Player.data.equipment;

        UpdateNameplate(m_ClientData.name, m_ClientData.health, m_ClientData.maxHealth, m_ClientData.lvl);
      
        m_UpdateTimer += Time.deltaTime;
        if (m_UpdateTimer >= sendRate && m_IdleTimer < idleDetectionSeconds) {
            network.SendNetworkPlayer(m_ClientData);
            m_UpdateTimer = 0;
        }
    }

    private bool ClientHasNotChanged() {
        return  m_ClientData.pos.x == transform.position.x && 
                m_ClientData.pos.y == transform.position.y && 
                m_ClientData.pos.z == transform.position.z &&
                m_ClientData.rot.x == transform.eulerAngles.x && 
                m_ClientData.rot.y == transform.eulerAngles.y && 
                m_ClientData.rot.z == transform.eulerAngles.z &&
                m_ClientData.input.attacking == false;   
    }

    // Player not controlled by this client
    private void UpdateNetworkPlayer() {
        if (m_UpdateTimer > m_Smooth) return;
        m_UpdateTimer += Time.deltaTime;
        transform.position = Vector3.Lerp(m_InitialPos, m_TargetPos, m_UpdateTimer / m_Smooth);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(m_InitialEuler), Quaternion.Euler(m_TargetEuler), m_UpdateTimer / m_Smooth);
        m_Running = Mathf.Lerp(m_InitialRunning, m_TargetRunning, m_UpdateTimer / m_Smooth);
        m_Strafing = Mathf.Lerp(m_InitialStrafing, m_TargetStrafing, m_UpdateTimer / m_Smooth);       
        m_Animator.SetFloat("running", m_Running);
        m_Animator.SetFloat("strafing", m_Strafing);
        m_Animator.SetFloat("totalSpeed", m_AttackSpeed);
        m_Animator.SetBool("grounded", m_Grounded);
        m_Animator.SetBool(m_Weapon, m_Attacking);
        m_Animator.SetBool("cycle", m_AttackCycle);
        m_Animator.SetBool(m_Special, m_SpecialInput);
       
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
