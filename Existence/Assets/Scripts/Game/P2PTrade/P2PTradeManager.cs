
using UnityEngine;
using UnityCore.Menu;

/*
 * Runs on player
 * Manage trade related windows.
 * Handle trade related events
 */
public class P2PTradeManager : GameSystem
{
    public static P2PTradeManager instance;

    public PageController pageController;

    private Session m_Session;
    private TargetController m_TargetController;
    private string m_TradePlayer;

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

    private TargetController targetController {
        get {
            if (!m_TargetController) {
                m_TargetController = TargetController.instance;
            }
            if (!m_TargetController) {
                LogError("Trying to use TargetController, but no instance could be found.");
            }
            return m_TargetController;
        }
    }

#region Unity Functions
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Start() {
        if (!targetController) return;
        targetController.OnTargetInteracted += OnTargetInteracted;
    }

    private void OnEnable() {
        if (!session.network) return;
        session.network.p2pTradeRequestEvt.OnMsg += OnTradeRequest;
        session.network.p2pTradeRequestRejectEvt.OnMsg += OnTradeReject;
        session.network.p2pTradeStartEvt.OnMsg += OnTradeStart;
        session.network.p2pTradeAcceptEvt.OnEvt += OnTradeAccept;
        session.network.p2pTradeCancelEvt.OnMsg += OnTradeCancel;
        session.network.p2pTradeAddItemEvt.OnEvt += OnTradeItemAdd;
        session.network.p2pTradeRemoveItemEvt.OnEvt += OnTradeItemRemove;
    }

    private void OnDisable() {
        if (!session.network) return;
        if (!targetController) return;
        session.network.p2pTradeRequestEvt.OnMsg -= OnTradeRequest;
        session.network.p2pTradeRequestRejectEvt.OnMsg -= OnTradeReject;
        session.network.p2pTradeStartEvt.OnMsg -= OnTradeStart;
        session.network.p2pTradeAcceptEvt.OnEvt -= OnTradeAccept;
        session.network.p2pTradeCancelEvt.OnMsg -= OnTradeCancel;
        session.network.p2pTradeAddItemEvt.OnEvt -= OnTradeItemAdd;
        session.network.p2pTradeRemoveItemEvt.OnEvt -= OnTradeItemRemove;
        targetController.OnTargetInteracted -= OnTargetInteracted;
    }
#endregion

#region Public Functions
#region PLAYER EVENTS
    public void RequestTrade(string _playerName) {
        if (!session.network) return;
        m_TradePlayer = _playerName;
        session.network.RequestP2PTrade(_playerName);
        pageController.TurnPageOn(PageType.SingleOptionMessage);
        SingleOptionMessagePage.instance.Redraw("Trade Transmission Request", "Sending trade transmission to "+_playerName+".", "Cancel");
        SingleOptionMessagePage.instance.OnOption(() => {CancelTrade(); pageController.TurnPageOff(PageType.SingleOptionMessage);});
    }

    public void RejectTrade() {
        if (!session.network) return;
        session.network.RejectP2PTradeRequest();
    }

    public void StartTrade() {
        if (!session.network) return;
        session.network.AcceptP2PTradeRequest();
    }

    public void AcceptTrade() {
        if (!session.network) return;
        session.network.AcceptP2PTrade();
    }

    public void CancelTrade() {
        if (!session.network) return;
        session.network.CancelP2PTrade();
    }

    public void AddTradeItem(IItem _item) {
        if (!session.network) return;
        session.network.AddP2PTradeItem(new NetworkP2PTradeItemData(session.player.data.player.name, _item.ToJsonString()));
    }
    
    public void RemoveTradeItem(IItem _item) {
        if (!session.network) return;
        session.network.RemoveP2PTradeItem(new NetworkP2PTradeItemData(session.player.data.player.name, _item.ToJsonString()));
    }
#endregion
#endregion

#region Private Functions
#region OTHER PLAYER EVENTS
    private void OnTradeRequest(string _player) {
        m_TradePlayer = _player;
        pageController.TurnPageOn(PageType.DualOptionMessage);
        DualOptionMessagePage.instance.Redraw("Incoming Trade Transmission.", _player+" wants to trade with you.", "Reject", "Accept");
        DualOptionMessagePage.instance.OnOption1(() => {RejectTrade(); pageController.TurnPageOff(PageType.DualOptionMessage);});
        DualOptionMessagePage.instance.OnOption2(() => {StartTrade(); pageController.TurnPageOff(PageType.DualOptionMessage);});
        Chatbox.instance.EmitMessageLocal("Incoming trade transmission from "+_player+".");
    }

    private void OnTradeReject(string _data) {
        pageController.TurnPageOff(PageType.SingleOptionMessage);
        Chatbox.instance.EmitMessageLocal("Your trade transmission was rejected.");
    }

    private void OnTradeStart(string _data) {
        pageController.TurnPageOff(PageType.SingleOptionMessage);
        pageController.TurnPageOn(PageType.P2PTrade);
        Chatbox.instance.EmitMessageLocal("Your trade transmission was accepted.");
    }

    private void OnTradeAccept(NetworkP2PTradeData _data) {
        if (_data.accepted) {
            pageController.TurnPageOff(PageType.P2PTrade);
            Chatbox.instance.EmitMessageLocal("Trade transmission complete.");
        } else {
            Chatbox.instance.EmitMessageLocal("Trade transmission accepted by "+m_TradePlayer);
        }
    }

    private void OnTradeCancel(string _data) {
        pageController.TurnPageOff(PageType.P2PTrade);
        pageController.TurnPageOff(PageType.SingleOptionMessage);
        pageController.TurnPageOff(PageType.DualOptionMessage);
        Chatbox.instance.EmitMessageLocal("Trade transmission canceled.");
    }

    private void OnTradeItemAdd(NetworkP2PTradeItemData _data) {
        if (!P2PTradePage.instance) return;
        IItem _item = ItemData.CreateItem(_data.itemJson);
        if (_data.playerName == session.player.data.player.name) {
            P2PTradePage.instance.AddOutgoingItem(_item);
            CursorController.instance.DropItem();
        } else {
            P2PTradePage.instance.AddIncomingItem(_item);
        }
    }

    private void OnTradeItemRemove(NetworkP2PTradeItemData _data) {
        if (!P2PTradePage.instance) return;
        IItem _item = ItemData.CreateItem(_data.itemJson);
        if (_data.playerName == session.player.data.player.name) {
            P2PTradePage.instance.RemoveOutgoingItem(_item);
            CursorController.instance.DropItem();
        } else {
            P2PTradePage.instance.RemoveIncomingItem(_item);
        }
    }
#endregion

    /*
     * Invoked by the TargetController's OnTargetInteracted event
     * Selectable _s will be passed, and is assumed to be a selectable of type 'NetworkPlayer'..
     * ..so we should be able to extract the NetworkPlayer object from '_s'.
     */
    private void OnTargetInteracted(Selectable _s, bool _primary) {
        NetworkPlayer _p = _s.GetComponent<NetworkPlayer>();
        if (!_p) {
            Log("This is not a player.");
            return;
        }
        RequestTrade(_p.networkData.name);
    }
#endregion
}
