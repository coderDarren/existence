using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloController : Selectable
{
    public GameObject[] portal;
    public Camera[] portalSceneCamera;
    public Material[] sceneMat;
    public GameObject holoWindow;
    

    private void Start(){
        
            
        
    }

    public void Beach(){
        for(int i=0;i<portal.Length;i++){
            if(portal[i].name == "Beach"){
                portal[i].SetActive(true);
                //holoWindow.GetComponent<Renderer>().material = sceneMat[i];
                /*if(portalSceneCamera[i].targetTexture != null)
                    portalSceneCamera[i].targetTexture.Release();
                portalSceneCamera[i].targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
                sceneMat[i].mainTexture = portalSceneCamera[i].targetTexture;*/
            }
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
        //holoWindow.GetComponent<Renderer>().material = sceneMat[sceneMat.Length - 1];
        this.gameObject.SetActive(false);
    }
}
