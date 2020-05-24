using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPlacing : MonoBehaviour
{

    public GameObject player;
    private Transform bone;

    void Update()
    {
        bone = player.transform.Find("mixamorig:Hips");
        Debug.Log(bone.position); 
    }
    
}
