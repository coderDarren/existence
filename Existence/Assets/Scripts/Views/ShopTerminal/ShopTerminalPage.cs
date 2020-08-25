using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Menu;

public class ShopTerminalPage : Page
{
    public static ShopTerminalPage instance;

    public Transform slotParent;

    private Session m_Session;
    private ShopManager m_ShopManager;
    private List<IItem> m_ShopItems;
    private List<IItem> m_SellItems;
    private List<IItem> m_BuyItems;

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
    private void Clear() {
        int _index = 0;
        foreach (Transform _t in slotParent) {
            if (_t == slotParent) continue;
            _t.GetComponent<ShopTerminalSlot>().ClearItem();
            _index++;
        }
    }

    private void Draw() {
        // // !! TODO 
        // // Looping through children to assign index id.. init ids a better way..
        // int _index = 0;
        // foreach (Transform _t in slotParent) {
        //     if (_t == slotParent) continue;
        //     _t.GetComponent<InventorySlot>().Init(_index);
        //     _index++;
        // }

        int _index = 0;
        foreach (IItem _item in m_ShopItems) {
            ShopTerminalSlot _slot = slotParent.transform.GetChild(_index).GetComponent<ShopTerminalSlot>();
            _slot.AssignItem(_item);
            _index++;
        }
    }
#endregion

#region Public Functions
    public void Redraw() {
        Clear();
        Draw();
    }
#endregion

#region Private Functions
    private List<IItem> ParseItems(string[] _itemData) {
        List<IItem> _ret = new List<IItem>();
        if (_itemData == null) return _ret;
        foreach(string _itemJson in _itemData) {
            IItem _item = ItemData.CreateItem(_itemJson);
            _ret.Add(_item);
        }
        return _ret;
    }
#endregion

#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        if (!session) return;
        if (!shopManager) return;
        if (session.playerData == null) return;
        if (!instance) {
            instance = this;
        }
        
        m_ShopItems = ParseItems(shopManager.shopTerminalData.itemData);
        m_BuyItems = new List<IItem>();
        m_SellItems = new List<IItem>();

        Redraw();
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        instance = null;
    }
#endregion
}
