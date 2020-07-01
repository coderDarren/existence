
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : InspectableItem
{
    public GearType type;
    public Image icon;

#region Unity Functions
    private void Update() {
        if (!m_Item) return;
        
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
#endregion

#region Private Functions
    private void UpdateAlpha(float _alpha) {
        Color _c = icon.color;
        _c.a = _alpha;
        icon.color = _c;
    }
#endregion
}
