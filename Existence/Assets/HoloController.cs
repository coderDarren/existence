using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloController : Selectable
{
    public GameObject[] portal;

    public void Beach(){
        for(int i=0;i<portal.Length;i++){
            if(portal[i].name == "Beach")
                portal[i].SetActive(true);
            else{
                portal[i].SetActive(false);
            }
        }
        this.gameObject.SetActive(false);
    }

    public void Combat(){
        for(int i=0;i<portal.Length;i++){            
                portal[i].SetActive(false);            
        }
        this.gameObject.SetActive(false);
    }
}
