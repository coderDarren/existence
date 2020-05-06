using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    private GameObject player;
    private GameObject child;
    private Material material;
    private float distance;
    private float dissolve;

    // Update is called once per frame
    void Update()
    {
        
        player = GameObject.FindGameObjectWithTag("Player");
        child = transform.GetChild(0).gameObject;
        material = GetComponent<Renderer>().sharedMaterial;
        distance = Vector3.Distance(transform.position, player.transform.position);

        if(distance <= 4){
            dissolve += Time.deltaTime;            
            dissolve = Mathf.Clamp(dissolve, 0, 1);
            child.GetComponent<Renderer>().sharedMaterial.SetColor("lightColor", Color.green);
            material.SetFloat("Dissolve", dissolve);

        }
        else{
            dissolve -= Time.deltaTime;
            dissolve = Mathf.Clamp(dissolve, 0, 1);
            child.GetComponent<Renderer>().sharedMaterial.SetColor("lightColor", Color.red);
            material.SetFloat("Dissolve", dissolve);
        }
        
    }
}
