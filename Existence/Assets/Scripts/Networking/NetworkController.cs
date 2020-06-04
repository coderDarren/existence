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
    public event StringAction OnAddInventoryFail;
    public delegate void InstanceUpdateAction(NetworkInstanceData _data);
    public event InstanceUpdateAction OnHandshake;
    public event InstanceUpdateAction OnInstanceUpdated;

#region PLAYER NETWORK EVENTS
    public delegate void PlayerAction(NetworkPlayerData _player);
    public event PlayerAction OnPlayerJoined;
    public event PlayerAction OnPlayerLeft;
    public event PlayerAction OnPlayerSpawn;
    public event StringAction OnPlayerExit;
    public delegate void InventoryUpdateAction(ItemData _data);
    public event InventoryUpdateAction OnInventoryAdded;
    public delegate void PlayerHitAction(NetworkPlayerHitInfo _data);
    public event PlayerHitAction OnPlayerHit;
#endregion

#region MOB NETWORK EVENTS
    public delegate void MobAction(NetworkMobData _mob);
    public event MobAction OnMobSpawn;
    public event StringAction OnMobExit;
    public event MobAction OnMobAttackRangeStateChange;
    public event MobAction OnMobCombatStateChange;
    public delegate void MobAttackAction(NetworkMobAttackData _data);
    public event MobAttackAction OnMobAttack;
#endregion

    public bool usePredictiveSmoothing=true;

    private SocketIOComponent m_Network;

    private static readonly string NETMSG_CONNECT = "connect";
    private static readonly string NETMSG_DISCONNECT = "disconnect";
    private static readonly string NETMSG_HANDSHAKE = "HANDSHAKE";
    private static readonly string NETMSG_PLAYER_DATA = "PLAYER";
    private static readonly string NETMSG_PLAYER_LEFT = "PLAYER_LEFT";
    private static readonly string NETMSG_PLAYER_JOINED = "PLAYER_JOINED";
    private static readonly string NETMSG_CHAT = "CHAT";
    private static readonly string NETMSG_INSTANCE = "INSTANCE";
    private static readonly string NETMSG_HIT_MOB = "HIT_MOB";
    private static readonly string NETMSG_MOB_ATTACK = "MOB_ATTACK";
    private static readonly string NETMSG_MOB_HIT_PLAYER = "MOB_HIT_PLAYER";
    private static readonly string NETMSG_INVENTORY_CHANGED = "INVENTORY_CHANGE";
    private static readonly string NETMSG_ADD_INVENTORY = "ADD_INVENTORY";
    private static readonly string NETMSG_ADD_INVENTORY_SUCCESS = "ADD_INVENTORY_SUCCESS";
    private static readonly string NETMSG_ADD_INVENTORY_FAILURE = "ADD_INVENTORY_FAILURE";
    private static readonly string NETMSG_MOB_SPAWN = "MOB_SPAWN";
    private static readonly string NETMSG_MOB_EXIT = "MOB_EXIT";
    private static readonly string NETMSG_PLAYER_SPAWN = "PLAYER_SPAWN";
    private static readonly string NETMSG_PLAYER_EXIT = "PLAYER_EXIT";
    private static readonly string NETMSG_MOB_COMBAT_STATE_CHANGE = "MOB_COMBAT_STATE_CHANGE";
    private static readonly string NETMSG_MOB_ATTACK_RANGE_CHANGE = "MOB_ATTACK_RANGE_STATE_CHANGE";

    public bool IsConnected { get { return m_Network.IsConnected; } }

