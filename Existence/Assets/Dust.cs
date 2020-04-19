using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dust : MonoBehaviour
{
    Vector3 playerPos;
    Vector3 dustSpawn;
    Vector3 town;
    float terrainY;
    float distance;
    public GameObject dust;
    GameObject terrain;
    GameObject[] dustArray;


    void Start()
    {
        terrain = GameObject.FindWithTag("Terrain");
        town = new Vector3(-378.0f, -3.0f, 235.0f);
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = GameObject.FindWithTag("Player").transform.position;
        distance = Vector3.Distance(playerPos, town);
        dustArray = GameObject.FindGameObjectsWithTag("Dust");

        if(distance > 350.0f && dustArray.Length < 15){
            dustSpawn = new Vector3(Random.Range(playerPos.x - 200.0f, playerPos.x + 200.0f), playerPos.y, Random.Range(playerPos.z - 200.0f, playerPos.z + 200.0f));
            Instantiate(dust, dustSpawn, Quaternion.identity);
        }
    }
}
