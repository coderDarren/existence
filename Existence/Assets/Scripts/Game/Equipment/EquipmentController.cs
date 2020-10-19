using System.Collections.Generic;
using UnityEngine;

/*
 * EquipmentController manages all players' visual equipment, networked and self
 * Responsible for translating item information into physical/visual equipment
 */
public class EquipmentController : GameSystem
{    
    [Header("Gear Locations")]
    public Transform EQUIP_LOC_LHAND;
    public Transform EQUIP_LOC_RHAND;
    public Transform EQUIP_LOC_BACK;
    public Transform PROSTHETIC_LOC_LARM;

    [Header("Skin Textures")]
    public Texture2D bodySkin;
    public Texture2D sleevesSkin;
    public Texture2D pantsSkin; 
    public Texture2D storedTex;   
    
    [Header("Prosthetic Masks")]
    public SkinnedMeshRenderer MASK_L_ARM;

    private Session m_Session;
    private Renderer[] m_Renderer;
    private Texture2D m_CopyTex;
    private Texture2D m_SkinTex;

    // get Session with integrity
    private Session session {
        get {
            if (!m_Session) {
                m_Session = Session.instance;
            }
            if (!m_Session) {
                LogError("Trying to use Session, but no instance could be found.");
            }
            return m_Session;
        }
    }
    
#region Public Functions
    public void Equip(int _itemID) {
        BasicItemData _item = new BasicItemData();
        _item.def = new ItemData(_itemID);
        Equip(_item);
    } 

    public void Equip(IItem _item) {
        if (!session) return;
        
        // validate equipment
        if (!session.equipmentItems.ContainsKey(_item.def.id)) {
            LogWarning("Unable to equip item "+_item.def.id+"-"+_item.def.name+". No data was found.");
            return;
        }

        // further equipment validation
        List<EquipmentData.Props> _equipmentProps = (List<EquipmentData.Props>)session.equipmentItems[_item.def.id];
        if (_equipmentProps.Count == 0) {
            LogWarning("Unable to equip item "+_item.def.id+"-"+_item.def.name+". Data was initialized but not listed.");
            return;
        }

        // look for equipment that matches the player's info
        Log("Equipping "+_item.def.id+"-"+_item.def.name+". Choosing best from "+_equipmentProps.Count+" alternatives.");
        EquipmentData.Props _equipment = null;
        foreach (EquipmentData.Props _props in _equipmentProps) {
            bool _sexFits = _props.sex == Sex.AGNOSTIC || _props.sex == session.player.data.player.sex;
            bool _raceFits = _props.race == Race.AGNOSTIC || _props.race == session.player.data.player.race;
            if (_sexFits && _raceFits) {
                _equipment = _props;
                break;
            }
        }

        // validate equipment
        if (_equipment == null) {
            LogWarning("Unable to equip item "+_item.def.id+"-"+_item.def.name+". Could not find valid alternative for player.");
            return;
        }

        switch((int)_equipment.location){
            case 0:
            case 2:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
            case 11:
            case 12:
            case 13:
            case 14:
            EquipPrefab(_equipment);
            break;
            
            case 1:
            case 3:
            case 9:
            case 10:
            EquipTexture(_equipment);
            break;   
        }

        // grab the location
        /*Transform _loc = GetGearTransform(_equipment.location);
        Log("equipping at loc: "+_loc.gameObject.name);

        // now we can equip!!
        GameObject _obj = Instantiate(_equipment.prefab, Vector3.zero, Quaternion.identity);
        _obj.transform.parent = _loc;
        _obj.transform.localPosition = Vector3.zero;
        _obj.transform.localRotation = Quaternion.identity;
        _obj.transform.localScale = Vector3.one;

        UpdateProstheticMask(_equipment.location, false);*/
    }

    public void Unequip(int _itemID) {
        if (!session) return;
        
        // validate equipment
        if (!session.equipmentItems.ContainsKey(_itemID)) {
            LogWarning("Unable to unequip item "+_itemID+". No data was found.");
            return;
        }

        // further equipment validation
        List<EquipmentData.Props> _equipmentProps = (List<EquipmentData.Props>)session.equipmentItems[_itemID];
        if (_equipmentProps.Count == 0) {
            LogWarning("Unable to unequip item "+_itemID+". Data was initialized but not listed.");
            return;
        }

        Unequip(_equipmentProps[0].location);
    }

