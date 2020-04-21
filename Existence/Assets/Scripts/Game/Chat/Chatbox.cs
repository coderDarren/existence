using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chatbox : GameSystem
{
    public InputField chat;
    public Text chatBox;

    private Session m_Session;
    private ChatCommandParser m_CommandParser;
    private NetworkController m_Network;

    // get Session with integrity
    private Session session {
        get {
            if (!m_Session) {
                m_Session = Session.instance;
            }
            if (!m_Session) {
                LogError("Trying to use Session, but no instance could be found.");
            }
            return m_Session;
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
    private void Start() {
        m_CommandParser = new ChatCommandParser();

        if (network) {
            network.OnConnect += OnServerConnect;
            network.OnDisconnect += OnServerDisconnect;
            network.OnHandshake += OnServerHandshake;
            network.OnPlayerJoined += OnPlayerJoined;
            network.OnPlayerLeft += OnPlayerLeft;
            network.OnChat += OnChat;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            EnterInput();
        }
    }

    private void OnDisable() {
        if (!network) return;
        network.OnConnect -= OnServerConnect;
        network.OnDisconnect -= OnServerDisconnect;
        network.OnHandshake -= OnServerHandshake;
        network.OnPlayerJoined -= OnPlayerJoined;
        network.OnPlayerLeft -= OnPlayerLeft;
        network.OnChat -= OnChat;
    }
#endregion

#region Public Functions
    public void OnInputChanged() {
        // tell session to stop the player
        session.FreezePlayerInput();
    }

    public void OnInputEntered() {
        // tell session to free up the player
        session.FreePlayerInput();
    }
#endregion

#region Private Functions
    private void EnterInput() {
        string _in = chat.text;
        // clear input
        chat.text = string.Empty;
        OnInputEntered();

        if (_in.Equals(string.Empty)) return;
        if (_in.StartsWith("/")) {
            ProcessPotentialCommand(_in);
            return;
        }
        // send input..
        network.SendChat(session.playerData.player.name+": "+_in);
    }

    private void ProcessPotentialCommand(string _in) {
        List<string> _args = null;
        ChatCommand _cmd = m_CommandParser.ParseCommand(_in, out _args);

        switch (_cmd) {
            case ChatCommand.LOGIN:
                if (_args.Count != 2) {
                    chatBox.text += "Command arguments were not understood.\n";
                    return;
                }
                Login(_args[1]);
                break;
            case ChatCommand.UNKNOWN:
                chatBox.text += "Command not recognized.\n";
                break;
        }
    }

    private async void Login(string _playerName) {
        Log("[Login]: Sending request...");
        chatBox.text += "Logging in as "+_playerName+"..\n";
        long _start = NetworkTimestamp.NowMilliseconds();
        PlayerData _data = await DatabaseService.GetService(true).GetPlayer(_playerName);
        Log("[Login]: ["+(NetworkTimestamp.NowMilliseconds()-_start)+"ms]: "+_data);

        if (_data != null) {
            session.ConnectPlayer(_data);
        } else {
            chatBox.text += "No account for "+_playerName+" exists. Create one first.\n";
        }
    }

    private void OnServerConnect() {
        
    }

    private void OnServerDisconnect() {
        
    }

    private void OnServerHandshake(NetworkInstanceData _instance) {
        chatBox.text += "<color=#0f0>Welcome to the Server.</color>\n";
    }

    private void OnPlayerJoined(NetworkPlayerData _player) {
        chatBox.text += "<color=#0f0>"+_player.name+" joined.</color>\n";
    }
    
    private void OnPlayerLeft(NetworkPlayerData _player) {
        chatBox.text += "<color=#f00>"+_player.name+" left.</color>\n";
    }

    private void OnChat(string _msg) {
        chatBox.text += _msg+"\n";
    }
#endregion
}
