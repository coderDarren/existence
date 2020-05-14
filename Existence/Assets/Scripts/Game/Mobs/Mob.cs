
using UnityEngine;
using UnityEngine.UI;

public class Mob : GameSystem
{
    public float smooth;
    public Text name;
    public Image healthBar;
    public Image energyBar;

    private NetworkController m_Network;
    private NetworkMobData m_Data;
    private PlayerController m_Controller;
    private CanvasGroup m_NamePlate;
    private Vector3 m_InitialPos;
    private Vector3 m_TargetPos;
    private Vector3 m_InitialRot;
    private Vector3 m_TargetRot;
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

#region Unity Functions
    private void Update() {
        if (m_UpdateTimer > smooth) return;
        m_UpdateTimer += Time.deltaTime;
        
        transform.position = Vector3.Lerp(m_InitialPos, m_TargetPos, m_UpdateTimer / smooth);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(m_InitialRot), Quaternion.Euler(m_TargetRot), m_UpdateTimer / smooth);
       DrawHealth();
    }

    private void DrawHealth(){
        /*if (m_Controller.m_CurrentTarget == GetComponentInChildren<SkinnedMeshRenderer>().gameObject || m_Controller.m_Target == GetComponentInChildren<SkinnedMeshRenderer>().gameObject)
            m_NamePlate.alpha = 1f;
        else
            m_NamePlate.alpha = 0f;
        */
    }
#endregion

#region Public Functions
    public void Init(NetworkMobData _data) {
        m_Data = _data;
        name.text = m_Data.name;
        healthBar.fillAmount = m_Data.health / (float)m_Data.maxHealth;
        energyBar.fillAmount = m_Data.energy / (float)m_Data.maxEnergy;
        m_NamePlate = GetComponent<CanvasGroup>();
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
        healthBar.fillAmount = m_Data.health / (float)m_Data.maxHealth;
        energyBar.fillAmount = m_Data.energy / (float)m_Data.maxEnergy;
        m_UpdateTimer = 0;
    }

    public void Hit(int _dmg) {
        if (!network) return;
        NetworkMobHitInfo _hitInfo = new NetworkMobHitInfo(m_Data.id, _dmg);
        network.HitMob(_hitInfo);
    }
#endregion

#region Private Functions

#endregion
}
