using System.Text.RegularExpressions;
using UnityEngine;
using SocketIO;

/// <summary>
/// This controller is responsible for managing all network messaging..
/// ..and transmitting events to anyone interested in subscribing
///
/// Aside from event transmission on the client, the controller is also..
/// ..responsible helping send data up to the network
///
/// NetworkController will operate directly with the core socket engine via SocketIOComponent
/// <summary>
[RequireComponent(typeof(SocketIOComponent))]
public class NetworkController : GameSystem
{
    public static NetworkController instance;

    /*
     * Some helpful events to subscribe to..
     * ..to help listeners get information from..
     * ..network events
     */
    public delegate void BasicAction();
    public event BasicAction OnConnect;
    public event BasicAction OnDisconnect;
    public delegate void StringAction(string _msg);
    public event StringAction OnChat;
    public delegate void PlayerAction(NetworkPlayerData _player);
    public event PlayerAction OnPlayerJoined;
    public event PlayerAction OnPlayerLeft;
    public delegate void InstanceUpdateAction(NetworkInstanceData _data);
    public event InstanceUpdateAction OnHandshake;
    public event InstanceUpdateAction OnInstanceUpdated;

    public bool usePredictiveSmoothing=true;

    private SocketIOComponent m_Network;

    private static readonly string NETWORK_MESSAGE_CONNECT = "connect";
    private static readonly string NETWORK_MESSAGE_DISCONNECT = "disconnect";
    private static readonly string NETWORK_MESSAGE_HANDSHAKE = "HANDSHAKE";
    private static readonly string NETWORK_MESSAGE_PLAYER_DATA = "PLAYER";
    private static readonly string NETWORK_MESSAGE_PLAYER_LEFT = "PLAYER_LEFT";
    private static readonly string NETWORK_MESSAGE_PLAYER_JOINED = "PLAYER_JOINED";
    private static readonly string NETWORK_MESSAGE_CHAT = "CHAT";
    private static readonly string NETWORK_MESSAGE_INSTANCE = "INSTANCE";
    private static readonly string NETWORK_MESSAGE_HIT_MOB = "HIT_MOB";
    private static readonly string NETWORK_MESSAGE_INVENTORY_CHANGED = "INVENTORY_CHANGE";

    public bool IsConnected { get { return m_Network.IsConnected; } }

#region Unity Functions
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Start() {
        m_Network = GetComponent<SocketIOComponent>();
        m_Network.On(NETWORK_MESSAGE_CONNECT, OnNetworkConnected);
        m_Network.On(NETWORK_MESSAGE_DISCONNECT, OnNetworkDisconnected);
        m_Network.On(NETWORK_MESSAGE_HANDSHAKE, OnNetworkHandshake);
        m_Network.On(NETWORK_MESSAGE_INSTANCE, OnInstanceUpdate);
        m_Network.On(NETWORK_MESSAGE_PLAYER_JOINED, OnNetworkPlayerJoined);
        m_Network.On(NETWORK_MESSAGE_PLAYER_LEFT, OnNetworkPlayerLeft);
        m_Network.On(NETWORK_MESSAGE_CHAT, OnNetworkChat);
    }

    private void OnDisable() {
        m_Network.Close();
    }
#endregion

#region Private Functions
    private void OnNetworkConnected(SocketIOEvent _evt) {
        Log("Successfully connected to the server.");
        TryRunAction(OnConnect);
    }

    private void OnNetworkDisconnected(SocketIOEvent _evt) {
        Log("Disconnected from the server.");
        TryRunAction(OnDisconnect);
    }

    private void OnNetworkHandshake(SocketIOEvent _evt) {
        Log("Handshake received from the server.");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        NetworkInstanceData _instance = NetworkInstanceData.FromJsonStr<NetworkInstanceData>(_msg);
        TryRunAction(OnHandshake, _instance);
    }

    private void OnNetworkPlayerJoined(SocketIOEvent _evt) {
        Log("Player joined the server.");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        NetworkPlayerData _player = NetworkPlayerData.FromJsonStr<NetworkPlayerData>(_msg);
        TryRunAction(OnPlayerJoined, _player);
    }

    private void OnNetworkPlayerLeft(SocketIOEvent _evt) {
        Log("Player left the server.");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        NetworkPlayerData _player = NetworkPlayerData.FromJsonStr<NetworkPlayerData>(_msg);
        TryRunAction(OnPlayerLeft, _player);
    }

    private void OnInstanceUpdate(SocketIOEvent _evt) {
        Log("Instance was updated from the server.");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        NetworkInstanceData _instance = NetworkInstanceData.FromJsonStr<NetworkInstanceData>(_msg);
        TryRunAction(OnInstanceUpdated, _instance);
    }

    private void OnNetworkChat(SocketIOEvent _evt) {
        Log("Incoming chat from the server.");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        TryRunAction(OnChat, _msg);
    }

    private void TryRunAction(BasicAction _action) {
        try {
            _action();
        } catch (System.Exception) {}
    }

    private void TryRunAction(StringAction _action, string _msg) {
        try {
            _action(_msg);
        } catch (System.Exception) {}
    }

    private void TryRunAction(PlayerAction _action, NetworkPlayerData _data) {
        try {
            _action(_data);
        } catch (System.Exception) {}
    }

    private void TryRunAction(InstanceUpdateAction _action, NetworkInstanceData _data) {
        try {
            _action(_data);
        } catch (System.Exception) {}
    }

    private void SendString(string _id, string _data) {
        //Log("Sending {\"message\":\""+_data+"\"} to "+_id);
        m_Network.Emit(_id, new JSONObject("{\"message\":\""+_data+"\"}"));
    }

    private void SendNetworkData<T>(string _id, T _data) where T : NetworkModel {
        _data.timestamp = NetworkTimestamp.NowMilliseconds().ToString();
        string _json = _data.ToJsonString();
        m_Network.Emit(_id, new JSONObject(_json));
    }
#endregion

#region Public Functions
    public void Connect() {
        m_Network.Connect();
    }

    public void Close() {
        m_Network.Close();
    }

    public void SendChat(string _chat) {
        SendString(NETWORK_MESSAGE_CHAT, _chat);
    }

    public void SendHandshake(NetworkHandshake _data) {
        SendNetworkData<NetworkHandshake>(NETWORK_MESSAGE_HANDSHAKE, _data);
    }

    public void SendNetworkPlayer(NetworkPlayerData _data) {
        SendNetworkData<NetworkPlayerData>(NETWORK_MESSAGE_PLAYER_DATA, _data);
    }

    public void HitMob(NetworkMobHitInfo _data) {
        SendNetworkData<NetworkMobHitInfo>(NETWORK_MESSAGE_HIT_MOB, _data);
    }

    public void SaveInventory(NetworkInventoryUpdate _data) {
        SendNetworkData<NetworkInventoryUpdate>(NETWORK_MESSAGE_INVENTORY_CHANGED, _data);
    }
#endregion
}
