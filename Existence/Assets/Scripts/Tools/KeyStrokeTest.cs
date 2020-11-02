using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyStrokeTest : MonoBehaviour
{    
    public KeyCode stroke;
    public GameObject target;
    
    void Update()
    {
        if(Input.GetKeyDown(stroke))
            RunEvent();
    }

    private void RunEvent(){
        target.GetComponent<Animator>().SetTrigger("Test");
    }
}
