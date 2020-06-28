
using UnityEngine;
using UnityEngine.EventSystems;

public class InspectableItem : GameSystem, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Inspectable Events")]
    public bool displayOnHover;
    public bool displayDetail;

    protected IItem m_Item;
    protected CursorController m_Cursor;
    protected bool m_Hovering;

    protected CursorController cursor {
        get {
            if (!m_Cursor) {
                m_Cursor = CursorController.instance;
            }
            if (!m_Cursor) {
                LogWarning("Trying to access cursor, but no instance of CursorController was found.");
            }
            return m_Cursor;
        }
    }

    public IItem item {
        get {
            return m_Item;
        }
    }

#region Unity Functions
    private void OnDisable() {
        Dispose();
    }
#endregion

#region Overrideable Functions
    protected virtual void Dispose() {
        if (!cursor) return;
        if (cursor.hoverItem == m_Item) {
            cursor.CloseHoverItem();
        }
        m_Hovering = false;
    }
#endregion

#region Interface Functions
    public void OnPointerEnter(PointerEventData _ped) {
        if (!displayOnHover) return;
        if (m_Item == null) return;
        if (!cursor) return;
        cursor.OpenHoverItem(m_Item);
        m_Hovering = true;
    }

    public void OnPointerExit(PointerEventData _ped) {
        if (!displayOnHover) return;
        if (m_Item == null) return;
        cursor.CloseHoverItem();
        m_Hovering = false;
    }
#endregion
}
