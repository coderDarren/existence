
using UnityEngine;

/* 
 * Not to be confused with EquipmentPage..
 * EquipmentWindow represents a subpage within the EquipmentPage (Gear, Prosthetics, Augments)
 */
[RequireComponent(typeof(CanvasGroup))]
public abstract class EquipmentWindow : GameSystem
{
    protected CanvasGroup m_Canvas;
    protected PlayerData m_Data;

#region Overridable Functions
    public abstract void InitWindow(PlayerData _data);
    public abstract void DisposeWindow();
    public abstract void DrawWindow();
    public abstract void EraseWindow();
#endregion
}
