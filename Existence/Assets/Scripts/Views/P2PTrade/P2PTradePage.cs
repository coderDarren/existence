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
    public InputField tixField;
    public Text incomingTix;

    private P2PTradeManager m_TradeManager;
    private List<IItem> incomingItems;
    private List<IItem> outgoingItems;

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
    public void AddIncomingItem(IItem _item) {
        incomingItems.Add(_item);
        Draw();
    }

    public void RemoveIncomingItem(IItem _item) {
        for (int i = incomingItems.Count - 1; i >= 0; i--) {
            IItem _i = incomingItems[i];
            if (_i.def.id == _item.def.id && _i.def.level == _item.def.level) {
                incomingItems.RemoveAt(i);
                break;
            }
        }
        Draw();
    }

    public void AddOutgoingItem(IItem _item) {
        outgoingItems.Add(_item);
        Draw();
    }

    public void RemoveOutgoingItem(IItem _item) {
        for (int i = outgoingItems.Count - 1; i >= 0; i--) {
            IItem _i = outgoingItems[i];
            if (_i.def.id == _item.def.id && _i.def.level == _item.def.level) {
                outgoingItems.RemoveAt(i);
                break;
            }
        }
        Draw();
    }

    public void SendAddOutgoingItem(IItem _item) {
        if (!tradeManager) return;
        tradeManager.AddTradeItem(_item);
    }

    public void SendRemoveOutgoingItem(IItem _item) {
        if (!tradeManager) return;
        tradeManager.RemoveTradeItem(_item);
    }

    public void ChangeTix() {
        if (!tradeManager) return;
        tradeManager.ChangeTix(int.Parse(tixField.text));
    }

    public void ChangeIncomingTix(int _tix) {
        incomingTix.text = _tix.ToString();
    }

    public void ChangeOutgoingTix(int _tix) {
        tixField.text = _tix.ToString();
    }

    public void AcceptTrade() {
        if (!tradeManager) return;
        tradeManager.AcceptTrade();
    }

    public void CancelTrade() {
        if (!tradeManager) return;
        tradeManager.CancelTrade();
    }
#endregion

#region Private Functions
    private void Draw() {
        Clear();
        DrawItems(incomingItemsContainer, incomingItems);
        DrawItems(outgoingItemsContainer, outgoingItems);
    }

    private void DrawItems(Transform _slotContainer, List<IItem> _items) {
        int _index = 0;
        if (_items == null) return;
        foreach (IItem _item in _items) {
            P2PTradeItemSlot _slot = _slotContainer.transform.GetChild(_index).GetComponent<P2PTradeItemSlot>();
            _slot.AssignItem(_item);
            _index++;
        }
    }

    private void Clear() {
        tixField.text = "0";
        incomingTix.text = "0";
        ClearChildren(incomingItemsContainer);
        ClearChildren(outgoingItemsContainer);
    }

    private void ClearChildren(Transform _parent) {
        foreach (Transform _t in _parent) {
            if (_t == _parent) continue;
            _t.GetComponent<P2PTradeItemSlot>().ClearItem();
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
            incomingItems = new List<IItem>();
            outgoingItems = new List<IItem>();
        }
    }

    protected override void OnPageDisabled() {
        base.OnPageDisabled();
        Clear();
        instance = null;
    }
#endregion
}
