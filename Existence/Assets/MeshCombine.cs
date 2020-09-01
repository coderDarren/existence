using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombine : MonoBehaviour
{
    private MeshFilter[] m_Meshes;
    private CombineInstance[] m_Instance;
    private Material[] m_InstanceMats;
   // Matrix4x4 pTransform;

    private void Start(){

        m_Meshes = GetComponentsInChildren<MeshFilter>();
        m_Instance = new CombineInstance[m_Meshes.Length];
        m_InstanceMats = transform.GetChild(0).GetComponent<Renderer>().sharedMaterials;  
        //pTransform = transform.worldToLocalMatrix;
        
        
        for(int i = 0; i < m_Meshes.Length; i++){
            m_Instance[i].mesh = m_Meshes[i].sharedMesh;
            m_Instance[i].transform = m_Meshes[i].transform.localToWorldMatrix * transform.worldToLocalMatrix;          
            m_Meshes[i].gameObject.SetActive(false);
        }
       

        MeshFilter meshFilter = transform.gameObject.AddComponent<MeshFilter>();
        GetComponent<MeshFilter>().mesh = new Mesh();
        GetComponent<MeshFilter>().mesh.CombineMeshes(m_Instance);
        
        MeshCollider meshCollider = transform.gameObject.AddComponent<MeshCollider>();
        GetComponent<Renderer>().sharedMaterials = m_InstanceMats;
        transform.gameObject.SetActive(true);
        
 
    }

}
