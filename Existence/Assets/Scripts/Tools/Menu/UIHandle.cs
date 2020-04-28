
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// UIHandle is responsible for two major window operations
///     1. Resizing
///     2. Dragging
/// </summary>
public class UIHandle : GameSystem, 
                        IPointerEnterHandler,
                        IPointerExitHandler,
                        IPointerDownHandler, 
                        IPointerUpHandler,
                        IDragHandler
{
    public enum HandleType {
        DRAG,
        RESIZE
    }

    public enum HandleLoc {
        IRRELEVANT,
        TOP_LEFT,
        TOP_RIGHT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT,
        TOP,
        RIGHT,
        BOTTOM,
        LEFT
    }

    public HandleType handleType;

    private UIContainer m_Target;
    private HandleLoc m_Loc;
    private Vector2 m_Offset;
    private CursorController m_Cursor;
    private bool m_PointerDown;
    private bool m_Hovering;

    private CursorController cursor {
        get {
            if (!m_Cursor) {
                m_Cursor = CursorController.instance;
            }
            if (!m_Cursor) {
                LogError("Trying to use cursor, but no instance was found.");
            }
            return m_Cursor;
        }
    }

#region Unity Functions
#endregion

#region Public Functions
    public void Configure(UIContainer _target, HandleLoc _loc) {
        m_Target = _target;
        m_Loc = _loc;
    }
#endregion

#region Interface Functions
    public void OnPointerEnter(PointerEventData _ped) {
        m_Hovering = true;

        if (!cursor) return;
        
        switch (m_Loc) {
            case HandleLoc.IRRELEVANT: cursor.LoadDrag(); break;
            case HandleLoc.BOTTOM_RIGHT:
            case HandleLoc.TOP_LEFT: cursor.LoadScaleDiagRight(); break;
            case HandleLoc.BOTTOM_LEFT:
            case HandleLoc.TOP_RIGHT: cursor.LoadScaleDiagLeft(); break;
            case HandleLoc.BOTTOM:
            case HandleLoc.TOP: cursor.LoadScaleVertical(); break;
            case HandleLoc.RIGHT:
            case HandleLoc.LEFT: cursor.LoadScaleHorizontal(); break;
        }
    }

    public void OnPointerExit(PointerEventData _ped) {
        m_Hovering = false;

        if (!cursor) return;
        if (m_PointerDown) return;
        cursor.LoadMain();
    }

    public void OnPointerUp(PointerEventData _ped) {
        m_PointerDown = true;

        if (!cursor) return;
        if (m_Hovering) return;
        cursor.LoadMain();
    }

    public void OnPointerDown(PointerEventData _ped) {
        m_PointerDown = false;

        if (!m_Target) return;
        m_Offset = new Vector2(m_Target.rect.transform.position.x, m_Target.rect.transform.position.y) - _ped.position;
    }

    public void OnDrag(PointerEventData _ped) {
        if (!m_Target) return;
        switch (handleType) {
            case HandleType.DRAG:
                m_Target.Drag(_ped.position + m_Offset);
                break;
            case HandleType.RESIZE:
                m_Target.Resize(_ped.position, m_Loc);
                break;
        }
    }
#endregion
}
