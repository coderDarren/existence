
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Mob : Selectable
{
    public float smooth;
    public LayerMask ground;

    private NetworkController m_Network;
    private NetworkMobData m_Data;
    private PlayerController m_Controller;
    //private CanvasGroup m_NamePlate;
    private Vector3 m_InitialPos;
    private Vector3 m_TargetPos;
    private Vector3 m_InitialRot;
    private Vector3 m_TargetRot;
    private float m_UpdateTimer;
    private CapsuleCollider m_Collider;

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
    private void Update() {
        if (m_UpdateTimer > smooth) return;
        m_UpdateTimer += Time.deltaTime;
        
        transform.position = Vector3.Lerp(m_InitialPos, m_TargetPos, m_UpdateTimer / smooth);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(m_InitialRot), Quaternion.Euler(m_TargetRot), m_UpdateTimer / smooth);
        FindGroundPos();
    }
#endregion

#region Public Functions
    public void Init(NetworkMobData _data) {
        m_Collider = GetComponent<CapsuleCollider>();
        m_Data = _data;
        m_Nameplate = new NameplateData();
        m_Nameplate.name = m_Data.name;
        m_Controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void UpdateData(NetworkMobData _data) {
        m_Data = _data;
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
#endregion
}
