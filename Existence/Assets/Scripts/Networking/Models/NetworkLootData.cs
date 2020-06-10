public class NetworkLootPreviewData : NetworkModel {
    public int id;
    public int level;
    public string name;
    public string icon;
    public bool locked;
}

public class NetworkPlayerLootData : NetworkModel {
    public string mobID;
    public int itemID;
    public NetworkPlayerLootData(string _mobID, int _itemID) {
        mobID = _mobID;
        itemID = _itemID;
    }
}

public class NetworkMobLootData : NetworkModel {
    public string playerID;
    public string mobID;
    public int itemID;
}