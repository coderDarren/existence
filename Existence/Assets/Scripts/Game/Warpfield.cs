using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warpfield : MonoBehaviour
{
    public GameObject destination;
    public Material skyBox;
    
    private Vector3 m_Difference;
    private Vector3 m_PosDiff;
    private GameObject m_Player;
    private CharacterController m_Controller;
    private float m_Dot;
    private float m_RotDiff;
    private bool m_Colliding;

    private void Update(){
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Controller = m_Player.GetComponent<CharacterController>();
        Debug.Log(m_Player.transform.position);
        Debug.Log(m_PosDiff);

        if(m_Colliding){
            m_Difference = m_Player.transform.position - transform.position;
            m_Dot = Vector3.Dot(transform.up, m_Difference);

            if(m_Dot < 0f){
                
                m_RotDiff = -Quaternion.Angle(transform.rotation, destination.transform.rotation);                
                m_RotDiff += 180;
                m_Player.transform.Rotate(Vector3.up, m_RotDiff);
                m_PosDiff = Quaternion.Euler(0f, m_RotDiff, 0f) * m_Difference;
                m_Controller.enabled = false;
                m_Controller.transform.position = destination.transform.position + m_PosDiff;
                RenderSettings.skybox = skyBox;
                m_Controller.enabled = true;                
                m_Colliding = false;
            }
        }
    }
    private void OnTriggerEnter(Collider col){
        if(col.tag == "Player"){
            m_Colliding = true;
        }        
    }

    private void OnTriggerExit(Collider col){
        
        if(col.tag == "Player"){
            m_Colliding = false;
        }        
    }
}
