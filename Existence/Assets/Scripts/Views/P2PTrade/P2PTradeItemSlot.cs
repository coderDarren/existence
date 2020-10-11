
using UnityEngine;
using UnityEngine.UI;

public class P2PTradeItemSlot : InspectableItem
{
    public Image icon;

#region Public Functions
    public void AssignItem(ItemData _item) {
        m_Item.def = _item;

        Sprite _s = Utilities.LoadStreamingAssetsSprite(m_Item.def.icon);1
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

    public void Interact() {
        if (!m_Hovering) return;
        if (m_Item == null) return;
        
        OnPointerExit(null);
    }
#endregion

#region Private Functions
    private void UpdateAlpha(float _alpha) {
        Color _c = icon.color;
        _c.a = _alpha;
        icon.color = _c;
    }
#endregion
}
