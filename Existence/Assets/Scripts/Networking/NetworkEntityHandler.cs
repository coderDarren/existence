using System.Collections;
using UnityEngine;

/// <summary>
/// This handler is responsible for handling NeworkInstanceData..
/// ..to spawn and move various entity types.
///
/// Entities include:
///     - Players
///     - Mobs
///     - NPCs
/// </summary>
public class NetworkEntityHandler : GameSystem
{
    public GameObject networkDummyObject;
    public GameObject networkPlayerObject;

    private Session m_Session;
    private NetworkController m_Network;
    private Hashtable m_Mobs;
    private Hashtable m_Players;

    // Get the Session with integrity
    private Session session {
        get {
            if (!m_Session) {
                m_Session = Session.instance;
            }
            if (!m_Session) {
                Log("Trying to access Session, but no instance was found.");
            }
            return m_Session;
        }
    }

    // Get the NetworkController with integrity
    private NetworkController network {
        get {
            if (!m_Network) {
                m_Network = NetworkController.instance;
            }
            if (!m_Network) {
                Log("Trying to access NetworkController, but no instance was found.");
            }
            return m_Network;
        }
    }

#region Unity Functions
    private void Start() {
        m_Mobs = new Hashtable();
        m_Players = new Hashtable();

        if (!network) return;
        network.OnHandshake += OnServerHandshake;
        network.OnPlayerJoined += OnPlayerJoined;
        network.OnPlayerLeft += OnPlayerLeft;
        network.OnInstanceUpdated += OnInstanceUpdated;
    }

    private void OnDisable() {
        if (!network) return;
        network.OnHandshake -= OnServerHandshake;
        network.OnPlayerJoined -= OnPlayerJoined;
        network.OnPlayerLeft -= OnPlayerLeft;
        network.OnInstanceUpdated -= OnInstanceUpdated;
    }
#endregion

#region Public Functions

#endregion

#region Private Functions
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
        if (_name == session.playerData.player.name) return; //this is you..
        if (m_Players.ContainsKey(_name)) return; // player already exists
        GameObject _obj = Instantiate(networkPlayerObject);
        NetworkPlayer _player = _obj.GetComponent<NetworkPlayer>();
        _player.Init(_data);
        m_Players.Add(_name, _player);
        NameplateController.instance.TrackSelectable((Selectable)_player);
    }

    private void RemovePlayer(NetworkPlayerData _data) {
        string _playerName = _data.name;
        if (_playerName == session.playerData.player.name) return; //this is you..
        if (!m_Players.ContainsKey(_playerName)) return;
        NetworkPlayer _player = (NetworkPlayer)m_Players[_playerName];
        m_Players.Remove(_playerName);
        Destroy(_player.gameObject);
        NameplateController.instance.ForgetSelectable((Selectable)_player);
    }

    private void MovePlayer(NetworkPlayerData _data) {
        string _name = _data.name;
        Log(_name+": "+m_Players.ContainsKey(_name));
        if (_name == session.playerData.player.name) return; //this is you..
        if (!m_Players.ContainsKey(_name)) return; // could not find player
        NetworkPlayer _player = (NetworkPlayer)m_Players[_name];
        _player.UpdateServerPlayer(_data);
    }

    private void SpawnMob(NetworkMobData _data) {
        string _name = _data.id;
        if (_name == null) return;
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
#endregion
}
