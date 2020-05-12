using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    private Collider[] hit;
    public LayerMask mask;
    private GameObject child;
    private Material material;
    private float dissolve;
    
    void Update()
    {
        hit = Physics.OverlapSphere(transform.position, 4, mask);    
        child = transform.GetChild(0).gameObject;
        material = GetComponent<Renderer>().sharedMaterial;


        

        if(hit.Length > 0){
            dissolve += Time.deltaTime;            
            dissolve = Mathf.Clamp(dissolve, 0, 1);
            child.GetComponent<Renderer>().sharedMaterial.SetColor("_EmissionColor", Color.green);
            material.SetFloat("Dissolve", dissolve);

        }
        else{
            dissolve -= Time.deltaTime;
            dissolve = Mathf.Clamp(dissolve, 0, 1);
            child.GetComponent<Renderer>().sharedMaterial.SetColor("_EmissionColor", Color.red);
            material.SetFloat("Dissolve", dissolve);
        }
        
    }
}
