using System.Collections.Generic;
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
    public static NetworkEntityHandler instance;

    public GameObject networkDummyObject;
    public GameObject networkPlayerObject;

    private Session m_Session;
    private NetworkController m_Network;
    private Hashtable m_PlayersHash;
    private Hashtable m_MobsHash;
    private Hashtable m_MobUpdateState;
    private Hashtable m_PlayerUpdateState;
    private List<Mob> m_Mobs;
    private List<NetworkPlayer> m_Players;

    public List<Mob> mobs {
        get {
            return m_Mobs;
        }
    }

    public List<NetworkPlayer> players {
        get {
            return m_Players;
        }
    }

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
    private void Awake() {
        if (!instance) {
            instance = this;
        }
    }
    private void Start() {
        if (instance != this) return;

        m_MobsHash = new Hashtable();
        m_MobUpdateState = new Hashtable();
        m_Mobs = new List<Mob>();
        m_PlayersHash = new Hashtable();
        m_PlayerUpdateState = new Hashtable();
        m_Players = new List<NetworkPlayer>();

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
    }

    private void OnInstanceUpdated(NetworkInstanceData _instance) {
        HandlePlayerUpdates(_instance.players);
        HandleMobUpdates(_instance.mobs);
    }

    private void HandlePlayerUpdates(NetworkPlayerData[] _players) {
        foreach (DictionaryEntry _entry in m_PlayersHash) {
            string _key = (string)_entry.Key;
            m_PlayerUpdateState[_key] = false;
        }
        
        foreach(NetworkPlayerData _player in _players) {
            // find mobs that did not update
            if (m_PlayerUpdateState.ContainsKey(_player.name)) {
                MovePlayer(_player);
                m_PlayerUpdateState[_player.name] = true;
            } else {
                SpawnPlayer(_player);
                m_PlayerUpdateState.Add(_player.name, true);
            }
        }

        // compare old mob hash to this new stuff
        for (int i = m_Players.Count - 1; i >= 0; i--) {
            string _key = m_Players[i].name;
            if (_key == string.Empty) continue;
            bool _didUpdate = (bool)m_PlayerUpdateState[_key];
            if (!_didUpdate) {
                NetworkPlayer _player = (NetworkPlayer)m_PlayersHash[_key];
                RemovePlayer(_player);
                m_Players.RemoveAt(i);
            }
        }
    }

    private void HandleMobUpdates(NetworkMobData[] _mobs) {
        foreach (DictionaryEntry _entry in m_MobsHash) {
            string _key = (string)_entry.Key;
            m_MobUpdateState[_key] = false;
        }
        
        foreach(NetworkMobData _mob in _mobs) {
            // find mobs that did not update
            if (m_MobUpdateState.ContainsKey(_mob.id)) {
                MoveMob(_mob);
                m_MobUpdateState[_mob.id] = true;
            } else {
                SpawnMob(_mob);
                m_MobUpdateState.Add(_mob.id, true);
            }
        }

        // compare old mob hash to this new stuff
        for (int i = m_Mobs.Count - 1; i >= 0; i--) {
            string _key = m_Mobs[i].id;
            bool _didUpdate = (bool)m_MobUpdateState[_key];
            if (!_didUpdate) {
                Mob _mob = (Mob)m_MobsHash[_key];
                RemoveMob(_mob);
                m_Mobs.RemoveAt(i);
            }
        }
    }

    private void OnPlayerJoined(NetworkPlayerData _player) {
        SpawnPlayer(_player);
    }
    
    private void OnPlayerLeft(NetworkPlayerData _data) {
        NetworkPlayer _player = (NetworkPlayer)m_PlayersHash[_data.name];
        RemovePlayer(_player);
    }

    private void SpawnPlayer(NetworkPlayerData _data) {
        string _name = _data.name;
        if (_name == null) return;
        if (_name == session.playerData.player.name) return; //this is you..
        if (m_PlayersHash.ContainsKey(_name)) return; // player already exists
        GameObject _obj = Instantiate(networkPlayerObject);
        NetworkPlayer _player = _obj.GetComponent<NetworkPlayer>();
        _player.Init(_data);
        m_PlayersHash.Add(_name, _player);
        m_Players.Add(_player);
        NameplateController.instance.TrackSelectable((Selectable)_player);
    }

    private void RemovePlayer(NetworkPlayer _data) {
        string _playerName = _data.name;
        if (_playerName == session.playerData.player.name) return; //this is you..
        if (!m_PlayersHash.ContainsKey(_playerName)) return;
        //NetworkPlayer _player = (NetworkPlayer)m_PlayersHash[_playerName];
        m_PlayersHash.Remove(_playerName);
        m_PlayerUpdateState.Remove(_playerName);
        Destroy(_data.gameObject);
        NameplateController.instance.ForgetSelectable((Selectable)_data);
    }

    private void MovePlayer(NetworkPlayerData _data) {
        string _name = _data.name;

        if (_name == session.playerData.player.name) return; //this is you..
        if (!m_PlayersHash.ContainsKey(_name)) return; // could not find player
        NetworkPlayer _player = (NetworkPlayer)m_PlayersHash[_name];
        _player.UpdateServerPlayer(_data);
    }

    private void SpawnMob(NetworkMobData _data) {
        string _name = _data.id;
        if (_name == null) return;
        if (m_MobsHash.ContainsKey(_name)) return; // player already exists
        GameObject _obj = Instantiate(networkDummyObject);
        Mob _mob = _obj.GetComponent<Mob>();
        _mob.Init(_data);
        m_MobsHash.Add(_name, _mob);
        m_Mobs.Add(_mob);
        NameplateController.instance.TrackSelectable((Selectable)_mob);
    }

    private void MoveMob(NetworkMobData _data) {
        string _name = _data.id;
        if (!m_MobsHash.ContainsKey(_name)) return; // could not find player
        Mob _mob = (Mob)m_MobsHash[_name];
        _mob.UpdateData(_data);
    }

    private void RemoveMob(Mob _mob) {
        string _name = _mob.id;
        if (!m_MobsHash.ContainsKey(_name)) return;
        NameplateController.instance.ForgetSelectable((Selectable)_mob);
        m_MobsHash.Remove(_name);
        m_MobUpdateState.Remove(_name);
        Destroy(_mob.gameObject);
    }
#endregion
}
