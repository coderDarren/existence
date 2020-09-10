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

    private SocketIOComponent m_Network;

    private static readonly string NETMSG_CONNECT = "connect";
    private static readonly string NETMSG_DISCONNECT = "disconnect";
    private static readonly string NETMSG_HANDSHAKE = "HANDSHAKE";
    private static readonly string NETMSG_PLAYER_DATA = "PLAYER";
    private static readonly string NETMSG_PLAYER_TRANSFORM_CHANGE = "PLAYER_TRANSFORM_CHANGE";
    private static readonly string NETMSG_PLAYER_HEALTH_CHANGE = "PLAYER_HEALTH_CHANGE";
    private static readonly string NETMSG_PLAYER_LVL_CHANGE = "PLAYER_LVL_CHANGE";
    private static readonly string NETMSG_PLAYER_LEFT = "PLAYER_LEFT";
    private static readonly string NETMSG_PLAYER_JOINED = "PLAYER_JOINED";
    private static readonly string NETMSG_PLAYER_ANIM_FLOAT = "NETMSG_PLAYER_ANIM_FLOAT";
    private static readonly string NETMSG_PLAYER_ANIM_BOOL = "NETMSG_PLAYER_ANIM_BOOL";
    private static readonly string NETMSG_CHAT = "CHAT";
    private static readonly string NETMSG_INSTANCE = "INSTANCE";
    private static readonly string NETMSG_HIT_MOB = "HIT_MOB";
    private static readonly string NETMSG_MOB_ATTACK = "MOB_ATTACK";
    private static readonly string NETMSG_MOB_ATTACK_START = "MOB_ATTACK_START";
    private static readonly string NETMSG_MOB_HIT_PLAYER = "MOB_HIT_PLAYER";
    private static readonly string NETMSG_INVENTORY_CHANGED = "INVENTORY_CHANGE";
    private static readonly string NETMSG_ADD_INVENTORY = "ADD_INVENTORY";
    private static readonly string NETMSG_ADD_INVENTORY_SUCCESS = "ADD_INVENTORY_SUCCESS";
    private static readonly string NETMSG_ADD_INVENTORY_FAILURE = "ADD_INVENTORY_FAILURE";
    private static readonly string NETMSG_RM_INVENTORY_SUCCESS = "RM_INVENTORY_SUCCESS";
    private static readonly string NETMSG_RM_INVENTORY_FAILURE = "RM_INVENTORY_FAILURE";
    private static readonly string NETMSG_MOB_SPAWN = "MOB_SPAWN";
    private static readonly string NETMSG_MOB_EXIT = "MOB_EXIT";
    private static readonly string NETMSG_PLAYER_SPAWN = "PLAYER_SPAWN";
    private static readonly string NETMSG_PLAYER_EXIT = "PLAYER_EXIT";
    private static readonly string NETMSG_MOB_COMBAT_STATE_CHANGE = "MOB_COMBAT_STATE_CHANGE";
    private static readonly string NETMSG_MOB_ATTACK_RANGE_CHANGE = "MOB_ATTACK_RANGE_STATE_CHANGE";
    private static readonly string NETMSG_MOB_HEALTH_CHANGE = "MOB_HEALTH_CHANGE";
    private static readonly string NETMSG_PLAYER_HIT_MOB_CONFIRMATION = "PLAYER_HIT_MOB_CONFIRMATION";
    private static readonly string NETMSG_MOB_DEATH = "MOB_DEATH";
    private static readonly string NETMSG_PLAYER_LOOT_MOB = "PLAYER_LOOT_MOB";
    private static readonly string NETMSG_MOB_LOOTED = "MOB_LOOTED";
    private static readonly string NETMSG_MOB_LOOT_LOCKED = "PLAYER_MOB_LOOT_LOCKED";
    private static readonly string NETMSG_PLAYER_EQUIP = "PLAYER_EQUIP";
    private static readonly string NETMSG_PLAYER_UNEQUIP = "PLAYER_UNEQUIP";
    private static readonly string NETMSG_PLAYER_EQUIP_SUCCESS = "PLAYER_EQUIP_SUCCESS";
    private static readonly string NETMSG_PLAYER_EQUIP_FAILURE = "PLAYER_EQUIP_FAILURE";
    private static readonly string NETMSG_PLAYER_UNEQUIP_SUCCESS = "PLAYER_UNEQUIP_SUCCESS";
    private static readonly string NETMSG_PLAYER_UNEQUIP_FAILURE = "PLAYER_UNEQUIP_FAILURE";
    private static readonly string NETMSG_INTERACT_SHOP = "NETMSG_INTERACT_SHOP";
    private static readonly string NETMSG_INTERACT_SHOP_SUCCESS = "NETMSG_INTERACT_SHOP_SUCCESS";
    private static readonly string NETMSG_TRADE_SHOP = "NETMSG_TRADE_SHOP";
    private static readonly string NETMSG_TRADE_SHOP_SUCCESS = "NETMSG_TRADE_SHOP_SUCCESS";

    /*
     * Some helpful events to subscribe to..
     * ..to help listeners get information from..
     * ..network events
     */
    public delegate void BasicAction();
    public delegate void StringAction(string _msg);

    public event BasicAction OnConnect;
    public event BasicAction OnDisconnect;
    
    public NetworkEventHandler<string> chatEvt {get; private set;}
    public NetworkEventHandler<NetworkInstanceData> handshakeEvt {get; private set;}
    public NetworkEventHandler<NetworkInstanceData> instanceDataEvt {get; private set;}

