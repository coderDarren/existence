
using UnityEngine;
using UnityEngine.EventSystems;

public class InspectableItem : GameSystem, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Inspectable Events")]
    public bool displayOnHover;
    public bool displayDetail;

    protected ItemData m_Item;
    protected CursorController m_Cursor;

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

    public ItemData item {
        get {
            return m_Item;
        }
    }

#region Interface Functions
    public void OnPointerEnter(PointerEventData _ped) {
        if (!displayOnHover) return;
        if (m_Item == null) return;
        if (!cursor) return;
        cursor.OpenHoverItem(m_Item);
    }

    public void OnPointerExit(PointerEventData _ped) {
        if (!displayOnHover) return;
        if (m_Item == null) return;
        cursor.CloseHoverItem();
    }
#endregion
}
