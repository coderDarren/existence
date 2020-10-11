
using UnityEngine;

/*
 * Runs on player
 * Manage trade related windows.
 * Handle trade related events
 */
public class P2PTradeManager : GameSystem
{
    public static P2PTradeManager instance;

    private Session m_Session;

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

#region Unity Functions
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void OnEnable() {
        if (!session.network) return;
        session.network.p2pTradeRequestEvt.OnMsg += OnTradeRequest;
        session.network.p2pTradeRequestRejectEvt.OnMsg += OnTradeReject;
        session.network.p2pTradeStartEvt.OnMsg += OnTradeStart;
        session.network.p2pTradeAcceptEvt.OnMsg += OnTradeAccept;
        session.network.p2pTradeCancelEvt.OnMsg += OnTradeCancel;
        session.network.p2pTradeAddItemEvt.OnEvt += OnTradeItemAdd;
        session.network.p2pTradeRemoveItemEvt.OnEvt += OnTradeItemRemove;
    }

    private void OnDisable() {
        if (!session.network) return;
        session.network.p2pTradeRequestEvt.OnMsg -= OnTradeRequest;
        session.network.p2pTradeRequestRejectEvt.OnMsg -= OnTradeReject;
        session.network.p2pTradeStartEvt.OnMsg -= OnTradeStart;
        session.network.p2pTradeAcceptEvt.OnMsg -= OnTradeAccept;
        session.network.p2pTradeCancelEvt.OnMsg -= OnTradeCancel;
        session.network.p2pTradeAddItemEvt.OnEvt -= OnTradeItemAdd;
        session.network.p2pTradeRemoveItemEvt.OnEvt -= OnTradeItemRemove;
    }
#endregion

#region Public Functions
#region PLAYER EVENTS
    public void RequestTrade(string _playerName) {
        if (!session.network) return;
        session.network.RequestP2PTrade(_playerName);
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

    public void AddTradeItem(ItemData _item) {
        if (!session.network) return;
        session.network.AddP2PTradeItem(_item);
    }
    
    public void RemoveTradeItem(ItemData _item) {
        if (!session.network) return;
        session.network.RemoveP2PTradeItem(_item);
    }
#endregion
#endregion

#region Private Functions
#region OTHER PLAYER EVENTS
    private void OnTradeRequest(string _player) {
        
    }

    private void OnTradeReject(string _data) {

    }

    private void OnTradeStart(string _data) {

    }

    private void OnTradeAccept(string _data) {

    }

    private void OnTradeCancel(string _data) {

    }

    private void OnTradeItemAdd(ItemData _item) {

    }

    private void OnTradeItemRemove(ItemData _item) {

    }
#endregion
#endregion
}
