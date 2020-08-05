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
    
    [Header("Prosthetic Masks")]
    public SkinnedMeshRenderer MASK_L_ARM;

    private Session m_Session;

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

        // grab the location
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
#endregion
}
