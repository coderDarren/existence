using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
 public  class ArmorTest : MonoBehaviour
 {
    public GameObject prosthetic;
    public GameObject player;
    public string partString;    

    private Renderer mat;    
    private GameObject prostheticI;
    private GameObject targetParent;
    private GameObject targetPart;
    private Vector3[] vert;
    private Vector3[] baseVert;
    private Mesh mesh;

    void Start(){
        //mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //baseVert = mesh.vertices;
        targetPart = transform.Find(partString).gameObject;
        targetParent = transform.FindDeepChild(partString + "_Pros").gameObject;
        mat = targetPart.GetComponent<Renderer>();        
        
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
        
        /*vert = mesh.vertices;               
        for(int i = 0; i < vert.Length; i++){
            vert[i] += mesh.normals[i] * 0.1f * 0.1f * 0.1f * 0.2f;
             
            
            
        }z
        mesh.vertices = vert;*/
        mat.enabled = false;
        
        prostheticI = Instantiate(prosthetic, targetParent.transform);
        prostheticI.transform.localPosition = new Vector3(0f,0f,0f);           
        //prostheticI.transform.localRotation = targetParent.transform.rotation;

    }

    void ResetMesh(){
        //mesh.vertices = baseVert;
        Destroy(prostheticI);
        mat.enabled = true;
    }
 }
