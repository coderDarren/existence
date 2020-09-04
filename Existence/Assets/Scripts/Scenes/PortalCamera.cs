using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public GameObject localPortal;
    public GameObject targetPortal;

    private GameObject m_Player;
    private GameObject m_MainCam;
    private Vector3 m_Offset;
    private Vector3 m_Direction;
    private Vector3 m_PosDiff;
    private Quaternion m_RotDiff;
    private float m_AngleDiff;

    private void FixedUpdate(){
        m_MainCam = GameObject.FindGameObjectWithTag("MainCamera");       
        m_Offset = m_MainCam.transform.position - targetPortal.transform.position;
        m_Offset.y *= -1;
        transform.position = localPortal.transform.position - m_Offset;
        m_AngleDiff = Quaternion.Angle(localPortal.transform.rotation, targetPortal.transform.rotation);
        m_RotDiff = Quaternion.AngleAxis(m_AngleDiff, Vector3.up);
        m_Direction = m_RotDiff * m_MainCam.transform.forward;
        transform.rotation = Quaternion.LookRotation(new Vector3(-m_Direction.x,m_Direction.y,-m_Direction.z), Vector3.up);
    }
}
