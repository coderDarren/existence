
using UnityEngine;

public class TestTextureUpdates : MonoBehaviour
{
    public Texture2D body1, body2, pants1, pants2, armor;
    public Utilities.RectBounds bodyBounds, pantsBounds, armorBounds;
    
    private Material m_Material;

#region Unity Functions
    private void Start() {
        m_Material = GetComponent<MeshRenderer>().materials[0];
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            m_Material.mainTexture = (Texture)Utilities.InsertTextureIntoTextureBounds(body1, (Texture2D)m_Material.mainTexture, bodyBounds);
        }
        
        if (Input.GetKeyDown(KeyCode.W)) {
            m_Material.mainTexture = (Texture)Utilities.InsertTextureIntoTextureBounds(body2, (Texture2D)m_Material.mainTexture, bodyBounds);
        }
        
        if (Input.GetKeyDown(KeyCode.E)) {
            m_Material.mainTexture = (Texture)Utilities.InsertTextureIntoTextureBounds(pants1, (Texture2D)m_Material.mainTexture, pantsBounds);
        }
        
        if (Input.GetKeyDown(KeyCode.R)) {
            m_Material.mainTexture = (Texture)Utilities.InsertTextureIntoTextureBounds(pants2, (Texture2D)m_Material.mainTexture, pantsBounds);
        }

        if (Input.GetKeyDown(KeyCode.T)) {
            m_Material.mainTexture = (Texture)Utilities.InsertTextureIntoTextureBounds(armor, (Texture2D)m_Material.mainTexture, armorBounds);
        }
    }
#endregion
}
