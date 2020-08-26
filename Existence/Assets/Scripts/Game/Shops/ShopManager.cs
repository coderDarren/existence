using System.Collections.Generic;
using UnityEngine;
using UnityCore.Menu;

/*
 * For now this just needs to respond to shop terminal events..
 * ..more to come surely..
 */
public class ShopManager : GameSystem
{
    public static ShopManager instance;

    public PageController pageController;

    private Session m_Session;

    // Use this to prevent opening multiple shop terminals
    private bool m_UsingShopTerminal;
    private NetworkShopTerminalData m_ShopTerminalData;
    private List<IItem> m_ShopItems;
    private List<IItem> m_SellItems;
    private List<IItem> m_TradeItems;

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

    public List<IItem> shopItems {
        get {
            return m_ShopItems;
        }
    }

    public List<IItem> sellItems {
        get {
            return m_SellItems;
        }
    }

    public List<IItem> tradeItems {
        get {
            return m_TradeItems;
        }
    }

#region Unity Functions
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Start() {
        if (instance != this) return;

        if (!session) return;
        if (!session.network) return;
        session.network.OnShopTerminalInteracted += OnShopTerminalInteracted;
    }

    private void OnDisable() {
        if (instance != this) return;
        instance = null;

        if (!session) return;
        if (!session.network) return;
        session.network.OnShopTerminalInteracted -= OnShopTerminalInteracted;
    }
#endregion

#region Public Functions
    public void OpenShop(int _id) {
        if (!session) return;
        if (!session.network) return;
        if (m_UsingShopTerminal) return;
        session.network.InteractShop(new NetworkShopTerminalInteractData(_id));
    }

    public void CloseShop() {
        m_UsingShopTerminal = false;
        m_ShopTerminalData = null;
        pageController.TurnPageOff(PageType.ShopTerminal);
        m_ShopItems = new List<IItem>();
        m_TradeItems = new List<IItem>();
        m_SellItems = new List<IItem>();
    }

    public void PrepareBuyItem(IItem _item) {
        if (!ShopTerminalPage.instance) return;
        m_TradeItems.Add(_item);
        // add to price
        ShopTerminalPage.instance.Redraw();
    }

    public void PrepareSellItem(IItem _item) {
        if (!ShopTerminalPage.instance) return;
        // check for item in inventory
        m_SellItems.Add(_item);
        ShopTerminalPage.instance.Redraw();
    }

    public void CancelBuyItem(IItem _item) {
        if (!ShopTerminalPage.instance) return;
        m_TradeItems.Remove(_item);
        ShopTerminalPage.instance.Redraw();
    }

    public void CancelSellItem(IItem _item) {
        if (!ShopTerminalPage.instance) return;
        m_SellItems.Remove(_item);
        ShopTerminalPage.instance.Redraw();
    }

    public void CompleteTrade() {
        if (!ShopTerminalPage.instance) return;
        if (!session) return;
        if (!session.network) return;
        List<NetworkShopTerminalSellInfo> _sell = new List<NetworkShopTerminalSellInfo>();
        List<NetworkShopTerminalBuyInfo> _buy = new List<NetworkShopTerminalBuyInfo>();
        foreach(IItem _item in m_TradeItems) {
            _buy.Add(new NetworkShopTerminalBuyInfo(_item.def.id, _item.def.level, 0));
        }
        session.network.TradeShop(new NetworkShopTerminalTradeData(_sell, _buy));
    }
#endregion

#region Private Functions
    private void OnShopTerminalInteracted(NetworkShopTerminalData _data) {
        m_UsingShopTerminal = true;
        m_ShopTerminalData = _data;
        m_ShopItems = ParseItems(m_ShopTerminalData.itemData);
        m_TradeItems = new List<IItem>();
        m_SellItems = new List<IItem>();
        pageController.TurnPageOn(PageType.ShopTerminal);
    }
    
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
}
