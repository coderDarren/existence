
using UnityEngine;

public class DatabaseServiceTester : GameSystem
{
    [Header("/api/getPlayer")]
    public string playerName;

    private DatabaseService m_Service;

#region Unity Functions
    private void Start() {

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.G)) {
            TestGetPlayer();
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
#endregion
}
