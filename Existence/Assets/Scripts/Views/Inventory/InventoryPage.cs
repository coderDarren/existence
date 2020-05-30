
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class InventoryPage : Page
{
    public Transform slotParent;
    private Session m_Session;
    private PlayerData m_PlayerData;

    // get Session with integrity
    private Session session {
        get {
            if (!m_Session) {
                m_Session = Session.instance;
            }
            if (!m_Session) {
                LogError("Trying to use Session, but no instance could be found.");
            }
            return m_Session;
        }
    }

#region Private Functions
    private void EraseInventory() {
        foreach (Transform _t in slotParent) {
            if (_t == slotParent) continue;
            _t.GetComponent<InventorySlot>().EraseIcon();
        }
    }

    private void DrawInventory() {
        EraseInventory();
        if (m_PlayerData.inventory == null || m_PlayerData.inventory.Length == 0) 
            return;

        int _index = 0;
        ItemData _item = m_PlayerData.inventory[_index];
        foreach (Transform _t in slotParent) {
            if (_t == slotParent) continue;
            if (m_PlayerData.inventory.Length < _index + 1) 
                return;
            
            _index++;
            // apply item to slot
            Sprite _sprite = Utilities.LoadStreamingAssetsSprite(_item.icon, 2, 2);
            _t.GetComponent<InventorySlot>().AssignIcon(_sprite);
        }
    }
#endregion

#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        if (!session) return;
        if (session.playerData == null) return;

        m_PlayerData = session.playerData;
        DrawInventory();
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
    }
#endregion
}
