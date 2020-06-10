
using UnityEngine;
using UnityEngine.EventSystems;

public class InspectablePreviewItem : GameSystem, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Inspectable Events")]
    public bool displayOnHover=true;

    protected PreviewItemData m_PreviewItem;
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

    public PreviewItemData previewItem {
        get {
            return m_PreviewItem;
        }
    }

#region Interface Functions
    public void OnPointerEnter(PointerEventData _ped) {
        if (!displayOnHover) return;
        if (m_PreviewItem == null) return;
        if (!cursor) return;
        cursor.OpenPreviewHoverItem(m_PreviewItem);
    }

    public void OnPointerExit(PointerEventData _ped) {
        if (!displayOnHover) return;
        cursor.ClosePreviewHoverItem();
    }
#endregion
}
