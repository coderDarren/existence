
using UnityEngine;

public class Mob : GameSystem
{
    public float smooth;

    private NetworkMobData m_Data;
    private Vector3 m_InitialPos;
    private Vector3 m_TargetPos;
    private Vector3 m_InitialRot;
    private Vector3 m_TargetRot;
    private float m_UpdateTimer;

#region Unity Functions
    private void Update() {
        if (m_UpdateTimer > smooth) return;
        m_UpdateTimer += Time.deltaTime;

        transform.position = Vector3.Lerp(m_InitialPos, m_TargetPos, m_UpdateTimer / smooth);
        transform.rotation = Quaternion.Lerp(Quaternion.Euler(m_InitialRot), Quaternion.Euler(m_TargetRot), m_UpdateTimer / smooth);
    }
#endregion

#region Public Functions
    public void Init(NetworkMobData _data) {
        m_Data = _data;
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
    }
#endregion

#region Private Functions

#endregion
}
