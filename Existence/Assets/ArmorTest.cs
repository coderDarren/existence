using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
 public  class ArmorTest : MonoBehaviour
 {
    public GameObject augment;
    
    private Vector3[] vert;
    private Vector3[] baseVert;
    private Mesh mesh;

    void Start(){
        mesh = GetComponent<MeshFilter>().mesh;
        baseVert = mesh.vertices;
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
        Debug.Log("ping");

        for(int i = 0; i < vert.Length; i++){
            vert[i] -= mesh.normals[i] * 2f;
            Debug.Log(vert[i]);
        }
        
        mesh.vertices = vert;
    }

    void ResetMesh(){
        mesh.vertices = baseVert;
    }
 }
