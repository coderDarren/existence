
[System.Serializable]
public class PlayerInfo : NetworkModel {
    public int ID;
    public string name;
    public int level;
    public int xp;
    public int statPoints;
}

public class PlayerSessionData : NetworkModel {
    public int ID;
    public float posX;
    public float posY;
    public float posZ;
    public float rotX;
    public float rotY;
    public float rotZ;
}

public class PlayerData : NetworkModel {
    public PlayerInfo player;
    public PlayerSessionData sessionData;
    public StatData stats;
    public ItemData[] inventory;
}