#region Unity Functions
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Start() {
        m_Network = GetComponent<SocketIOComponent>();
        m_Network.On(NETMSG_CONNECT, OnNetworkConnected);
        m_Network.On(NETMSG_DISCONNECT, OnNetworkDisconnected);
        m_Network.On(NETMSG_HANDSHAKE, OnNetworkHandshake);
        m_Network.On(NETMSG_INSTANCE, OnInstanceUpdate);
        m_Network.On(NETMSG_PLAYER_JOINED, OnNetworkPlayerJoined);
        m_Network.On(NETMSG_PLAYER_LEFT, OnNetworkPlayerLeft);
        m_Network.On(NETMSG_CHAT, OnNetworkChat);
        m_Network.On(NETMSG_MOB_HIT_PLAYER, OnNetworkHitPlayer);
        m_Network.On(NETMSG_MOB_ATTACK, OnNetworkMobAttack);
        m_Network.On(NETMSG_ADD_INVENTORY_SUCCESS, OnAddInventorySuccess);
        m_Network.On(NETMSG_ADD_INVENTORY_FAILURE, OnAddInventoryFailure);
        m_Network.On(NETMSG_PLAYER_SPAWN, OnNetworkPlayerSpawn);
        m_Network.On(NETMSG_PLAYER_EXIT, OnNetworkPlayerExit);
        m_Network.On(NETMSG_MOB_SPAWN, OnNetworkMobSpawn);
        m_Network.On(NETMSG_MOB_EXIT, OnNetworkMobExit);
        m_Network.On(NETMSG_MOB_COMBAT_STATE_CHANGE, OnNetworkMobCombatStateChange);
        m_Network.On(NETMSG_MOB_ATTACK_RANGE_CHANGE, OnNetworkMobAttackRangeStateChange);
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

    private void OnAddInventorySuccess(SocketIOEvent _evt) {
        Log("Add inventory success.");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        ItemData _item = ItemData.FromJsonStr<ItemData>(_msg);
        TryRunAction(OnInventoryAdded, _item);
    }

    private void OnAddInventoryFailure(SocketIOEvent _evt) {
        Log("Add inventory fail.");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        TryRunAction(OnAddInventoryFail, _msg);
    }

    private void OnNetworkHitPlayer(SocketIOEvent _evt) {
        Log("Player was hit");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        NetworkPlayerHitInfo _info = NetworkPlayerHitInfo.FromJsonStr<NetworkPlayerHitInfo>(_msg);
        TryRunAction(OnPlayerHit, _info);
    }
    
    private void OnNetworkMobAttack(SocketIOEvent _evt) {
        Log("Mob attacked");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        NetworkMobAttackData _data = NetworkMobAttackData.FromJsonStr<NetworkMobAttackData>(_msg);
        TryRunAction(OnMobAttack, _data);
    }

    private void OnNetworkPlayerSpawn(SocketIOEvent _evt) {
        Log("Player spawned");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        NetworkPlayerData _data = NetworkPlayerData.FromJsonStr<NetworkPlayerData>(_msg);
        TryRunAction(OnPlayerSpawn, _data);
    }

    private void OnNetworkPlayerExit(SocketIOEvent _evt) {
        Log("Player out of range");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        TryRunAction(OnPlayerExit, _msg);
    }

    private void OnNetworkMobSpawn(SocketIOEvent _evt) {
        Log("Mob spawned");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        NetworkMobData _data = NetworkMobData.FromJsonStr<NetworkMobData>(_msg);
        TryRunAction(OnMobSpawn, _data);
    }

    private void OnNetworkMobExit(SocketIOEvent _evt) {
        Log("Mob out of range");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        TryRunAction(OnMobExit, _msg);
    }

    private void OnNetworkMobAttackRangeStateChange(SocketIOEvent _evt) {
        Log("Mob attack range state changed");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        NetworkMobData _data = NetworkMobData.FromJsonStr<NetworkMobData>(_msg);
        TryRunAction(OnMobAttackRangeStateChange, _data);
    }

    private void OnNetworkMobCombatStateChange(SocketIOEvent _evt) {
        Log("Mob combat state changed");
        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);
        NetworkMobData _data = NetworkMobData.FromJsonStr<NetworkMobData>(_msg);
        TryRunAction(OnMobCombatStateChange, _data);
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

    private void TryRunAction(InventoryUpdateAction _action, ItemData _item) {
        try {
            _action(_item);
        } catch (System.Exception) {}
    }

    private void TryRunAction(PlayerHitAction _action, NetworkPlayerHitInfo _data) {
        try {
            _action(_data);
        } catch (System.Exception) {}
    }

    private void TryRunAction(MobAttackAction _action, NetworkMobAttackData _data) {
        try {
            _action(_data);
        } catch (System.Exception) {}
    }

    private void TryRunAction(MobAction _action, NetworkMobData _data) {
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
        SendString(NETMSG_CHAT, _chat);
    }

    public void SendHandshake(NetworkHandshake _data) {
        SendNetworkData<NetworkHandshake>(NETMSG_HANDSHAKE, _data);
    }

    public void SendNetworkPlayer(NetworkPlayerData _data) {
        SendNetworkData<NetworkPlayerData>(NETMSG_PLAYER_DATA, _data);
    }

    public void HitMob(NetworkMobHitInfo _data) {
        SendNetworkData<NetworkMobHitInfo>(NETMSG_HIT_MOB, _data);
    }

    public void SaveInventory(NetworkInventoryUpdate _data) {
        SendNetworkData<NetworkInventoryUpdate>(NETMSG_INVENTORY_CHANGED, _data);
    }

    public void AddInventory(ItemData _data) {
        SendNetworkData<ItemData>(NETMSG_ADD_INVENTORY, _data);
    }
#endregion
}
