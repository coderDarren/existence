using System.Text.RegularExpressions;
using UnityEngine;
using SocketIO;

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
        m_Network.Connect();
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
        NetworkPlayerData _instance = NetworkPlayerData.FromJsonStr<NetworkPlayerData>(_msg);
        TryRunAction(OnPlayerJoined, _instance);
    }

    private void OnNetworkPlayerLeft(SocketIOEvent _evt) {
        Log("Player left the server.");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        NetworkPlayerData _instance = NetworkPlayerData.FromJsonStr<NetworkPlayerData>(_msg);
        TryRunAction(OnPlayerLeft, _instance);
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
#endregion

#region Public Functions
    public void SendHandshake(string _data) {
        SendString(NETWORK_MESSAGE_HANDSHAKE, _data);
    }

    public void SendChat(string _chat) {
        SendString(NETWORK_MESSAGE_CHAT, _chat);
    }

    public void SendNetworkPlayer(NetworkPlayerData _data) {
        _data.timestamp = NetworkTimestamp.NowMilliseconds().ToString();
        string _json = _data.ToJsonString();
        m_Network.Emit(NETWORK_MESSAGE_PLAYER_DATA, new JSONObject(_json));
    }
#endregion
}
