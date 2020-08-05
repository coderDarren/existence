using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chatbox : GameSystem
{
    public delegate void BasicAction();
    public event BasicAction OnChatStarted;
    public event BasicAction OnChatEnded;

    public static Chatbox instance;

    public InputField chat;
    public Text chatBox;

    private Session m_Session;
    private ChatCommandParser m_CommandParser;
    private NetworkController m_Network;
    private bool m_Active;

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
    private void Awake() {
        if (!instance) {
            instance = this;
        }
    }

    private void Start() {
        m_CommandParser = new ChatCommandParser();
        Mob.OnMobInit += OnMobInit;

        if (network) {
            network.OnConnect += OnServerConnect;
            network.OnDisconnect += OnServerDisconnect;
            network.OnHandshake += OnServerHandshake;
            network.OnPlayerJoined += OnPlayerJoined;
            network.OnPlayerLeft += OnPlayerLeft;
            network.OnChat += OnChat;
            network.OnInventoryAdded += OnInventoryAdded;
            network.OnAddInventoryFail += OnAddInventoryFail;
            network.OnPlayerHit += OnPlayerHit;
            network.OnMobAttackStart += OnMobAttackStart;
            network.OnMobHit += OnMobHit;
            network.OnMobDeath += OnMobDeath;
            network.OnPlayerEquipSuccess += OnPlayerEquipSuccess;
            network.OnPlayerUnequipSuccess += OnPlayerUnequipSuccess;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            if (m_Active) {
                EnterInput();
            } else {
                chat.ActivateInputField();
                m_Active = true;
                OnInputChanged();
            }
        }
    }

    private void OnDisable() {
        Mob.OnMobInit -= OnMobInit;

        if (network) {
            network.OnConnect -= OnServerConnect;
            network.OnDisconnect -= OnServerDisconnect;
            network.OnHandshake -= OnServerHandshake;
            network.OnPlayerJoined -= OnPlayerJoined;
            network.OnPlayerLeft -= OnPlayerLeft;
            network.OnChat -= OnChat;
            network.OnInventoryAdded -= OnInventoryAdded;
            network.OnAddInventoryFail -= OnAddInventoryFail;
            network.OnPlayerHit -= OnPlayerHit;
            network.OnMobAttackStart -= OnMobAttackStart;
            network.OnMobHit -= OnMobHit;
            network.OnMobDeath -= OnMobDeath;
            network.OnPlayerEquipSuccess -= OnPlayerEquipSuccess;
            network.OnPlayerUnequipSuccess -= OnPlayerUnequipSuccess;
        }

        if (session) {
            session.player.OnXpAdded -= OnPlayerXpAdded;
        }
    }
#endregion

#region Public Functions
    public void ConfigurePlayerEvents() {
        if (session) {
            session.player.OnXpAdded += OnPlayerXpAdded;
        }
    }

    public void OnInputChanged() {
        // tell session to stop the player
        session.FreezePlayerInput();
        TryRunAction(OnChatStarted);
    }

    public void OnInputEntered() {
        // tell session to free up the player
        session.FreePlayerInput();
        TryRunAction(OnChatEnded);
        StartCoroutine(CloseChatAtEndOfFrame());
    }

    // Emits message only for this client
    // Probably used by a service that needs to notify the player about something..
    // ..like gear not being compatible
    // !! TODO This should be coming from the server ultimately
    public void EmitMessageLocal(string _msg) {
        OnChat(_msg);
    }
#endregion

#region Private Functions
    private IEnumerator CloseChatAtEndOfFrame() {
        yield return new WaitForEndOfFrame();
        m_Active = false;
    }

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
                    chatBox.text += "\nCommand arguments were not understood.";
                    return;
                }
                Login(_args[1]);
                break;
            case ChatCommand.LOGOUT:
                chatBox.text += "\nCommand arguments were not understood.";
                session.LogoutToCharSelection();
                break;
            case ChatCommand.XP:
                if (_args.Count != 2) {
                    chatBox.text += "\nCommand arguments were not understood.";
                    return;
                }
                session.player.AddXp(int.Parse(_args[1]));
                break;
            case ChatCommand.INVENTORY:
                if (_args.Count < 2) {
                    chatBox.text += "\nCommand arguments were not understood.";
                    return;
                }
                if (_args[1] == "add") {
                    if (_args.Count < 3) {
                        chatBox.text += "\nExpected format [/inventory add <id>].";
                        return;
                    }
                    int _id;
                    if (System.Int32.TryParse(_args[2], out _id)) {
                        session.network.AddInventory(new ItemData(_id));
                    } else {
                        chatBox.text += "\nThe add parameter must be a number.";
                        return;
                    }
                } else {
                    chatBox.text += "\nCommand arguments were not understood.";
                    return;
                }
                break;
            case ChatCommand.UNKNOWN:
                chatBox.text += "\nCommand not recognized.";
                break;
        }
    }

    private async void Login(string _playerName) {
        Log("[Login]: Sending request...");
        chatBox.text += "\nLogging in as "+_playerName+"..";
        long _start = NetworkTimestamp.NowMilliseconds();
        PlayerData _data = await DatabaseService.GetService(true).GetPlayer(_playerName);
        Log("[Login]: ["+(NetworkTimestamp.NowMilliseconds()-_start)+"ms]: "+_data);

        if (_data != null) {
            session.StartGame(_data);
        } else {
            chatBox.text += "\nNo account for "+_playerName+" exists. Create one first.";
        }
    }

    private void OnServerConnect() {
        
    }

    private void OnServerDisconnect() {
        
    }

    private void OnServerHandshake(NetworkInstanceData _instance) {
        chatBox.text += "\n<color=#0f0>Welcome to the Server.</color>";
    }

    private void OnPlayerJoined(NetworkPlayerData _player) {
        chatBox.text += "\n<color=#0f0>"+_player.name+" joined.</color>";
    }
    
    private void OnPlayerLeft(NetworkPlayerData _player) {
        chatBox.text += "\n<color=#f00>"+_player.name+" left.</color>";
    }

    private void OnChat(string _msg) {
        chatBox.text += "\n<color=#0f0>"+_msg+"</color>";
    }

    private void OnPlayerXpAdded(int _xp) {
        chatBox.text += "\n<color=#fc0>You earned "+_xp+"xp.</color>";
    }

    private void OnInventoryAdded(string _itemStr) {
        if (!session) return;

        IItem _item = ItemData.CreateItem(_itemStr);
        
        chatBox.text += "\nItem "+_item.def.name+" was added to your inventory. "+_item.def.itemType.ToString();
        
        switch (_item.def.itemType) {
            case ItemType.WEAPON: chatBox.text += "\nWeapon type: "+(((WeaponItemData)_item).slotType.ToString()); break;
            case ItemType.ARMOR: chatBox.text += "\nArmor type: "+(((ArmorItemData)_item).slotType.ToString()); break;
            default: break;
        }

        session.player.AddInventory(_item);
    }

    private void OnAddInventoryFail(string _msg) {
        chatBox.text += "\n<color=#f00>"+_msg+"</color>";
    }

    private void OnPlayerHit(NetworkPlayerHitInfo _info) {
        if (!session) return;
        if (session.player.data.player.name != _info.playerName) return;
        
        chatBox.text += "\n<color=#ccc>"+_info.mobName+" hit you for "+_info.dmg+" points of damage.</color>";
    }

    private void OnMobAttackStart(NetworkMobAttackData _data) {
        if (!session) return;
        if (session.player.data.player.name != _data.playerName) return;

        chatBox.text += "\n<color=#f00>"+_data.mobName+" started attacking you.</color>";
    }

    private void OnMobHit(NetworkMobHitInfo _data) {
        if (!session) return;
        if (session.player.data.player.name != _data.playerName) return;

        chatBox.text += "\n<color=#fff>You hit "+_data.mobName+" for "+_data.dmg+" points of damage.</color>";
    }

    private void OnMobDeath(NetworkMobDeathData _data) {        
        foreach (NetworkLootPreviewData _preview in _data.lootPreview) {
            chatBox.text += "\n"+_data.name+" dropped a LV. "+_preview.level+" "+_preview.name+".";
        }
    }

    private void OnMobInit(Mob _mob) {
        foreach (NetworkLootPreviewData _preview in _mob.data.lootPreview) {
            chatBox.text += "\nYou are near a "+_mob.data.name+", which dropped a LV. "+_preview.level+" "+_preview.name+".";
        }
    }

    private void OnPlayerEquipSuccess(NetworkEquipSuccessData _data) {
        if (!session) return;
        if (session.player.data.player.ID == _data.playerID) {
            chatBox.text += "\nYou equipped item "+_data.itemID+".";
        } else {
            chatBox.text += "\n"+_data.playerName+" equipped item "+_data.itemID+".";
        }
    }

    private void OnPlayerUnequipSuccess(NetworkEquipSuccessData _data) {
        if (!session) return;
        if (session.player.data.player.ID == _data.playerID) {
            chatBox.text += "\nYou unequipped item "+_data.itemID+".";
        } else {
            chatBox.text += "\n"+_data.playerName+" unequipped item "+_data.itemID+".";
        }
    }

    private void TryRunAction(BasicAction _action) {
        try {
            _action();
        } catch (System.Exception) {}
    }
#endregion
}
