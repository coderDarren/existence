using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlideOpen : MonoBehaviour
{
    private Collider[] hit;
    public LayerMask mask;
    private GameObject botDoor;
    private GameObject topDoor;
    private Vector3 botClosed;
    private Vector3 topClosed;
    
    void Update()
    {
       hit = Physics.OverlapSphere(transform.position, 4, mask);    
        botDoor = transform.GetChild(1).gameObject;
        topDoor = transform.GetChild(2).gameObject;        

        if(hit.Length > 0){
            
            if(topDoor.transform.localPosition.y <= 4.7f){
                topDoor.transform.Translate(Vector3.up * Time.deltaTime * 7, Space.World);                
            }
            if(botDoor.transform.localPosition.y >= -1.8f){
                botDoor.transform.Translate(Vector3.down * Time.deltaTime * 7, Space.World);                
            }
        }
        else{
            if(topDoor.transform.localPosition.y >= 2.4f){
                topDoor.transform.Translate(Vector3.down * Time.deltaTime * 2.5f, Space.World);
            }
            if(botDoor.transform.localPosition.y <= 0.5f){
                botDoor.transform.Translate(Vector3.up * Time.deltaTime * 2.5f, Space.World);
            }
        }
        
        
    }
}