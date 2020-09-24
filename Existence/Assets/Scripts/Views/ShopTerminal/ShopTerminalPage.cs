using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class ShopTerminalPage : Page
{
    public static ShopTerminalPage instance;

    public Transform shopSlotsParent;
    public Transform sellSlotsParent;
    public Transform tradeSlotsParent;
    public Text netTransferLabel;

    private Session m_Session;
    private ShopManager m_ShopManager;
    private int m_NetTransfer;

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

    // get ShopManager with integrity
    private ShopManager shopManager {
        get {
            if (!m_ShopManager) {
                m_ShopManager = ShopManager.instance;
            }
            if (!m_ShopManager) {
                LogError("Trying to use ShopManager, but no instance could be found.");
            }
            return m_ShopManager;
        }
    }

#region Private Functions
    private void Draw() {
        DrawItems(shopSlotsParent, shopManager.shopItems);
        DrawItems(sellSlotsParent, shopManager.sellItems, false);
        DrawItems(tradeSlotsParent, shopManager.tradeItems);
    }

    private void DrawItems(Transform _slotContainer, List<IItem> _items, bool _buy=true) {
        int _index = 0;
        if (_items == null) return;
        foreach (IItem _item in _items) {
            ShopTerminalSlot _slot = _slotContainer.transform.GetChild(_index).GetComponent<ShopTerminalSlot>();
            _slot.AssignItem(_item, _buy);
            _index++;
        }
    }

    private void Clear() {
        ClearChildren(shopSlotsParent);
        ClearChildren(sellSlotsParent);
        ClearChildren(tradeSlotsParent);
    }

    private void ClearChildren(Transform _parent) {
        foreach (Transform _t in _parent) {
            if (_t == _parent) continue;
            _t.GetComponent<ShopTerminalSlot>().ClearItem();
        }
    }
#endregion

#region Public Functions
    public void AddPrice(int _added) {
        m_NetTransfer += _added;

        if (m_NetTransfer < 0) {
            netTransferLabel.text = "<color=#f00>"+m_NetTransfer.ToString()+"</color>";
        } else if (m_NetTransfer > 0) {
            netTransferLabel.text = "<color=#0f0>"+m_NetTransfer.ToString()+"</color>";
        } else {
            netTransferLabel.text = m_NetTransfer.ToString();
        }
    }

    public void Redraw() {
        if (instance != this) return;
        Clear();
        Draw();
    }
#endregion

#region Private Functions
    
#endregion

#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        if (!session) return;
        if (!shopManager) return;
        if (instance != null) return;
        if (!instance) {
            instance = this;
        }

        m_NetTransfer = 0;
        netTransferLabel.text = m_NetTransfer.ToString();

        Redraw();
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        instance = null;
    }
#endregion
}
