
using UnityEngine;
using UnityEngine.UI;

public class P2PTradeItemSlot : InspectableItem
{
    public Image icon;

#region Unity Functions
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Interact();
        }
    }
#endregion

#region Public Functions
    public void AssignItem(IItem _item) {
        m_Item = _item;

        Sprite _s = Utilities.LoadStreamingAssetsSprite(m_Item.def.icon);
        if (_s == null) {
            UpdateAlpha(0);
            return;
        }

        icon.sprite = _s;
        UpdateAlpha(1);
    }

    public void ClearItem() {
        m_Item = null;
        icon.sprite = null;
        UpdateAlpha(0);
    }
#endregion

#region Private Functions
    public void Interact() {
        if (!m_Hovering) return;
        if (!P2PTradePage.instance) return;
        if (!CursorController.instance) return;
        if (CursorController.instance.selectedItem != null) {
            P2PTradePage.instance.SendAddOutgoingItem(CursorController.instance.selectedItem);
        } else if (m_Item != null) {
            P2PTradePage.instance.SendRemoveOutgoingItem(m_Item);
        }
        OnPointerExit(null);
    }

    private void UpdateAlpha(float _alpha) {
        Color _c = icon.color;
        _c.a = _alpha;
        icon.color = _c;
    }
#endregion
}