#region PLAYER NETWORK EVENTS
    public NetworkEventHandler<NetworkPlayerData> playerJoinEvt {get; private set;}
    public NetworkEventHandler<NetworkPlayerData> playerLeaveEvt {get; private set;}
    public NetworkEventHandler<NetworkPlayerData> playerSpawnEvt {get; private set;}
    public NetworkEventHandler<NetworkTransform> playerTransformEvt {get; private set;}
    public NetworkEventHandler<NetworkPlayerLvl> playerLvlEvt {get; private set;}
    public NetworkEventHandler<NetworkPlayerHealth> playerHealthEvt {get; private set;}
    public NetworkEventHandler<NetworkMobHitInfo> playerHitEvt {get; private set;}
    public NetworkEventHandler<NetworkEquipSuccessData> playerEquipSuccessEvt {get; private set;}
    public NetworkEventHandler<NetworkEquipSuccessData> playerUnequipSuccessEvt {get; private set;}
    public NetworkEventHandler<NetworkAnimFloat> playerAnimFloatEvt {get; private set;}
    public NetworkEventHandler<NetworkAnimBool> playerAnimBoolEvt {get; private set;}
    public NetworkEventHandler<string> playerEquipFailEvt {get; private set;}
    public NetworkEventHandler<string> playerUnequipFailEvt {get; private set;}
    public NetworkEventHandler<string> playerExitEvt {get; private set;}
#endregion

#region INVENTORY NETWORK EVENTS
    public NetworkEventHandler<NetworkInventoryRemoveData> rmInventorySuccessEvt {get; private set;}
    public NetworkEventHandler<NetworkInventoryRemoveData> rmInventoryFailEvt {get; private set;}
    public NetworkEventHandler<string> addInventoryEvt {get; private set;}
    public NetworkEventHandler<string> addInventoryFailEvt {get; private set;}
#endregion

#region MOB NETWORK EVENTS
    public NetworkEventHandler<NetworkMobData> mobSpawnEvt;
    public NetworkEventHandler<NetworkMobData> mobAttackRangeStateChangeEvt;
    public NetworkEventHandler<NetworkMobData> mobCombatStateChangeEvt;
    public NetworkEventHandler<NetworkMobData> mobHealthChangeEvt;
    public NetworkEventHandler<string> mobExitEvt;
    public NetworkEventHandler<NetworkMobAttackData> mobAttackEvt;
    public NetworkEventHandler<NetworkMobAttackData> mobAttackStartEvt;
    public NetworkEventHandler<NetworkPlayerHitInfo> mobHitEvt;
    public NetworkEventHandler<NetworkMobDeathData> mobDeathEvt;
#endregion

#region LOOT NETWORK EVENTS
    public NetworkEventHandler<NetworkMobLootData> mobLootEvt;
#endregion

#region SHOP TERMINAL EVENTS
    public NetworkEventHandler<NetworkShopTerminalData> shopTerminalInteractEvt;
    public NetworkEventHandler<NetworkShopTerminalTradeSuccessData> shopTerminalTradeSuccessEvt;
#endregion

    public bool usePredictiveSmoothing=true;

    public bool IsConnected { get { return m_Network.IsConnected; } }

