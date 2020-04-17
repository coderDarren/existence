using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class Session : GameSystem
{
    public GameObject playerObject;
    public Text chatBox;
    public InputField input;

    private NetworkController m_Network;
    private Hashtable m_Players;
    private string m_PlayerName;

    public string playerName { get{return m_PlayerName;} }

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

    private void Awake() {
        m_PlayerName = RandomString(12);
    }

    private async void Start() {
        if (!network) return;

        m_Players = new Hashtable();
        network.OnConnect += OnServerConnect;
        network.OnDisconnect += OnServerDisconnect;
        network.OnHandshake += OnServerHandshake;
        network.OnPlayerJoined += OnPlayerJoined;
        network.OnPlayerLeft += OnPlayerLeft;
        network.OnInstanceUpdated += OnInstanceUpdated;
        network.OnChat += OnChat;
    }

    private void OnServerConnect() {
        network.SendHandshake(m_PlayerName);
    }

    private void OnServerDisconnect() {
        
    }

    private void OnServerHandshake(NetworkInstanceData _instance) {
        chatBox.text += "<color=#0f0>Welcome to the Server.</color>\n";
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
        chatBox.text += "<color=#0f0>"+_player.name+" joined.</color>\n";
        SpawnPlayer(_player);
    }
    
    private void OnPlayerLeft(NetworkPlayerData _player) {
        chatBox.text += "<color=#f00>"+_player.name+" left.</color>\n";
        RemovePlayer(_player);
    }

    private void OnChat(string _msg) {
        chatBox.text += _msg+"\n";
    }

    public void SendChatMessage() {
        if (!network) return;
        if (input.text.Equals(string.Empty)) return;
        network.SendChat(m_PlayerName+": "+input.text);
        input.text = string.Empty;
    }

    private void SpawnPlayer(NetworkPlayerData _data) {
        string _name = _data.name;
        if (_name == m_PlayerName) return; //this is you..
        if (m_Players.ContainsKey(_name)) return; // player already exists
        GameObject _obj = Instantiate(playerObject);
        NetworkPlayer _player = _obj.GetComponent<NetworkPlayer>();
        _player.Init(_data);
        m_Players.Add(_name, _player);
    }

    private void RemovePlayer(NetworkPlayerData _data) {
        string _playerName = _data.name;
        if (_playerName == m_PlayerName) return; //this is you..
        if (!m_Players.ContainsKey(_playerName)) return;
        NetworkPlayer _player = (NetworkPlayer)m_Players[_playerName];
        m_Players.Remove(_playerName);
        Destroy(_player.gameObject);
    }

    private void MovePlayer(NetworkPlayerData _data) {
        string _name = _data.name;
        if (_name == m_PlayerName) return; //this is you..
        if (!m_Players.ContainsKey(_name)) return; // could not find player
        NetworkPlayer _player = (NetworkPlayer)m_Players[_name];
        _player.UpdatePosition(_data);
    }

    private void OnDisable() {
        if (!network) return;
        network.OnConnect -= OnServerConnect;
        network.OnDisconnect -= OnServerDisconnect;
        network.OnHandshake -= OnServerHandshake;
        network.OnPlayerJoined -= OnPlayerJoined;
        network.OnPlayerLeft -= OnPlayerLeft;
        network.OnInstanceUpdated -= OnInstanceUpdated;
        network.OnChat -= OnChat;
    }

    public static string RandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[Random.Range(0,chars.Length)];
        }

        return new string(stringChars);
    }

}
