using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
 public  class ArmorTest : MonoBehaviour
 {
    public GameObject prosthetic;
    public GameObject player;

    private Transform bone;
    private Vector3[] vert;
    private Vector3[] baseVert;
    private Mesh mesh;

    void Start(){
        mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        baseVert = mesh.vertices;
        bone = player.transform.Find("mixamorig:LeftForeArm");
    }
    
    void Update(){
        if(Input.GetKeyDown(KeyCode.R)){
            EquipArmor();
        }
        if(Input.GetKeyDown(KeyCode.T)){
            ResetMesh();
        }
    }

    void EquipArmor(){
        
        vert = mesh.vertices;
        for(int i = 0; i < vert.Length; i++){
            vert[i] -= mesh.normals[i] * 0.1f * 0.1f * 0.1f * 0.2f;
            Debug.Log(vert[i]);
        }
        mesh.vertices = vert;
        
        Instantiate(prosthetic, bone.position, bone.rotation);
        

    }

    void ResetMesh(){
        mesh.vertices = baseVert;
    }
 }
