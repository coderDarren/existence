using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Scene;
using UnityCore.Menu;

public class Session : GameSystem
{
    public delegate void BasicAction();
    public event BasicAction OnPlayerConnected;

    public static Session instance;

    public GameObject networkDummyObject;
    public GameObject networkPlayerObject;
    [HideInInspector]
    public PageType entryPage=PageType.Login;

    private AccountData m_Account;
    private PlayerData[] m_AccountPlayers;
    private Player m_Player;
    private PlayerController m_PlayerController;
    private NetworkPlayer m_NetworkPlayer;
    private NetworkController m_Network;
    private SceneController m_SceneController;
    private Hashtable m_Players;
    private Hashtable m_Mobs;

    public AccountData account {
        get {
            return m_Account;
        }
    }

    public PlayerData[] accountPlayers {
        get {
            return m_AccountPlayers;
        }
    }

    public Player player {
        get {
            return m_Player;
        }
    }

    public PlayerData playerData {
        get {
            return m_Player.data;
        }
        set {
            m_Player.data = value;
        }
    }

    public NetworkController network {
        get {
            if (!m_Network) {
                m_Network = NetworkController.instance;
            }
            if (!m_Network) {
                LogWarning("Trying to get network but no instance of NetworkController was found.");
            }
            return m_Network;
        }
    }

    private SceneController sceneController {
        get {
            if (!m_SceneController) {
                m_SceneController = SceneController.instance;
            }
            if (!m_SceneController) {
                Log("Trying to access SceneController, but no instance was found.");
            }
            return m_SceneController;
        }
    }

#region Unity Functions
    private void Awake() {
        if (!instance) {
            instance = this;
        }
    }

    private async void Start() {
        m_Mobs = new Hashtable();
        m_Players = new Hashtable();

        if (network) {
            Log("subscribing to network events");
            network.OnConnect += OnServerConnect;
            network.OnDisconnect += OnServerDisconnect;
            network.OnHandshake += OnServerHandshake;
            network.OnPlayerJoined += OnPlayerJoined;
            network.OnPlayerLeft += OnPlayerLeft;
            network.OnInstanceUpdated += OnInstanceUpdated;
        }
    }

    private void OnDisable() {
        if (!network) return;
        Log("unsubscribing to network events");
        network.OnConnect -= OnServerConnect;
        network.OnDisconnect -= OnServerDisconnect;
        network.OnHandshake -= OnServerHandshake;
        network.OnPlayerJoined -= OnPlayerJoined;
        network.OnPlayerLeft -= OnPlayerLeft;
        network.OnInstanceUpdated -= OnInstanceUpdated;
    }
#endregion

#region Public Functions
    public void InitAccount(AccountData _account) {
        m_Account = _account;
    }

    public void InitAccountPlayers(PlayerData[] _accountPlayers) {
        m_AccountPlayers = _accountPlayers;
    }

    public void SignOut() {
        m_Account = null;
        m_AccountPlayers = null;
    }

    private void InitPlayer(Player _player) {
        m_Player = _player;
        m_PlayerController = m_Player.GetComponent<PlayerController>();
        m_NetworkPlayer = m_Player.GetComponent<NetworkPlayer>();
        Chatbox.instance.ConfigurePlayerEvents();
    }
    /// <summary>
    /// Should be called by ChatBox when user logs in
    /// </summary>
    public void ConnectPlayer(PlayerData _player) {
        if (!network) return;
        // !! TODO
        // we probably want to actually spawn the character freshly here
        InitPlayer(GameObject.FindObjectOfType<Player>());
        m_NetworkPlayer.Init(_player);
        playerData = _player;
        network.Connect();
    }

    public void FreezePlayerInput() {
        m_PlayerController.FreezeInput();
    }

    public void FreePlayerInput() {
        m_PlayerController.FreeInput();
    }

    public void DisconnectPlayer() {
        if (network) {
            network.Close();
        }
        m_Mobs = new Hashtable();
        m_Players = new Hashtable();
        entryPage = PageType.CharacterSelection;
        if (sceneController) {
            sceneController.Load(SceneType.Login);
        }
    }
#endregion

#region Private Functions
    private void OnServerConnect() {
        m_Player.ConnectWithData(playerData);
        network.SendHandshake(m_NetworkPlayer.clientData);
        TryRunAction(OnPlayerConnected);
    }

    private void OnServerDisconnect() {
        
    }

    private void OnServerHandshake(NetworkInstanceData _instance) {
        foreach(NetworkPlayerData _player in _instance.players) {
            SpawnPlayer(_player);
        }

        foreach(NetworkMobData _mob in _instance.mobs) {
            SpawnMob(_mob);
        }
    }

    private void OnInstanceUpdated(NetworkInstanceData _instance) {
        foreach(NetworkPlayerData _player in _instance.players) {
            MovePlayer(_player);
        }

        foreach(NetworkMobData _mob in _instance.mobs) {
            MoveMob(_mob);
        }
    }

    private void OnPlayerJoined(NetworkPlayerData _player) {
        SpawnPlayer(_player);
    }
    
    private void OnPlayerLeft(NetworkPlayerData _player) {
        RemovePlayer(_player);
    }

    private void SpawnPlayer(NetworkPlayerData _data) {
        string _name = _data.name;
        if (_name == null) return;
        if (_name == playerData.player.name) return; //this is you..
        if (m_Players.ContainsKey(_name)) return; // player already exists
        GameObject _obj = Instantiate(networkPlayerObject);
        NetworkPlayer _player = _obj.GetComponent<NetworkPlayer>();
        _player.Init(_data);
        m_Players.Add(_name, _player);
    }

    private void RemovePlayer(NetworkPlayerData _data) {
        string _playerName = _data.name;
        if (_playerName == playerData.player.name) return; //this is you..
        if (!m_Players.ContainsKey(_playerName)) return;
        NetworkPlayer _player = (NetworkPlayer)m_Players[_playerName];
        m_Players.Remove(_playerName);
        Destroy(_player.gameObject);
    }

    private void MovePlayer(NetworkPlayerData _data) {
        string _name = _data.name;
        if (_name == playerData.player.name) return; //this is you..
        if (!m_Players.ContainsKey(_name)) return; // could not find player
        NetworkPlayer _player = (NetworkPlayer)m_Players[_name];
        _player.UpdatePosition(_data);
    }

    private void SpawnMob(NetworkMobData _data) {
        string _name = _data.id;
        if (m_Mobs.ContainsKey(_name)) return; // player already exists
        GameObject _obj = Instantiate(networkDummyObject);
        Mob _mob = _obj.GetComponent<Mob>();
        _mob.Init(_data);
        m_Mobs.Add(_name, _mob);
    }

    private void MoveMob(NetworkMobData _data) {
        string _name = _data.id;
        if (!m_Mobs.ContainsKey(_name)) return; // could not find player
        Mob _mob = (Mob)m_Mobs[_name];
        _mob.UpdateData(_data);
    }

    private void TryRunAction(BasicAction _action) {
        try {
            _action();
        } catch (System.Exception) {}
    }
#endregion
}
