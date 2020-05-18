using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
 public  class ArmorTest : MonoBehaviour
 {
    public GameObject armorPiece;
    public bool isClient;

    private GameObject player;
    private GameObject networkPlayer;
    private Transform targetBone;
    private Transform oldBone;
    private Transform oldBoneNetwork;
    private Transform newBone;
    private Transform parentBone;
    private Transform childBone;
    private BoneWeight[] weight;
    private SkinnedMeshRenderer skin;
    private Mesh mesh;
    private Vector3[] vertices;
    private string boneName;
    private int targetIndex;

    

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");
        networkPlayer = GameObject.FindGameObjectWithTag("NetworkPlayer");
        boneName = armorPiece.transform.GetChild(0).transform.name;        
        oldBone = player.transform.FindDeepChild(boneName);        
        parentBone = oldBone.parent;
        //childBone = oldBone.GetChild(0);
        targetBone = oldBone;
        mesh = new Mesh();
        skin = player.GetComponentInChildren<SkinnedMeshRenderer>();
        
        if(!isClient){
            oldBoneNetwork = networkPlayer.transform.FindDeepChild(boneName);
            targetBone = oldBoneNetwork;
            skin = networkPlayer.GetComponentInChildren<SkinnedMeshRenderer>();
        }
        
        skin.BakeMesh(mesh);
        
        Debug.Log(boneName);
        //Debug.Log(skin);  
             

    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Z)){
            Equip();
        }
    }

    void Equip(){
               
        for(int i = 0; i < skin.bones.Length; i++){
            if(skin.bones[i].name == boneName){
                targetIndex = i;
            }
        }

        
        GameObject equippedArmor = Instantiate(armorPiece, targetBone.position, targetBone.rotation);        
        //newBone = equippedArmor.transform.Find(boneName);
        //newBone.localScale = targetBone.localScale;        
        equippedArmor.transform.parent = parentBone.transform.GetChild(0);
        //childBone.SetParent(newBone);
        equippedArmor.transform.GetChild(0).transform.localPosition = targetBone.localPosition;
        equippedArmor.transform.GetChild(0).transform.localRotation = targetBone.localRotation;
        equippedArmor.transform.GetChild(0).transform.localScale = targetBone.localScale;
        //Destroy(newBone.gameObject);
    }
 }
