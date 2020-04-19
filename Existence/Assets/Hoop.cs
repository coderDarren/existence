using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoop : MonoBehaviour
{
    bool faggot;

    void Start(){
        faggot = false;
    }

    void Update (){
        if(faggot){Debug.Log("Fuck you gaylord");}
    }
    
    
    void OnTriggerEnter(){
       faggot = true;
   }
}
