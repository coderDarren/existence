
using UnityEngine;

public class GearWindow : EquipmentWindow
{
    public EquipmentSlot[] slots;
    
#region Override Functions
    public override void InitWindow(PlayerData _data) {
        m_Canvas = GetComponent<CanvasGroup>();
        m_Data = _data;
    }

    public override void DisposeWindow() {
        m_Canvas.alpha = 0;
    }
    
    public override void DrawWindow() {
        m_Canvas.alpha = 1;
    }
    
    public override void EraseWindow() {
        m_Canvas.alpha = 0;
    }
#endregion
}
