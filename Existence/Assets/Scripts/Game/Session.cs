using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Session : GameSystem
{

    public static Session instance;

    public GameObject networkPlayerObject;
    public Player player;

    private PlayerController m_PlayerController;
    private NetworkController m_Network;
    private Hashtable m_Players;
    private PlayerData m_PlayerData;

    public PlayerData playerData {
        get {
            return m_PlayerData;
        }
    }

    private NetworkController network {
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

#region Unity Functions
    private void Awake() {
        if (!instance) {
            instance = this;
        }
    }

    private async void Start() {
        m_PlayerData = player.data;
        m_Players = new Hashtable();
        m_PlayerController = player.GetComponent<PlayerController>();

        if (network) {
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
        network.OnConnect -= OnServerConnect;
        network.OnDisconnect -= OnServerDisconnect;
        network.OnHandshake -= OnServerHandshake;
        network.OnPlayerJoined -= OnPlayerJoined;
        network.OnPlayerLeft -= OnPlayerLeft;
        network.OnInstanceUpdated -= OnInstanceUpdated;
    }
#endregion

#region Public Functions
    /// <summary>
    /// Should be called by ChatBox when user logs in
    /// </summary>
    public void ConnectPlayer(PlayerData _player) {
        if (!network) return;
        m_PlayerData = _player;
        network.Connect();
    }

    public void FreezePlayerInput() {
        m_PlayerController.FreezeInput();
    }

    public void FreePlayerInput() {
        m_PlayerController.FreeInput();
    }
#endregion

#region Private Functions
    private void OnServerConnect() {
        player.ConnectWithData(m_PlayerData);
        network.SendHandshake(m_PlayerData.player.name);
    }

    private void OnServerDisconnect() {
        
    }

    private void OnServerHandshake(NetworkInstanceData _instance) {
        foreach(NetworkPlayerData _player in _instance.players) {
            SpawnPlayer(_player);
        }
    }

    private void OnInstanceUpdated(NetworkInstanceData _instance) {
        foreach(NetworkPlayerData _player in _instance.players) {
            MovePlayer(_player);
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
        if (_name == m_PlayerData.player.name) return; //this is you..
        if (m_Players.ContainsKey(_name)) return; // player already exists
        GameObject _obj = Instantiate(networkPlayerObject);
        NetworkPlayer _player = _obj.GetComponent<NetworkPlayer>();
        _player.Init(_data);
        m_Players.Add(_name, _player);
    }

    private void RemovePlayer(NetworkPlayerData _data) {
        string _playerName = _data.name;
        if (_playerName == m_PlayerData.player.name) return; //this is you..
        if (!m_Players.ContainsKey(_playerName)) return;
        NetworkPlayer _player = (NetworkPlayer)m_Players[_playerName];
        m_Players.Remove(_playerName);
        Destroy(_player.gameObject);
    }

    private void MovePlayer(NetworkPlayerData _data) {
        string _name = _data.name;
        if (_name == m_PlayerData.player.name) return; //this is you..
        if (!m_Players.ContainsKey(_name)) return; // could not find player
        NetworkPlayer _player = (NetworkPlayer)m_Players[_name];
        _player.UpdatePosition(_data);
    }
#endregion
}
