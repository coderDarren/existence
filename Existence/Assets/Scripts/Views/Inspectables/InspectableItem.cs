
using UnityEngine;

public class InspectableItem : MonoBehaviour
{
    protected ItemData m_Item;
    protected CursorController m_Cursor;

    protected CursorController cursor {
        get {
            if (!m_Cursor) {
                m_Cursor = CursorController.instance;
            }
            if (!m_Cursor) {
                Debug.LogWarning("Inventory Slot is trying to access cursor, but no instance of CursorController was found.");
            }
            return m_Cursor;
        }
    }

    public ItemData item {
        get {
            return m_Item;
        }
    }
}
