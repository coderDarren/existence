using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorLerp : MonoBehaviour
{
    public Vector3[] floor;
    public float floorDelay;
    public float range;

    private int m_Next;
    private int m_Direction;
    private float m_Timer;

    private void Start(){
        m_Next = 1;
        m_Timer = 0;
        m_Direction = 1;
    }


    private void Update()
    {   
        if(Vector3.Distance(transform.localPosition, floor[(floor.Length - 1)]) <= range)
            m_Direction = -1;
        
        if(Vector3.Distance(transform.localPosition, (floor[0])) <= range)
            m_Direction = 1;
        
        if(Vector3.Distance(transform.localPosition, floor[m_Next]) <= range)
            m_Timer += Time.deltaTime;
        
        if(m_Timer > floorDelay){
                m_Next += m_Direction;
                m_Timer = 0;
            }

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, floor[m_Next], 1.5f * Time.deltaTime);
    }
}
