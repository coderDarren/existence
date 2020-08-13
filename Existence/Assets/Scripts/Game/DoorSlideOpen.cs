using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class DoorSlideOpen : MonoBehaviour
{
    public LayerMask mask;

    private GameObject m_Player;
    private float m_Distance;
    private Vector3 m_ClosedPos;
    private Vector3 m_OpenPos;
    private Vector3 m_CrackedPos;
    private Vector3 m_WorldPos;
    private bool m_Open;
    private bool m_Activated;
        
    
    private void Start(){
        m_ClosedPos = transform.localPosition;
        m_CrackedPos = new Vector3(m_ClosedPos.x, m_ClosedPos.y, m_ClosedPos.z + 0.3f);
        m_OpenPos = new Vector3(m_ClosedPos.x, m_ClosedPos.y, m_ClosedPos.z + 3.5f);
        m_Open = false;
        Debug.Log("ClosedPos:" + m_ClosedPos);
        Debug.Log("CrackedPos:" + m_CrackedPos);
        Debug.Log("OpenPos:" + m_OpenPos);
    }

    private void Update()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Distance = Vector3.Distance(transform.position, m_Player.transform.position);
        Debug.Log(m_Distance);
        
        if(m_Distance <= 6.0f && m_Open == false){         
            transform.localPosition = Vector3.Lerp(transform.localPosition, m_OpenPos, 1f * Time.deltaTime);

            if(!m_Activated)
                SlideOpen();
        
        }
        else if(m_Distance > 8.0f && m_Open == true){
            transform.localPosition = Vector3.Lerp(transform.localPosition, m_ClosedPos, 1f * Time.deltaTime);
            
            if(!m_Activated)
                SlideClosed();
        }       
    }

    private async void SlideOpen(){
        m_Activated = true;
        await Task.Delay(3000);
        m_Open = true;
        m_Activated = false;
    }
    
    private async void SlideClosed(){
        m_Activated = true;
        await Task.Delay(3000);
        m_Open = false;
        m_Activated = false;
    }
}