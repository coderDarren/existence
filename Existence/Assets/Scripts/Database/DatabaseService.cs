using UniRx.Async;
using UnityEngine;

public class DatabaseService
{
    private static readonly string SERVICE_ENDPOINT = "https://15paucwkia.execute-api.us-east-1.amazonaws.com/dev/api/";
    private static readonly string PATH_GETPLAYER = "getPlayer";
    private static readonly string PATH_CREATEACCOUNT = "createAccount";
    private static readonly string PATH_UPDATESTATS = "updateStats";
    private static readonly string PATH_UPDATEPLAYER = "updatePlayer";
    private static readonly string PATH_AUTHENTICATE = "authenticate";
    private static readonly string PATH_GETACCOUNTPLAYERS = "getAccountPlayers";

    private static DatabaseService m_Service;
    private RestService m_Rest;

    public bool debug;

    public static DatabaseService GetService(bool _debug) {
		if (m_Service == null) {
			m_Service = new DatabaseService();
		}

        m_Service.debug = _debug;

		return m_Service;
	}

    private DatabaseService() {
        m_Rest = RestService.GetService();
    }

    public async UniTask<AccountData> CreateAccount(AccountData _acct) {
        string _url = SERVICE_ENDPOINT+PATH_CREATEACCOUNT;
        APIResponse _resp = await m_Rest.Post(_url, _acct.ToJsonString());
        switch (_resp.statusCode) {
            case 200:
                try {
                    AccountData _data = NetworkModel.FromJsonStr<AccountData>(_resp.message);
                    Log("[CreateAccount]: raw "+_resp.message);
                    _data.creationResponseCode = _resp.statusCode;
                    return _data;
                } catch (System.Exception _err) {
                    Log("[CreateAccount]: "+_err.Message);
                    return new AccountData(_resp.statusCode);
                }
                break;
            default:
                Log("[CreateAccount]: "+_resp.message);
                return new AccountData(_resp.statusCode);;
                break;
        }
    }

    public async UniTask<PlayerData> GetPlayer(string _playerName) {
        string _url = SERVICE_ENDPOINT+PATH_GETPLAYER+"?player="+_playerName;
        APIResponse _resp = await m_Rest.Get(_url);
        switch (_resp.statusCode) {
            case 200:
                try {
                    PlayerData _data = NetworkModel.FromJsonStr<PlayerData>(_resp.message);
                    Log("[GetPlayer]: raw "+_resp.message);
                    return _data;
                } catch (System.Exception _err) {
                    Log("[GetPlayer]: "+_err.Message);
                    return null;
                }
                break;
            default:
                Log("[GetPlayer]: "+_resp.message);
                return null;
                break;
        }
    }

    public async UniTask<bool> UpdateStats(StatData _stats) {
        string _url = SERVICE_ENDPOINT+PATH_UPDATESTATS;
        APIResponse _resp = await m_Rest.Post(_url, _stats.ToJsonString());
        switch (_resp.statusCode) {
            case 200:
                Log("[UpdateStats]: raw "+_resp.message);
                return true;
                break;
            default:
                Log("[UpdateStats]: "+_resp.message);
                return false;
                break;
        }
    }

    public async UniTask<bool> UpdatePlayer(PlayerInfo _player) {
        string _url = SERVICE_ENDPOINT+PATH_UPDATEPLAYER;
        APIResponse _resp = await m_Rest.Post(_url, _player.ToJsonString());
        switch (_resp.statusCode) {
            case 200:
                Log("[UpdatePlayer]: raw "+_resp.message);
                return true;
                break;
            default:
                Log("[UpdatePlayer]: "+_resp.message);
                return false;
                break;
        }
    }

    public async UniTask<AccountData> Authenticate(AuthenticationData _auth) {
        string _url = SERVICE_ENDPOINT+PATH_AUTHENTICATE;
        APIResponse _resp = await m_Rest.Post(_url, _auth.ToJsonString());
        switch (_resp.statusCode) {
            case 200:
                Log("[Authenticate]: raw "+_resp.message);
                return NetworkModel.FromJsonStr<AccountData>(_resp.message);
                break;
            default:
                Log("[Authenticate]: "+_resp.message);
                return null;
                break;
        }
    }

    public async UniTask<PlayerData[]> GetAccountPlayers(AccountData _account) {
        string _url = SERVICE_ENDPOINT+PATH_GETACCOUNTPLAYERS+"?account="+_account.id+"&apiKey="+_account.apiKey;
        APIResponse _resp = await m_Rest.Get(_url);
        switch (_resp.statusCode) {
            case 200:
                Log("[GetAccountPlayers]: raw "+_resp.message);
                return NetworkModel.FromJsonStr<PlayerData[]>(_resp.message);
                break;
            default:
                Log("[GetAccountPlayers]: "+_resp.message);
                return null;
                break;
        }
    }

#region Private Functions
    private void Log(string _msg) {
        if (!debug) return;
        Debug.Log("[DatabaseService]: "+_msg);
    }
#endregion
}
