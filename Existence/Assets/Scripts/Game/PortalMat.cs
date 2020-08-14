using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalMat : MonoBehaviour
{   
    public Camera portalCamera;
    public Material portalMat; 

    private void Start(){
        
        if(portalCamera.targetTexture != null)
            portalCamera.targetTexture.Release();
            
        portalCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        portalMat.mainTexture = portalCamera.targetTexture;
    }
}
