using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    public bool appear;
    
    private Material m_Material;
    private GameObject m_Child;
    private float m_Dissolve;
    private float m_Distance;
    private float m_Timer;
    
    private void Start(){
        m_Child = transform.GetChild(1).gameObject;
        m_Material = m_Child.GetComponent<SkinnedMeshRenderer>().sharedMaterial;
        m_Dissolve = 1;        
        m_Material.SetFloat("Dissolve", m_Dissolve);
        
    }
    
    void Update(){    
        m_Distance = GetComponent<MobLookAt>().distance;

        if(m_Distance <= 45){
                m_Dissolve -= (Time.deltaTime / 3);            
                m_Dissolve = Mathf.Clamp(m_Dissolve, 0, 1);
            if(appear)       
                m_Material.SetFloat("Dissolve", m_Dissolve);
            if(!appear)
                if(m_Dissolve <= 0.2f)
                    m_Child.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
    }
}
