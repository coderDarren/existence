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
    private Vector3[] equipVert;
    private Mesh equipMesh;
    private Mesh mesh;
    private RaycastHit hit;
    public LayerMask layerName;    

    void Start(){
        
        
        targetPart = transform.Find(partString).gameObject;
        targetParent = transform.FindDeepChild(partString + "_Pros").gameObject;
        mat = targetPart.GetComponent<Renderer>();
        mesh = targetPart.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //layerName = LayerMask.GetMask("Mesh");
        //Debug.Log(LayerMask.LayerToName(layerName)); 
        
    }
    
    void Update(){
        if(Input.GetKey(KeyCode.R)){
            EquipArmor();
        }
        if(Input.GetKeyDown(KeyCode.T)){
            ResetMesh();
        }
        
    }

    void EquipArmor(){
        
        prostheticI = Instantiate(prosthetic, targetParent.transform);
        prostheticI.transform.localPosition = new Vector3(0f,0f,0f);
        //prostheticI.transform.rotation =
        equipMesh = prostheticI.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
        equipVert = equipMesh.vertices;
        

        

        for(int i = 0; i < equipVert.Length; i++){
            equipMesh = prostheticI.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
            equipVert = equipMesh.vertices;
            //Debug.DrawLine(transform.TransformPoint(equipVert[i]), -equipMesh.normals[i] + transform.TransformPoint(equipVert[i]), Color.green, 20f);           
            Debug.DrawRay(prostheticI.transform.TransformPoint(equipVert[i]), prostheticI.transform.TransformDirection(equipMesh.normals[i]), Color.blue, 20f);
            
                
            if(Physics.Raycast(prostheticI.transform.TransformPoint(equipVert[i]), prostheticI.transform.TransformDirection(-equipMesh.normals[i]),out hit, 5, layerName)){
                equipMesh.vertices[i] -= equipMesh.normals[i];
                
                Debug.Log(hit.point + " : -hit");
                Debug.Log(hit.collider.gameObject.name);
            }
             
            if(Physics.Raycast(prostheticI.transform.TransformPoint(equipVert[i]), prostheticI.transform.TransformDirection(equipMesh.normals[i]),out hit, 5, layerName)){
                //equipMesh.vertices[i] = equipMesh.normals[i] * 0.1f;
                
                Debug.Log(hit.point + " : +hit");
                
            }
            
        }
        //mesh.vertices = vert;
        //mat.enabled = false;
        
                   
        //prostheticI.transform.localRotation = targetParent.transform.rotation;

    }

    void ResetMesh(){
        //mesh.vertices = baseVert;
        Destroy(prostheticI);
        mat.enabled = true;
    }
 }