    public void Unequip(GearType _gearType) {
        if (!session) return;
        
        Transform _loc = GetGearTransform(_gearType);
        Log("unequipping from loc: "+_loc.gameObject.name);

        Transform _child = _loc.GetChild(0);
        if (!_loc.GetChild(0) || _loc.transform.childCount == 0) {
            LogWarning("Unable to equip "+_gearType+". No item exists here.");
            return;
        }

        Log("Successfully unequipped "+_gearType);
        Destroy(_child.gameObject);

        UpdateProstheticMask(_gearType, true);
    }
#endregion

#region Private Functions
    private Transform GetGearTransform(GearType _gearType) {
        Transform _ret = _gearType == GearType.BACK ? EQUIP_LOC_BACK :
                         _gearType == GearType.L_HAND ? EQUIP_LOC_LHAND : 
                         _gearType == GearType.R_HAND ? EQUIP_LOC_RHAND : PROSTHETIC_LOC_LARM;
        
        return _ret;
    }

    private void UpdateProstheticMask(GearType _gearType, bool _active) {
        switch((int)_gearType) {
            case 14: MASK_L_ARM.enabled = _active; break;
        }
    }

    private void EquipPrefab(EquipmentData.Props _equipment){
        Transform _loc = GetGearTransform(_equipment.location);
        Log("equipping at loc: "+_loc.gameObject.name);

        // now we can equip!!
        GameObject _obj = Instantiate(_equipment.prefab, Vector3.zero, Quaternion.identity);
        _obj.transform.parent = _loc;
        _obj.transform.localPosition = Vector3.zero;
        _obj.transform.localRotation = Quaternion.identity;
        _obj.transform.localScale = Vector3.one;

        UpdateProstheticMask(_equipment.location, false);
    }

    private void EquipTexture(EquipmentData.Props _equipment){

        m_Renderer = GetComponentsInChildren<SkinnedMeshRenderer>();
        for(int i=0; i < m_Renderer.Length; i++){
            if(m_Renderer[i].sharedMaterial.mainTexture == null)
                m_Renderer[i].material.mainTexture = storedTex;
        }
        
        m_CopyTex = Instantiate(m_Renderer[0].sharedMaterial.mainTexture) as Texture2D;        
        
        m_CopyTex = (Texture2D)Utilities.InsertTextureIntoTextureBounds(_equipment.tex, (Texture2D)m_CopyTex, _equipment.bounds);
        for(int i=0; i < m_Renderer.Length; i++){
            m_Renderer[i].material.mainTexture = m_CopyTex;
        }
           
    }

    private void UnequipTexture(EquipmentData.Props _equipment){
        m_CopyTex = Instantiate(m_Renderer[0].sharedMaterial.mainTexture) as Texture2D;        
        
        switch((int)_equipment.location){
            case 1:
            m_SkinTex = bodySkin;
            break;
            case 3:
            m_SkinTex = pantsSkin;
            break;
            case 9:
            case 10:
            m_SkinTex = sleevesSkin;
            break;
        }           
        m_CopyTex = (Texture2D)Utilities.InsertTextureIntoTextureBounds(m_SkinTex, (Texture2D)m_CopyTex, _equipment.bounds);
        for(int i=0; i < m_Renderer.Length; i++){
            m_Renderer[i].material.mainTexture = m_CopyTex;
        }
        
    }
        
    /*private async void StoreTex(){ //Potential for storing player's current armor texture, unsure if its neccessary. Disabled for now
        Color32[] _color = m_CopyTex.GetPixels32();
        storedTex.SetPixels32(_color);
        storedTex.Apply();
        for(int i=0; i < m_Renderer.Length; i++){
            m_Renderer[i].material.mainTexture = storedTex;
        }       
    }*/

    
    
#endregion
}
