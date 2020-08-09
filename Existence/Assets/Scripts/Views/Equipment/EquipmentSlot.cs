
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : InspectableItem
{
    public GearType type;
    public Image icon;

#region Unity Functions
    private void Update() {
        // check input for equip attempts
        if (Input.GetMouseButtonUp(1)) {
            UnequipItem();
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
    private void UpdateAlpha(float _alpha) {
        Color _c = icon.color;
        _c.a = _alpha;
        icon.color = _c;
    }

    private void UnequipItem() {
        if (!m_Hovering) return;
        if (m_Item == null) return;
        if (!EquipmentPage.instance) return;
        EquipmentPage.instance.UnequipItem(m_Item);
        OnPointerExit(null);
    }
#endregion
}
