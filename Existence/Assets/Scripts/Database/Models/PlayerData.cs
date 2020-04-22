
public class PlayerInfo : NetworkModel
{
    public int ID;
    public string name;
}

public class PlayerData : NetworkModel
{
    public PlayerInfo player;
    public StatData stats;
}
