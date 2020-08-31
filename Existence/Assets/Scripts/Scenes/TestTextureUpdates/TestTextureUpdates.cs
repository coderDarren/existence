
using UnityEngine;

public class TestTextureUpdates : MonoBehaviour
{
    public Texture2D storedTex;
    public Texture2D body, pants, sleeves, bodySkin, pantsSkin, sleevesSkin;
    public Utilities.RectBounds bodyBounds, pantsBounds, sleevesBounds;
        
    private Material m_Material;
    private Texture2D m_CopyTex;

#region Unity Functions
     private void Start() {
        m_Material = GetComponent<SkinnedMeshRenderer>().sharedMaterials[0];
        m_CopyTex = Instantiate(m_Material.mainTexture) as Texture2D;
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
            m_Material.mainTexture = m_CopyTex;
            StoreTex();
    }

    private async void StoreTex(){
        Color32[] _color = m_CopyTex.GetPixels32();
        storedTex.SetPixels32(_color);
        storedTex.Apply();
        m_Material.mainTexture = storedTex;       
    }
#endregion
}
