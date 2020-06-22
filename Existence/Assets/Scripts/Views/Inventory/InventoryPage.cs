
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class InventoryPage : Page
{
    public static InventoryPage instance;

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
        if (m_PlayerData.inventory == null || m_PlayerData.inventory.Count == 0) 
            return;

        // !! TODO 
        // Looping through children to assign index id.. init ids a better way..
        int _index = 0;
        foreach (Transform _t in slotParent) {
            if (_t == slotParent) continue;
            _t.GetComponent<InventorySlot>().Init(_index);
            _index++;
        }

        foreach (IItem _item in m_PlayerData.inventory) {
            if (_item.def.slotLoc == -1) {
                for (int i = 0; i < slotParent.transform.childCount; i++) {
                    InventorySlot _check = slotParent.transform.GetChild(i).GetComponent<InventorySlot>();
                    if (_check.item == null) {
                        // this is an available slot
                        _item.def.slotLoc = i;
                        SaveInventory(_item);
                        _check.AssignIcon(_item);
                        break;
                    }
                }
                continue;
            }
            InventorySlot _slot = slotParent.transform.GetChild(_item.def.slotLoc).GetComponent<InventorySlot>();
            _slot.AssignIcon(_item);
        }
    }
#endregion

#region Public Functions
    public void SaveInventory(IItem _item) {
        if (!session) return;
        if (!session.network) return;
        NetworkInventoryUpdate _data = new NetworkInventoryUpdate(_item.def.slotID, _item.def.slotLoc);
        session.network.SaveInventory(_data);
    }

    public void Redraw() {
        EraseInventory();
        DrawInventory();
    }
#endregion

#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        if (!session) return;
        if (session.playerData == null) return;
        if (!instance) {
            instance = this;
        }

        m_PlayerData = session.playerData;
        Redraw();
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        instance = null;
    }
#endregion
}
