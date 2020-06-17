using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class NPCTurn : MonoBehaviour
{
    private Animator m_Animator;
    private GameObject m_Player;
    private Vector3 m_PlayerPos;
    private Vector3 m_Direction;
    private Vector3 m_CurrentPos;
    private Quaternion m_LookRotation;
    private Quaternion m_CurrentRotation;
    private Quaternion m_StartRotation;
    public bool m_Approached;
    private float m_TurnSpeed;

    private void Start(){
        
        m_Animator = GetComponent<Animator>();
        m_StartRotation = transform.rotation;
        m_TurnSpeed = 5.0f * Time.deltaTime;
    }

    private void Update(){
        m_CurrentPos = transform.position;
        m_CurrentRotation = transform.rotation;
        Turn();
        if(m_Approached)
            Clicked();
    }
    
    public async void Clicked(){
        m_Animator.SetTrigger("Approached");
        m_Player = GameObject.FindWithTag("Player");
        m_PlayerPos = m_Player.transform.position;
        m_Direction = m_CurrentPos - m_PlayerPos;
               
        await Task.Delay(15000);
        m_Approached = false;
        

    }

    private void Turn(){ 
        m_LookRotation = Quaternion.LookRotation(-m_Direction);
        if(m_Approached)
            transform.rotation = Quaternion.Slerp(m_CurrentRotation, m_LookRotation, m_TurnSpeed);        
        else
            transform.rotation = Quaternion.Slerp(m_CurrentRotation, m_StartRotation, m_TurnSpeed);
        
        Debug.Log(m_PlayerPos);
        Debug.Log(m_LookRotation); 
    }

    
}
