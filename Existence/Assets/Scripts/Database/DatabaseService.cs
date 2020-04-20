using UniRx.Async;
using UnityEngine;

public class DatabaseService
{
    private static readonly string SERVICE_ENDPOINT = "https://15paucwkia.execute-api.us-east-1.amazonaws.com/dev/api/";
    private static readonly string PATH_GETPLAYER = "getPlayer";

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

    public async UniTask<PlayerData> GetPlayer(string _playerName) {
        string _url = SERVICE_ENDPOINT+PATH_GETPLAYER+"?player="+_playerName;
        APIResponse _resp = await m_Rest.Get(_url);
        switch (_resp.statusCode) {
            case 200:
                try {
                    PlayerData _data = NetworkModel.FromJsonStr<PlayerData>(_resp.message);
                    Log("[GetPlayer]: raw"+_resp.message);
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

#region Private Functions
    private void Log(string _msg) {
        if (!debug) return;
        Debug.Log("[DatabaseService]: "+_msg);
    }
#endregion
}
