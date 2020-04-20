using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.Game.Pooling;
using System.Threading.Tasks;

public class Dust : MonoBehaviour
{
    public Pool pool;
    Vector3 playerPos;
    Vector3 dustSpawn;
    Vector3 town;  
    float distance;
    float spawnTime;
    RaycastHit hit;
    

    
   
    


    void Start()
    {
        town = new Vector3(-378.0f, -3.0f, 235.0f);
        
    }

    // Update is called once per frame
    void Update()
    {        
       
        spawnTime += Time.deltaTime;
        
        if(spawnTime >= 0.1f){
            spawnTime = 0;
            Spawn();
        }
        
    }

    void Spawn(){

        
        playerPos = GameObject.FindWithTag("Player").transform.position;
        distance = Vector3.Distance(playerPos, town);
        if(distance > 350.0f){
            GameObject obj = pool.SpawnObject();
            dustSpawn = new Vector3(Random.Range(playerPos.x - 200.0f, playerPos.x + 200.0f), playerPos.y, Random.Range(playerPos.z - 200.0f, playerPos.z + 200.0f));
            if(obj){
                obj.transform.position = dustSpawn;
                if (Physics.Raycast(obj.transform.position, obj.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity)){
                    obj.transform.position = new Vector3(obj.transform.position.x, hit.point.y, obj.transform.position.z);
                }
             
                SpawnDelay(obj);  
                
                
            }

            
        }
    }

    async void SpawnDelay(GameObject otherObj){
        await Task.Delay(8000);
        pool.DisposeObject(otherObj);
          
    }
               
    
}


