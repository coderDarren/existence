
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

    /*
     * We set up a local event system to prevent..
     * ..events from multiple nearby handles fighting..
     * ..for control on the same container.
     * _groupId is assigned by this handle's container.
     */
    public delegate void UIHandleEvent(int _groupId);
    public static event UIHandleEvent StartedUsing;
    public static event UIHandleEvent StoppedUsing; 

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
    private bool m_Busy;
    private bool m_DidExit;
    private int m_HandleGroupId;

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

/*
 * Deal with the local event subscription
 */
#region Unity Functions
    private void OnEnable() {
        UIHandle.StartedUsing += OnStartedUsing;
        UIHandle.StoppedUsing += OnStoppedUsing;
    }

    private void OnDisable() {
        UIHandle.StartedUsing -= OnStartedUsing;
        UIHandle.StoppedUsing -= OnStoppedUsing;
    }
#endregion

#region Private Functions
    private void OnStartedUsing(int _groupId) {
        if (_groupId != m_HandleGroupId) return;
        m_Busy = true;
    }

    private void OnStoppedUsing(int _groupId) {
        if (_groupId != m_HandleGroupId) return;
        m_Busy = false;
    }
#endregion

#region Public Functions
    public void Configure(UIContainer _target, HandleLoc _loc, int _groupId) {
        m_Target = _target;
        m_Loc = _loc;
        m_HandleGroupId = _groupId;
    }
#endregion

#region Interface Functions
    public void OnPointerEnter(PointerEventData _ped) {
        if (!cursor) return;
        if (m_Busy) return;
        
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
        m_DidExit = true;
        if (!cursor) return;
        if (m_Busy) return;
        cursor.LoadMain();
    }

    public void OnPointerUp(PointerEventData _ped) {
        if (!cursor) return;
        StoppedUsing(m_HandleGroupId);
        if (m_DidExit) {
            m_DidExit = false;
            cursor.LoadMain();
        }
    }

    public void OnPointerDown(PointerEventData _ped) {
        if (!m_Target) return;
        if (m_Busy) return;
        StartedUsing(m_HandleGroupId);
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
