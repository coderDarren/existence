using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityCore.Menu;

public class P2PTradePage : Page
{
    public static P2PTradePage instance;

    public Transform incomingItemsContainer;
    public Transform outgoingItemsContainer;

    private P2PTradeManager m_TradeManager;
    private List<ItemData> incomingItems;
    private List<ItemData> outgoingItems;

    // get P2PTradeManager with integrity
    private P2PTradeManager tradeManager {
        get {
            if (!m_TradeManager) {
                m_TradeManager = P2PTradeManager.instance;
            }
            if (!m_TradeManager) {
                LogError("Trying to use P2PTradeManager, but no instance could be found.");
            }
            return m_TradeManager;
        }
    }

#region Public Functions
    public void AddIncomingItem(ItemData _item) {
        incomingItems.Add(_item);
        Draw();
    }

    public void RemoveIncomingItem(ItemData _item) {
        incomingItems.Remove(_item);
        Draw();
    }

    public void AddOutgoingItem(ItemData _item) {
        outgoingItems.Add(_item);
        Draw();
    }

    public void RemoveOutgoingItem(ItemData _item) {
        outgoingItems.Remove(_item);
        Draw();
    }
#endregion

#region Private Functions
    private void Draw() {
        Clear();
        DrawItems(incomingItemsContainer, incomingItems);
        DrawItems(outgoingItemsContainer, outgoingItems);
    }

    private void DrawItems(Transform _slotContainer, List<ItemData> _items) {
        int _index = 0;
        if (_items == null) return;
        foreach (ItemData _item in _items) {
            P2PTradeItemSlot _slot = _slotContainer.transform.GetChild(_index).GetComponent<P2PTradeItemSlot>();
            _slot.AssignItem(_item);
            _index++;
        }
    }

    private void Clear() {
        ClearChildren(incomingItemsContainer);
        ClearChildren(outgoingItemsContainer);
    }

    private void ClearChildren(Transform _parent) {
        foreach (Transform _t in _parent) {
            if (_t == _parent) continue;
            _t.GetComponent<ShopTerminalSlot>().ClearItem();
        }
    }
#endregion

#region Override Functions
    protected override void OnPageEnabled() {
        base.OnPageEnabled();
        if (!tradeManager) return;
        if (instance != null) return;
        if (!instance) {
            instance = this;
        }
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        instance = null;
    }
#endregion
}
