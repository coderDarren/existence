using System.Collections.Generic;
using UnityEngine;

/*
 * EquipmentController manages all players' visual equipment, networked and self
 * Responsible for translating item information into physical/visual equipment
 */
public class EquipmentController : GameSystem
{
    public Transform EQUIP_LOC_LHAND;
    public Transform EQUIP_LOC_RHAND;
    public Transform EQUIP_LOC_BACK;

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
        Transform _loc = _equipment.location == GearType.BACK ? EQUIP_LOC_BACK :
                         _equipment.location == GearType.L_HAND ? EQUIP_LOC_LHAND : EQUIP_LOC_RHAND;

        // now we can equip!!
        GameObject _obj = Instantiate(_equipment.prefab);
        _obj.transform.parent = _loc;
        _obj.transform.position = Vector3.zero;
        _obj.transform.rotation = Quaternion.identity;
        _obj.transform.localScale = Vector3.one;
    }

    public void Unequip(IItem _item) {

    }
#endregion
}
