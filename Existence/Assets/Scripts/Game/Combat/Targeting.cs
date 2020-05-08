using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Targeting : MonoBehaviour
{
    public float radius;
    private GameObject[] m_Targets;
    private Collider[] m_Colliders;
    private float[] m_Distances;
    private int m_Amount;
    private int layer;
    private Vector3 m_Position; 

    void Start()
    {
        layer = 1 << 11;
        m_Amount = 0;
    }

    void FixedUpdate(){        
        m_Position = transform.position;
        m_Colliders = Physics.OverlapSphere(m_Position, radius, layer);
        m_Targets = new GameObject[m_Colliders.Length];
        m_Distances = new float[m_Colliders.Length];
    }


    public void Search(){

        for(int i=0; i < m_Colliders.Length; i++){
            
            m_Distances[i] = Vector3.Distance(m_Colliders[i].transform.position, m_Position);
            m_Targets[i] = m_Colliders[i].gameObject;
        }

        Array.Sort(m_Distances, m_Targets);
        
        if(m_Targets.Length != m_Amount){
            
            GetComponent<PlayerController>().FindTarget(m_Targets);
            m_Amount = m_Targets.Length;           
        }
        else{
            GetComponent<PlayerController>().CycleTarget();    
        }
    }
}
