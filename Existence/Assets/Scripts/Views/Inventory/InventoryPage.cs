
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

        // !! TODO 
        // Looping through children to assign index id.. init ids a better way..
        int _index = 0;
        foreach (Transform _t in slotParent) {
            if (_t == slotParent) continue;
            _t.GetComponent<InventorySlot>().Init(_index);
            _index++;
        }

        foreach (ItemData _item in m_PlayerData.inventory) {
            if (_item.slotLoc == -1) continue;
            InventorySlot _slot = slotParent.transform.GetChild(_item.slotLoc).GetComponent<InventorySlot>();
            _slot.AssignIcon(_item);
        }
    }
#endregion

#region Public Functions
    public void SaveInventory(ItemData _item) {
        if (!session) return;
        if (!session.network) return;
        NetworkInventoryUpdate _data = new NetworkInventoryUpdate(_item.slotID, _item.slotLoc);
        session.network.SaveInventory(_data);
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
