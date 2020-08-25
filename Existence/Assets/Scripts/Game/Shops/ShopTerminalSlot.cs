
using UnityEngine;
using UnityEngine.UI;

public class ShopTerminalSlot : InspectableItem
{
    public Image icon;

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

    public void HandleAction() {
        
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
