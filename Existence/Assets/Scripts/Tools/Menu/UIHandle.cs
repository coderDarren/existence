
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// UIHandle is responsible for two major window operations
///     1. Resizing
///     2. Dragging
/// </summary>
public class UIHandle : MonoBehaviour, 
                        IPointerDownHandler, 
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

#region Unity Functions
#endregion

#region Public Functions
    public void Configure(UIContainer _target, HandleLoc _loc) {
        m_Target = _target;
        m_Loc = _loc;
    }
#endregion

#region Interface Functions
    public void OnPointerDown(PointerEventData _ped) {
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
