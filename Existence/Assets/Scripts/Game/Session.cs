using System.Collections;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using UnityCore.Scene;
using UnityCore.Menu;
using UnityCore.Audio;

/// <summary>
/// Session is responsible for collecting and maintaining player data..
/// ..from login to gameplay to exit
/// </summary>
public class Session : GameSystem
{
    public delegate void BasicAction();
    public event BasicAction OnPlayerConnected;
    public event BasicAction OnPlayerDisconnected;

    public static Session instance;

    public bool testing;
    public GameObject playerObject;
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
            if (m_Player == null) {
                Log("Trying to get player data, but no instance of Player was found.");
            }
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

    private void Start() {
        if (testing) {
            InitTestGame();
        }

        // params (audioType, fadeIn(optional, default=false), maxVolume(optional, default=1), delay(optional, default=0))
        AudioController.instance.PlayAudio(UnityCore.Audio.AudioType.ST_01, true, 0.15f, 0.0f);

        if (!network) return;
        network.OnConnect += OnServerConnect;
        network.OnDisconnect += OnServerDisconnect;
    }

    private void OnDisable() {
        if (!network) return;
        network.OnConnect -= OnServerConnect;
        network.OnDisconnect -= OnServerDisconnect;
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

    /// <summary>
    /// Should be called by ChatBox when user logs in..
    /// ..or by login load page 
    /// </summary>
    public async void StartGame(PlayerData _player) {
        if (!network) return;
        if (network.IsConnected) {
            network.Close();
            await UniTask.Delay(1000);
        }
        if (m_NetworkPlayer != null) {
            m_NetworkPlayer.Dispose();
        }
        InitGame(_player);
        network.Connect();
    }

    public void FreezePlayerInput() {
        m_PlayerController.FreezeInput();
    }

    public void FreePlayerInput() {
        m_PlayerController.FreeInput();
    }

    public void LogoutToCharSelection() {
        if (network) {
            network.Close();
        }
        SaveSession();
        entryPage = PageType.CharacterSelection;
        if (sceneController) {
            sceneController.Load(SceneType.Login);
        }
    }
#endregion

#region Private Functions
    private void OnServerConnect() {
        m_Player.ConnectWithData(playerData);
        network.SendHandshake(new NetworkHandshake(m_NetworkPlayer.clientData, account, playerData.sessionData.ID));
        TryRunAction(OnPlayerConnected);
    }

    private void OnServerDisconnect() {
        // !! TODO
        // Implement 
        TryRunAction(OnPlayerDisconnected);
    }

    private void SaveSession() {
        if (m_AccountPlayers == null) return;
        foreach(PlayerData _player in m_AccountPlayers) {
            if (_player.player.name == playerData.player.name) {
                _player.sessionData.posX = player.transform.position.x;
                _player.sessionData.posY = player.transform.position.y;
                _player.sessionData.posZ = player.transform.position.z;
                _player.sessionData.rotX = player.transform.eulerAngles.x;
                _player.sessionData.rotY = player.transform.eulerAngles.y;
                _player.sessionData.rotZ = player.transform.eulerAngles.z;
            }
        }
    }

    private void InitTestGame() {
        PlayerData _playerData = new PlayerData();
        _playerData.stats = new StatData();
        _playerData.sessionData = new PlayerSessionData();
        _playerData.player = new PlayerInfo();
        _playerData.player.name = "Tester";
        _playerData.sessionData.posX = -21;
        _playerData.sessionData.posY = 35;
        _playerData.sessionData.posZ = 275;
        InitGame(_playerData);
    }

    private void InitGame(PlayerData _player) {
        InitPlayer(_player);
        InitSystems();
    }

    private void InitPlayer(PlayerData _playerData) {
        PlayerController _testPlayer = GameObject.FindObjectOfType<PlayerController>();
        if (_testPlayer) {
            Destroy(_testPlayer.gameObject);
        }

        GameObject _go = Instantiate(
            playerObject, 
            new Vector3(_playerData.sessionData.posX, _playerData.sessionData.posY, _playerData.sessionData.posZ),
            Quaternion.Euler(_playerData.sessionData.rotX, _playerData.sessionData.rotY, _playerData.sessionData.rotZ)
        );
        Player _player = _go.GetComponent<Player>();
        NetworkPlayer _netPlayer = _go.GetComponent<NetworkPlayer>();
        PlayerController _playerController = _go.GetComponent<PlayerController>();

        if (!_player || !_netPlayer || !_playerController) {
            LogError("Invalid playerObject instantiated when initializing game. Your player is missing one of type Player, NetworkPlayer or PlayerController.");
            return;
        }

        m_Player = _player;
        m_PlayerController = _playerController;
        m_NetworkPlayer = _netPlayer;
        m_NetworkPlayer.Init(_playerData);
        m_Player.ConnectWithData(_playerData);
        Camera.main.GetComponent<CameraController>().target = _go.transform;
    }

    private void InitSystems() {
        Chatbox.instance.ConfigurePlayerEvents();
    }

    private void TryRunAction(BasicAction _action) {
        try {
            _action();
        } catch (System.Exception) {}
    }
#endregion
}
