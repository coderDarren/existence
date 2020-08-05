using System.Collections;
using UnityEngine;

public class ProstheticWindow : EquipmentWindow
{
    public EquipmentSlot[] slots;

    private EquipmentPage m_Controller;
    private Hashtable m_SlotsHash;
    
#region Override Functions
    public override void InitWindow(EquipmentPage _controller, PlayerData _data) {
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
            if (!m_SlotsHash.ContainsKey(_armor.slotType)) continue;
            EquipmentSlot _slot = (EquipmentSlot)m_SlotsHash[_armor.slotType];
            _slot.AssignItem(_armor);
        }
    }
    
    public override void EraseWindow() {
        m_Canvas.alpha = 0;
        foreach (EquipmentSlot _slot in slots) {
            _slot.ClearItem();
        }
    }
#endregion
}
