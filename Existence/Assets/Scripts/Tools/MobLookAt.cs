using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobLookAt : MonoBehaviour
{
    public float range;
    
    private Transform m_Player;
    private Animator m_Animator;
    private float m_Distance;
    private bool m_Aggro;
    private bool m_Look;
    
    private void Start(){
        m_Animator = GetComponent<Animator>();
    }

    private void Update(){
        if (!m_Player)
            m_Player = GameObject.FindGameObjectWithTag("Player").transform;

        m_Distance = Vector3.Distance(transform.position, m_Player.position);
        
        MobLook();   
        Debug.Log(m_Distance);
        Debug.Log(m_Look);             
    }

    private void MobLook(){   
        if (m_Distance < range)
            m_Look = true;
        
        else if(m_Distance > 50){
            m_Aggro = false;
            m_Look = false;
        }

        if (m_Look){
            if (!m_Aggro){                        
                m_Aggro = true;
                m_Animator.SetTrigger("Aggro");
            }
            transform.LookAt(m_Player);
        }
    }

    public void PlaySoundEffect(){
        GetComponent<AudioSource>().Play();
    }
}
