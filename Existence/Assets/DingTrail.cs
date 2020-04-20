using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DingTrail : MonoBehaviour
{
    public GameObject trail;
    ParticleSystem ps;
    int num;
    int i;
    Vector3 particleLocA, particleLocB, particleLocC, particleLocD, particleLocE, particleLocF;
    GameObject[] trails;
    float destroyTimer;    

    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1)){
            LevelUp();
        }

        if(destroyTimer > 5.3f){
            for(int x = 0; x < 6; x++)
                Destroy(trails[x].gameObject);
        }

        trails = GameObject.FindGameObjectsWithTag("DingTrail");
        ps=GetComponent<ParticleSystem>();        
        ParticleSystem.Particle[] particles=new ParticleSystem.Particle[ps.particleCount];
        num = ps.GetParticles (particles);
        
        if(num > 0){
            particleLocA = transform.position + particles[0].position;
            trails[0].transform.position = particleLocA;
            particleLocB = transform.position + particles[1].position;
            trails[1].transform.position = particleLocB;
            particleLocC = transform.position + particles[2].position;
            trails[2].transform.position = particleLocC;
            particleLocD = transform.position + particles[3].position;
            trails[3].transform.position = particleLocD;
            particleLocE = transform.position + particles[4].position;
            trails[4].transform.position = particleLocE;
            particleLocF = transform.position + particles[5].position;
            trails[5].transform.position = particleLocF;
        }
        destroyTimer += Time.deltaTime;
       

        Debug.Log(trails.Length);

    }
    
    void LevelUp(){
        for(i = 0; i < 6; i++)
            Instantiate(trail, transform.position, Quaternion.identity);
            
    }

}
