using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTest : MonoBehaviour
{
   public GameObject target;
   Material movingMat;  
   public bool controlBool;
   

    void Start(){
        controlBool = false;
        movingMat = target.GetComponent<Renderer>().material;
    }

    void Update()
    {
        

        if(controlBool){
            movingMat.SetFloat("_movingBool", 1f);
        }
        else{
            movingMat.SetFloat("_movingBool", 0f);
        }
    }
}
