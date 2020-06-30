using System.Collections;
using UnityEngine;

public class GearWindow : EquipmentWindow
{
    public EquipmentSlot[] slots;

    private Hashtable m_SlotsHash;
    
#region Override Functions
    public override void InitWindow(PlayerData _data) {
        m_Canvas = GetComponent<CanvasGroup>();
        m_Data = _data;
        m_SlotsHash = new Hashtable();
        foreach (EquipmentSlot _slot in slots) {
            m_SlotsHash.Add((int)_slot.type, _slot);
        }
    }

    public override void DisposeWindow() {
        m_Canvas.alpha = 0;
    }
    
    public override void DrawWindow() {
        m_Canvas.alpha = 1;

        foreach (ArmorItemData _armor in m_Data.equipment.armor) {
            EquipmentSlot _slot = (EquipmentSlot)m_SlotsHash[_armor.slotType];
            _slot.AssignItem(_armor);
        }

        foreach (WeaponItemData _weapon in m_Data.equipment.weapons) {
            EquipmentSlot _slot = (EquipmentSlot)m_SlotsHash[_weapon.slotType];
            _slot.AssignItem(_weapon);
        }
    }
    
    public override void EraseWindow() {
        m_Canvas.alpha = 0;
    }
#endregion
}
