
[System.Serializable]
public class PlayerInfo : NetworkModel
{
    public int ID;
    public string name;
    public int level;
    public int xp;
    public int statPoints;
}

public class PlayerData : NetworkModel
{
    public PlayerInfo player;
    public StatData stats;
    public ItemData[] inventory;
}
