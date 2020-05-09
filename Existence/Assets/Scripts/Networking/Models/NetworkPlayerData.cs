using UnityEngine;

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
    public string name;
    public string weaponName;
    public NetworkPlayerInput input;
    public NetworkVector3 pos;
    public NetworkVector3 rot;
    public int health;
    public int energy;
    

    public NetworkPlayerData() {
        input = new NetworkPlayerInput();
        pos = new NetworkVector3();
        rot = new NetworkVector3();
    }
}
