using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanarReflections3;

[RequireComponent (typeof (PlanarReflectionsCaster))]
public class ReflectionCulling : MonoBehaviour
{
    
    private PlanarReflectionsCaster target;
    private float m_ReflectDistance;

    private void Start(){
        target = GetComponent<PlanarReflectionsCaster>();
        m_ReflectDistance = 20.0f;
    }
    private void Update(){
        if(RenderExtensions.IsVisibleFrom(target.gameObject.GetComponent<Renderer>(), Camera.main) && Vector3.Distance(Camera.main.transform.position, transform.position) < m_ReflectDistance)
            target.enabled = true;        
        else
            target.enabled = false;
        
    }
}
