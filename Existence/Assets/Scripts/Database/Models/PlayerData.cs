
public class PlayerInfo : NetworkModel
{
    public string name;
}

public class PlayerData : NetworkModel
{
    public PlayerInfo player;
    public StatData stats;
}
