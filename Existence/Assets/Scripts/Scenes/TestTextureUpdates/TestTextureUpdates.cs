
using UnityEngine;

public class TestTextureUpdates : MonoBehaviour
{
    public Texture2D storedTex;  
    public Texture2D body, pants, sleeves, bodySkin, pantsSkin, sleevesSkin;
    public Utilities.RectBounds bodyBounds, pantsBounds, sleevesBounds;
        
    private Renderer[] m_Renderer;
    private Texture2D m_CopyTex;

#region Unity Functions
     private void Start() {
        m_Renderer = GetComponentsInChildren<SkinnedMeshRenderer>();
        for(int i=0; i < m_Renderer.Length; i++){
            if(m_Renderer[i].material.mainTexture == null)
                m_Renderer[i].material.mainTexture = storedTex;
        }
        m_CopyTex = Instantiate(m_Renderer[0].sharedMaterial.mainTexture) as Texture2D;
    }

    private void Update() {
        if(!body){
            body = bodySkin;
            EquipTex(body, bodyBounds);
        }

        if(!pants){
            pants = pantsSkin;
            EquipTex(pants, pantsBounds); 
        }

        if(!sleeves){
            sleeves = sleevesSkin;
            EquipTex(sleeves, sleevesBounds);
        }

// Delete this section once equip slots call proper function.
        if (Input.GetKeyDown(KeyCode.V)) 
            EquipTex(body, bodyBounds);   
        
        if (Input.GetKeyDown(KeyCode.B))
            EquipTex(pants, pantsBounds);

        if (Input.GetKeyDown(KeyCode.N))
            EquipTex(sleeves, sleevesBounds);       
    }

    public void EquipChest(){
        EquipTex(body, bodyBounds);
    }

    public void EquipPants(){
        EquipTex(pants, pantsBounds);
    }

    public void EquipSleeves(){
        EquipTex(sleeves, sleevesBounds); 
    }

    private void EquipTex(Texture2D _slot, Utilities.RectBounds _bounds){
        m_CopyTex = (Texture2D)Utilities.InsertTextureIntoTextureBounds(_slot, (Texture2D)m_CopyTex, _bounds);
        for(int i=0; i < m_Renderer.Length; i++){
            m_Renderer[i].material.mainTexture = m_CopyTex;
        }
        //StoreTex();
    }

    private async void StoreTex(){ //Potential for storing player's current armor texture, unsure if its neccessary. Disabled for now
       /* Color32[] _color = m_CopyTex.GetPixels32();
        storedTex.SetPixels32(_color);
        storedTex.Apply();
        for(int i=0; i < m_Renderer.Length; i++){
            m_Renderer[i].material.mainTexture = storedTex;
        }     */  
    }
#endregion
}
