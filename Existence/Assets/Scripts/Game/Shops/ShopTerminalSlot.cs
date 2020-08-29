
using UnityEngine;
using UnityEngine.UI;

public class ShopTerminalSlot : InspectableItem
{
    public enum SlotType {
        SHOP,
        SELL,
        TRADE
    }

    public Image icon;
    public SlotType slotType;

    private ShopManager m_ShopManager;
    private CursorController m_Cursor;

    private CursorController cursor {
        get {
            if (!m_Cursor) {
                m_Cursor = CursorController.instance;
            }
            if (!m_Cursor) {
                LogWarning("Trying to use cursor, but no instance of CursorController was found.");
            }
            return m_Cursor;
        }
    }

    private ShopManager shopManager {
        get {
            if (!m_ShopManager) {
                m_ShopManager = ShopManager.instance;
            }
            if (!m_ShopManager) {
                LogWarning("Trying to access shop manager, but no instance of ShopManager was found.");
            }
            return m_ShopManager;
        }
    }

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

    public void HandleAction() {
        switch (slotType) {
            case SlotType.SHOP: BuyItem(); break;
            case SlotType.SELL: 
                if (m_Item == null) {
                    SellItem();
                } else {
                    CancelSellItem();
                }
                break;
            case SlotType.TRADE: CancelBuyItem(); break;
        }
    }
#endregion

#region Private Functions
    private void UpdateAlpha(float _alpha) {
        Color _c = icon.color;
        _c.a = _alpha;
        icon.color = _c;
    }

    /*
     * Called when player clicks an item in the SHOP column
     * Item is transferred to the TRADE column, in preparation for purchase
     */
    private void BuyItem() {
        if (!shopManager) return;
        if (!cursor) return;
        if (cursor.selectedItem != null) return;
        if (m_Item == null) return;
        shopManager.PrepareBuyItem(m_Item);
    }

    /*
     * Called when player clicks an item in the TRADE column
     * Item is removed from the TRADE column
     */
    private void CancelBuyItem() {
        if (!shopManager) return;
        if (m_Item == null) return;
        OnPointerExit(null);
        shopManager.CancelBuyItem(m_Item);
    }

    /*
     * Called when player clicks a slot in the SELL column, which has no item in it
     * IF item is selected with cursor, it will be dropped here
     */
    private void SellItem() {
        if (!shopManager) return;
        if (!cursor) return;
        if (cursor.selectedItem == null) return;
        shopManager.PrepareSellItem(cursor.selectedItem);
    }

    /*
     * Called when player clicks an item in the SELL column
     * Item is transferred back to the player's inventory
     */
    private void CancelSellItem() {
        if (!shopManager) return;
        if (m_Item == null) return;
        if (cursor.selectedItem != null) return;
        OnPointerExit(null);
        shopManager.CancelSellItem(m_Item);
    }
#endregion
}
