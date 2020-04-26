
using UnityEngine;

public class DatabaseServiceTester : GameSystem
{
    [Header("GET /api/getPlayer")]
    public string playerName;
    public KeyCode getPlayerKeyCode;

    [Header("POST /api/updateStats")]
    public StatData playerStats;
    public KeyCode updateStatsKeyCode;

    [Header("POST /api/updatePlayer")]
    public PlayerInfo playerInfo;
    public KeyCode updatePlayerKeyCode;

    private DatabaseService m_Service;

#region Unity Functions
    private void Start() {

    }

    private void Update() {
        if (Input.GetKeyDown(getPlayerKeyCode)) {
            TestGetPlayer();
        }

        if (Input.GetKeyDown(updateStatsKeyCode)) {
            TestUpdateStats();
        }

        if (Input.GetKeyDown(updatePlayerKeyCode)) {
            TestUpdatePlayer();
        }
    }
#endregion

#region Private Functions
    private async void TestGetPlayer() {
        Log("[TestGetPlayer]: Sending request...");
        long _start = NetworkTimestamp.NowMilliseconds();
        m_Service = DatabaseService.GetService(debug);
        PlayerData _player = await m_Service.GetPlayer(playerName);
        Log("["+(NetworkTimestamp.NowMilliseconds()-_start)+"ms] [TestGetPlayer]: "+_player);
    }

    private async void TestUpdateStats() {
        Log("[TestUpdateStats]: Sending request...");
        long _start = NetworkTimestamp.NowMilliseconds();
        m_Service = DatabaseService.GetService(debug);
        bool _res = await m_Service.UpdateStats(playerStats);
        Log("["+(NetworkTimestamp.NowMilliseconds()-_start)+"ms] [TestUpdateStats]: "+_res);
    }

    private async void TestUpdatePlayer() {
        Log("[TestUpdatePlayer]: Sending request...");
        long _start = NetworkTimestamp.NowMilliseconds();
        m_Service = DatabaseService.GetService(debug);
        bool _res = await m_Service.UpdatePlayer(playerInfo);
        Log("["+(NetworkTimestamp.NowMilliseconds()-_start)+"ms] [TestUpdatePlayer]: "+_res);
    }
#endregion
}
