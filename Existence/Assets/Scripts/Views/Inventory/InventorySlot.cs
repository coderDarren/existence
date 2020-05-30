
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;

#region Public Functions
    public void EraseIcon() {
        UpdateAlpha(0);
        icon.sprite = null;
    }

    public void AssignIcon(Sprite _sprite) {
        if (_sprite == null) return;
        UpdateAlpha(1);
        icon.sprite = _sprite;
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
