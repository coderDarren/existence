
public class NetworkPlayerHitInfo : NetworkModel {
    public string mobId;
    public string mobName; 
    public string playerName;
    public int dmg;
}

/// <summary>
/// Data for individual players
/// Each player will send this info up for..
/// ..other players to receive
///
/// Ideally the client sends up their NetworkPlayerData..
/// ..and receives NetworkInstanceData
/// </summary>
public class NetworkPlayerData : NetworkModel
{
    public int id;
    public string name;
    public string weaponName;
    public string specialName;
    public NetworkPlayerInput input;
    public NetworkVector3 pos;
    public NetworkVector3 rot;
    public int health;
    public int maxHealth;
    public int energy;
    public int maxEnergy;

    public NetworkPlayerData() {
        input = new NetworkPlayerInput();
        pos = new NetworkVector3();
        rot = new NetworkVector3();
    }
}
