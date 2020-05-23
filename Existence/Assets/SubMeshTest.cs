using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMeshTest : MonoBehaviour
{
     
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Debug.Log(mesh.subMeshCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
