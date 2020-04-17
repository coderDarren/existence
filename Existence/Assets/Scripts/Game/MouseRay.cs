using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRay : MonoBehaviour
{
    RaycastHit hit;
    Ray mouseRay;
    bool leftHit;
    bool rightHit;
    GameObject player;
    
    void Start()
    {
        
    }
   
    void Update()
    {
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);         

        if(Input.GetMouseButtonDown(0)){
            if(Physics.Raycast(mouseRay, out hit, 5.0f)){
                Debug.Log(leftHit);
            }           
        }
        
        if(Input.GetMouseButtonDown(1)){
            if(Physics.Raycast(mouseRay, out hit, 5.0f)){                
                if(hit.collider.tag == ("NPC")){
                    hit.collider.gameObject.GetComponent<NPCController>().Clicked(20000);
                    
                    rightHit = true;                    
                }
            }            
        }

        if(rightHit){
            Debug.Log(rightHit);
            Debug.Log(hit);
        } 
        rightHit = false;  
    }
}