#region Unity Functions
    private void Awake() {
        if (instance == null) {
            instance = this;
            ConfigureEventHandlers();
        }
    }

    private void Start() {
        if (instance != this) return;
        SubscribeEventHandlers();
    }

    private void OnDisable() {
        if (instance != this) return;
        if (!m_Network) return;
        m_Network.Close();
    }
#endregion

#region Private Functions
    private void ConfigureEventHandlers() {
        chatEvt = new NetworkEventHandler<string>("Incoming chat", debug);
        handshakeEvt = new NetworkEventHandler<NetworkInstanceData>("Incoming handshake.", debug);        
        instanceDataEvt = new NetworkEventHandler<NetworkInstanceData>("Network instance updated.", debug);
        
        // player events
        playerJoinEvt = new NetworkEventHandler<NetworkPlayerData>("Player joined.", debug);
        playerLeaveEvt = new NetworkEventHandler<NetworkPlayerData>("Player left.", debug);
        playerSpawnEvt = new NetworkEventHandler<NetworkPlayerData>("Player entered range.", debug);
        playerTransformEvt = new NetworkEventHandler<NetworkTransform>("Player transform changed.", debug);
        playerLvlEvt = new NetworkEventHandler<NetworkPlayerLvl>("Player lvl changed.", debug);
        playerHealthEvt = new NetworkEventHandler<NetworkPlayerHealth>("Player health changed.", debug);
        playerHitEvt = new NetworkEventHandler<NetworkMobHitInfo>("Player hit mob.", debug);
        playerEquipSuccessEvt = new NetworkEventHandler<NetworkEquipSuccessData>("Player equipped.", debug);
        playerUnequipSuccessEvt = new NetworkEventHandler<NetworkEquipSuccessData>("Player unequipped.", debug);
        playerEquipFailEvt = new NetworkEventHandler<string>("Player failed to equip.", debug);
        playerUnequipFailEvt = new NetworkEventHandler<string>("Player failed to unequip.", debug);
        playerExitEvt = new NetworkEventHandler<string>("Player exited range.", debug);
        playerAnimFloatEvt = new NetworkEventHandler<NetworkAnimFloat>("Player float animation changed.", debug);
        playerAnimBoolEvt = new NetworkEventHandler<NetworkAnimBool>("Player bool animation changed.", debug);

        // inventory events
        rmInventorySuccessEvt = new NetworkEventHandler<NetworkInventoryRemoveData>("Successfully removed inventory.", debug);
        rmInventoryFailEvt = new NetworkEventHandler<NetworkInventoryRemoveData>("Failed to remove inventory.", debug);
        addInventoryEvt = new NetworkEventHandler<string>("Successfully added inventory.", debug);
        addInventoryFailEvt = new NetworkEventHandler<string>("Failed to add inventory.", debug);

        // mob events
        mobSpawnEvt = new NetworkEventHandler<NetworkMobData>("Mob spawned.", debug);
        mobAttackRangeStateChangeEvt = new NetworkEventHandler<NetworkMobData>("Mob attack range state changed.", debug);
        mobCombatStateChangeEvt = new NetworkEventHandler<NetworkMobData>("Mob combat state changed.", debug);
        mobHealthChangeEvt = new NetworkEventHandler<NetworkMobData>("Mob health changed.", debug);
        mobExitEvt = new NetworkEventHandler<string>("Mob exited range.", debug);
        mobAttackEvt = new NetworkEventHandler<NetworkMobAttackData>("Mob attacked.", debug);
        mobAttackStartEvt = new NetworkEventHandler<NetworkMobAttackData>("Mob started attacking.", debug);
        mobHitEvt = new NetworkEventHandler<NetworkPlayerHitInfo>("Mob hit player.", debug);
        mobDeathEvt = new NetworkEventHandler<NetworkMobDeathData>("Mob died.", debug);

        // loot events
        mobLootEvt = new NetworkEventHandler<NetworkMobLootData>("Mob looted.", debug);

        // shop terminal events
        shopTerminalInteractEvt = new NetworkEventHandler<NetworkShopTerminalData>("Shop terminal was interacted with.", debug);
        shopTerminalTradeSuccessEvt = new NetworkEventHandler<NetworkShopTerminalTradeSuccessData>("Shop terminal trade was successful.", debug);
    }

    private void SubscribeEventHandlers() {
        m_Network = GetComponent<SocketIOComponent>();

        // server events
        m_Network.On(NETMSG_CONNECT, OnNetworkConnected);
        m_Network.On(NETMSG_DISCONNECT, OnNetworkDisconnected);
        m_Network.On(NETMSG_HANDSHAKE, handshakeEvt.HandleEvt);
    
        m_Network.On(NETMSG_INSTANCE, instanceDataEvt.HandleEvt);
        m_Network.On(NETMSG_CHAT, chatEvt.HandleEvt);

        // player events
        m_Network.On(NETMSG_PLAYER_SPAWN, playerSpawnEvt.HandleEvt);
        m_Network.On(NETMSG_PLAYER_EXIT, playerExitEvt.HandleEvt);
        m_Network.On(NETMSG_PLAYER_JOINED, playerJoinEvt.HandleEvt);
        m_Network.On(NETMSG_PLAYER_LEFT, playerLeaveEvt.HandleEvt);
        m_Network.On(NETMSG_PLAYER_TRANSFORM_CHANGE, playerTransformEvt.HandleEvt);
        m_Network.On(NETMSG_PLAYER_LVL_CHANGE, playerLvlEvt.HandleEvt);
        m_Network.On(NETMSG_PLAYER_HEALTH_CHANGE, playerHealthEvt.HandleEvt);
        m_Network.On(NETMSG_PLAYER_HIT_MOB_CONFIRMATION, playerHitEvt.HandleEvt);
        m_Network.On(NETMSG_PLAYER_EQUIP_SUCCESS, playerEquipSuccessEvt.HandleEvt);
        m_Network.On(NETMSG_PLAYER_UNEQUIP_SUCCESS, playerUnequipSuccessEvt.HandleEvt);
        m_Network.On(NETMSG_PLAYER_EQUIP_FAILURE, playerEquipFailEvt.HandleEvt);
        m_Network.On(NETMSG_PLAYER_UNEQUIP_FAILURE, playerUnequipFailEvt.HandleEvt);
        m_Network.On(NETMSG_PLAYER_ANIM_FLOAT, playerAnimFloatEvt.HandleEvt);
        m_Network.On(NETMSG_PLAYER_ANIM_BOOL, playerAnimBoolEvt.HandleEvt);

        // inventory events
        m_Network.On(NETMSG_ADD_INVENTORY_SUCCESS, addInventoryEvt.HandleEvt);
        m_Network.On(NETMSG_ADD_INVENTORY_FAILURE, addInventoryFailEvt.HandleEvt);
        m_Network.On(NETMSG_RM_INVENTORY_SUCCESS, rmInventorySuccessEvt.HandleEvt);
        m_Network.On(NETMSG_RM_INVENTORY_FAILURE, rmInventoryFailEvt.HandleEvt);

        // mob events
        m_Network.On(NETMSG_MOB_SPAWN, mobSpawnEvt.HandleEvt);
        m_Network.On(NETMSG_MOB_EXIT, mobExitEvt.HandleEvt);
        m_Network.On(NETMSG_MOB_COMBAT_STATE_CHANGE, mobCombatStateChangeEvt.HandleEvt);
        m_Network.On(NETMSG_MOB_ATTACK_RANGE_CHANGE, mobAttackRangeStateChangeEvt.HandleEvt);
        m_Network.On(NETMSG_MOB_HEALTH_CHANGE, mobHealthChangeEvt.HandleEvt);
        m_Network.On(NETMSG_MOB_ATTACK_START, mobAttackStartEvt.HandleEvt);
        m_Network.On(NETMSG_MOB_DEATH, mobDeathEvt.HandleEvt);
        m_Network.On(NETMSG_MOB_HIT_PLAYER, mobHitEvt.HandleEvt);
        m_Network.On(NETMSG_MOB_ATTACK, mobAttackEvt.HandleEvt);

        // loot events
        m_Network.On(NETMSG_MOB_LOOTED, mobLootEvt.HandleEvt);

        // shop terminal events
        m_Network.On(NETMSG_INTERACT_SHOP_SUCCESS, shopTerminalInteractEvt.HandleEvt);
        m_Network.On(NETMSG_TRADE_SHOP_SUCCESS, shopTerminalTradeSuccessEvt.HandleEvt);
    }

    private void OnNetworkConnected(SocketIOEvent _evt) {
        Log("Successfully connected to the server.");
        TryRunAction(OnConnect);
    }

    private void OnNetworkDisconnected(SocketIOEvent _evt) {
        Log("Disconnected from the server.");
        TryRunAction(OnDisconnect);
    }

    private void TryRunAction(BasicAction _action) {
        try {
            _action();
        } catch (System.Exception _e) {
            Debug.LogWarning(_e);
        }
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

    public void SendPlayerHealth(NetworkPlayerHealth _data) {
        SendNetworkData<NetworkPlayerHealth>(NETMSG_PLAYER_HEALTH_CHANGE, _data);
    }

    public void SendPlayerLevel(NetworkPlayerLvl _data) {
        SendNetworkData<NetworkPlayerLvl>(NETMSG_PLAYER_LVL_CHANGE, _data);
    }
    
    public void SendPlayerAnimFloat(NetworkAnimFloat _data) {
        SendNetworkData<NetworkAnimFloat>(NETMSG_PLAYER_ANIM_FLOAT, _data);
    }

    public void SendPlayerAnimBool(NetworkAnimBool _data) {
        SendNetworkData<NetworkAnimBool>(NETMSG_PLAYER_ANIM_BOOL, _data);
    }

    public void SendPlayerTransform(NetworkTransform _data) {
        SendNetworkData<NetworkTransform>(NETMSG_PLAYER_TRANSFORM_CHANGE, _data);
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

    public void LootMob(NetworkPlayerLootData _data) {
        SendNetworkData<NetworkPlayerLootData>(NETMSG_PLAYER_LOOT_MOB, _data);
    }

    public void Equip(NetworkEquipData _data) {
        SendNetworkData<NetworkEquipData>(NETMSG_PLAYER_EQUIP, _data);
    }
    
    public void Unequip(NetworkEquipData _data) {
        SendNetworkData<NetworkEquipData>(NETMSG_PLAYER_UNEQUIP, _data);
    }

    public void InteractShop(NetworkShopTerminalInteractData _data) {
        SendNetworkData<NetworkShopTerminalInteractData>(NETMSG_INTERACT_SHOP, _data);
    }

    public void TradeShop(NetworkShopTerminalTradeData _data) {
        SendNetworkData<NetworkShopTerminalTradeData>(NETMSG_TRADE_SHOP, _data);
    }
#endregion
}

/*
 * This class designed in response to unwieldy network methods..
 * Solves an issue where each NetworkModel type would require its..
 * ..own "HandleEvt()" and "TryRunAction()" functions
 *
 * This class supports NetworkModel and string types
 */
public class NetworkEventHandler<T> {
    
    public delegate void NetworkAction(T _data);
    public delegate void StringAction(string _msg);

    public event NetworkAction OnEvt;
    public event StringAction OnMsg;

    private string m_EvtLog;
    private bool m_Debug;
    private bool m_Invalid;

    public NetworkEventHandler(string _evtLog, bool _debug) {
        m_EvtLog = _evtLog;
        m_Debug = _debug;

        if (!typeof(NetworkModel).IsAssignableFrom(typeof(T)) && !typeof(string).IsAssignableFrom(typeof(T))) {
            m_Invalid = true;
            Debug.LogError("Network Event Handler for event must be of type NetworkModel or string: "+_evtLog);
        }
    }

    public void HandleEvt(SocketIOEvent _evt) {
        if (m_Invalid) return;

        string _msg = Regex.Unescape((string)_evt.data.ToDictionary()["message"]);

        if (typeof(NetworkModel).IsAssignableFrom(typeof(T))) {
            if (m_Debug) 
                Debug.Log(m_EvtLog+" NETWORK MODEL "+typeof(T)+": "+_msg);
            T _netData = NetworkModel.FromJsonStr<T>(_msg);
            TryRunAction(OnEvt, _netData);
        } else if (typeof(T) == typeof(string)) {
            if (m_Debug)
                Debug.Log(m_EvtLog+" NETWORK MODEL STRING: "+_msg);
            TryRunAction(OnMsg, _msg);
        } else {
            if (m_Debug)
                Debug.LogWarning(m_EvtLog+" NO EVENT EMITTED FOR "+_msg+" "+typeof(T));
        }
    }

    private void TryRunAction(NetworkAction _action, T _data) {
        if (m_Invalid) return;

        try {
            _action(_data);
        } catch (System.Exception _e) {
            if (m_Debug) {
                Debug.LogWarning(_e);
            }
        }
    }

    private void TryRunAction(StringAction _action, string _data) {
        if (m_Invalid) return;

        try {
            _action(_data);
        } catch (System.Exception _e) {
            if (m_Debug) {
                Debug.LogWarning(_e);
            }
        }
    }
}