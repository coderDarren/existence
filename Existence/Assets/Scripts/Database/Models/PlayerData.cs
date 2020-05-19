
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

    /* Helper property to provide insight into player creation failure
     * 200 - OK
     * 1400 - Username must be at least 5 characters
     * 1401 - Username cannot begin with a number or special character
     * 1402 - Password must be at least 8 characters
     * 1403 - Password cannot begin with a special character or a number
     */
    public int responseCode;

    public PlayerData(int _responseCode) {
        responseCode = _responseCode;
    }
    public PlayerData() {}
}
