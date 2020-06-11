using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleChecker : MonoBehaviour
{
    private ParticleSystem m_DustSpawner;
    private ParticleSystem.Particle[] m_Outside;
    private Vector3 particlePos;

    void Start()
    {
        m_DustSpawner = GetComponent<ParticleSystem>();
        m_Outside = new ParticleSystem.Particle[1000];   
    }

    // Update is called once per frame
    void Update()
    {
        int numExit = m_DustSpawner.GetParticles(m_Outside);
        
        for (int i = 0; i < numExit; i++){

            ParticleSystem.Particle p = m_Outside[i];
            
            particlePos = transform.TransformPoint(p.position);
            if(Vector3.Distance(particlePos, GameObject.FindGameObjectWithTag("Player").transform.position) > 100){
                
                p.remainingLifetime = -0.1f;
            }
            
            m_Outside[i] = p;
        }

        m_DustSpawner.SetParticles(m_Outside);
    }
}
